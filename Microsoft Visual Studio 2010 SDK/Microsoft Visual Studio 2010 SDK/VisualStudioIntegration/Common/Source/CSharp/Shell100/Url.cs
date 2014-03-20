//--------------------------------------------------------------------------
//  <copyright file="Url.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
//  </copyright>
//  <summary>
//  </summary>
//--------------------------------------------------------------------------
using System;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Xml;
using System.Text;
using System.Net;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace Microsoft.VisualStudio.Shell
{	
    /// <summary>
    /// This class wraps the Uri class and provides an unescaped "LocalPath" for file URL's
    /// and an unescaped AbsoluteUri for other schemes, plus it also returned an un-hex-escaped
    /// result from MakeRelative so it can be presented to the user.
    /// </summary>
    public class Url
    {
        private Uri uri = null;
        private bool isFile;
        private string cachedAbsUrl;

        
        public Url(string path)
        {
            Init(path);
        }
        
        void Init(string path)
        {
            // Must try absolute first, then fall back on relative, otherwise it
            // makes some absolute UNC paths like (\\lingw11\Web_test\) relative!
            if (path != null)
            {

                if (!Uri.TryCreate(path, UriKind.Absolute, out this.uri))
                {
                    Uri.TryCreate(path, UriKind.Relative, out this.uri);
                } 
                
                this.CheckIsFile();
            }
        }

        void CheckIsFile()
        {
            this.isFile = false;
            if (this.uri != null)
            {
                if (this.uri.IsAbsoluteUri)
                {
                    this.isFile = this.uri.IsFile;
                }
                else
                {
                    int numberSlashes = this.uri.OriginalString.Count<char>(ch => (ch == '/'));
                    int numberBackSlashes = this.uri.OriginalString.Count<char>(ch => (ch == '\\'));
                    // If there are no slashes in the string, or the number of slashes is smaller than the number of backslashes, consider this to be a File Uri.
                    if (numberSlashes == 0 || numberSlashes < numberBackSlashes)
                    {
                        this.isFile = true;
                    }
                }
            }
        }

        // allows relpath to be null, in which case it just returns the baseUrl.
        
        public Url(Url baseUrl, string relpath)
        {
            if (baseUrl.uri == null || !baseUrl.uri.IsAbsoluteUri)
            {
                Init(relpath);
            }
            else if (string.IsNullOrEmpty(relpath))
            {
                this.uri = baseUrl.uri;
            }
            else
            {
                Uri.TryCreate(baseUrl.uri, relpath, out this.uri);
            }
            CheckIsFile();
        }
        
        
        public string AbsoluteUrl
        {
            get
            {
                if (this.uri == null) return null;
                if (cachedAbsUrl != null) return cachedAbsUrl;
                if (this.uri.IsAbsoluteUri)
                {
                    if (this.isFile)
                    {
                        // Fix for build break. UriComponents.LocalPath is no longer available.
                        // return uri.GetComponents(UriComponents.LocalPath, UriFormat.SafeUnescaped);
                        cachedAbsUrl = uri.LocalPath;
                    }
                    else
                    {
                        cachedAbsUrl = uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.SafeUnescaped);
                    }
                }
                else
                {
                    cachedAbsUrl = uri.OriginalString;
                }
            return cachedAbsUrl;
            }
        }

        
        /// <summary>Returns the AbsoluteUrl for the parent directory containing the file 
        /// referenced by this URL object, where the Directory string is also unescaped.</summary>
        public string Directory
        {
            get 
            {
                string path = this.AbsoluteUrl;
                if (path == null) return null;
                if (this.isFile) {
                    return Path.GetDirectoryName(path);
                }

                int i = path.LastIndexOf('/');
                int len = (i > 0) ? i : path.Length;
                return path.Substring(0, len);
            }
        }

        
        public bool IsFile
        {
            get { return this.isFile; }
        }

        
        public Url Move(Url oldBase, Url newBase)
        {
            if (this.uri == null || oldBase.uri == null) return null;
            string rel = oldBase.uri.MakeRelativeUri(this.uri).ToString();
            return new Url(newBase, rel);
        }

        // return an un-escaped relative path
        
        public string MakeRelative(Url url)
        {
            if (this.uri == null || url.uri == null) return null;
            if (this.uri.Scheme != url.uri.Scheme || this.uri.Host != url.uri.Host)
            {
                // Then it cannot be relatavized (e.g from file:// to http://).
                return url.AbsoluteUrl;
            }
            // This will return a hex-escaped string.
            string rel = this.uri.MakeRelativeUri(url.uri).ToString();

            // So unescape it.
            return Unescape(rel, this.isFile);
        }

        const char c_DummyChar = (char)0xFFFF;

        private static char EscapedAscii(char digit, char next)
        {
            // Only accept hexadecimal characters
            if (!(((digit >= '0') && (digit <= '9'))
                || ((digit >= 'A') && (digit <= 'F'))
                || ((digit >= 'a') && (digit <= 'f'))))
            {
                return c_DummyChar;
            }

            int res = 0;
            if (digit <= '9')
                res = (int)digit - (int)'0';
            else if (digit <= 'F')
                res = ((int)digit - (int)'A') + 10;
            else
                res = ((int)digit - (int)'a') + 10;

            // Only accept hexadecimal characters
            if (!(((next >= '0') && (next <= '9'))
                || ((next >= 'A') && (next <= 'F'))
               || ((next >= 'a') && (next <= 'f'))))
            {
                return c_DummyChar;
            }

            res = res << 4;
            if (next <= '9')
                res += (int)next - (int)'0';
            else if (digit <= 'F')
                res += ((int)next - (int)'A') + 10;
            else
                res += ((int)next - (int)'a') + 10;

            return (char)(res);
        }

        
        public static string Unescape(string escaped, bool isFile)
        {
            if (String.IsNullOrEmpty(escaped))
            {
                return String.Empty;
            }

            byte[] bytes = null;
            char[] dest = new char[escaped.Length];
            int j = 0;

            for (int i = 0, end = escaped.Length; i < end; i++)
            {
                char ch = escaped[i];
                if (ch != '%')
                {
                    if (ch == '/' && isFile)
                    {
                        ch = Path.DirectorySeparatorChar;
                    }
                    dest[j++] = ch;
                }
                else
                {
                    int byteCount = 0;
                    // lazy initialization of max size, will reuse the array for next sequences
                    if (bytes == null)
                    {
                        bytes = new byte[end - i];
                    }

                    do
                    {
                        // Check on exit criterion
                        if ((ch = escaped[i]) != '%' || (end - i) < 3)
                        {
                            break;
                        }
                        // already made sure we have 3 characters in str
                        ch = EscapedAscii(escaped[i + 1], escaped[i + 2]);
                        if (ch == c_DummyChar)
                        {
                            //invalid hex sequence, we will out '%' character
                            ch = '%';
                            break;
                        }
                        else if (ch < '\x80')
                        {
                            // character is not part of a UTF-8 sequence
                            i += 2;
                            break;
                        }
                        else
                        {
                            //a UTF-8 sequence
                            bytes[byteCount++] = (byte)ch;
                            i += 3;
                        }
                    } while (i < end);

                    if (byteCount != 0)
                    {

                        int charCount = Encoding.UTF8.GetCharCount(bytes, 0, byteCount);
                        if (charCount != 0)
                        {
                            Encoding.UTF8.GetChars(bytes, 0, byteCount, dest, j);
                            j += charCount;
                        }
                        else
                        {
                            // the encoded, high-ANSI characters are not UTF-8 encoded
                            for (int k = 0; k < byteCount; ++k)
                            {
                                dest[j++] = (char)bytes[k];
                            }
                        }
                    }
                    if (i < end)
                    {
                        dest[j++] = ch;
                    }
                }
            }
            return new string(dest, 0, j);
        }

        
        public Uri Uri
        {
            get { return this.uri; }
        }

        /// Return unescaped path up to (but not including) segment i.
        public string GetPartial(int i)
        {
            return GetPartial(0, i);
        }

        public string GetPartial(int i, int j) {
            string path = JoinSegments(i, j);
            if (i == 0)
                if (!this.isFile) {
                    // prepend "http://host/"
                    path = this.uri.Scheme + "://" + this.uri.Host + '/' + path;
                } else if (this.uri.IsAbsoluteUri && this.uri.IsUnc && this.AbsoluteUrl.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase)) {
                    path = @"\\" + path;
                }
            return path;
        }

        
        /// Return unescaped relative path starting segment i.
        public string GetRemainder(int i)
        {
            return JoinSegments(i, -1);
        }
        
        // Unlike the Uri class, this ALWAYS succeeds, even on relative paths, and it
        // strips out the path separator characters
        public string[] Segments
        {
            get
            {
                if (this.uri == null)
                    return null;
                string path = this.AbsoluteUrl;
                if (this.isFile)
                {
                    if (path.EndsWith(@"\", StringComparison.OrdinalIgnoreCase))
                    {
                        path = path.Substring(0, path.Length - 1);
                    }
                    if (this.uri.IsAbsoluteUri && this.uri.IsUnc && path.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase))
                    {
                        path = path.Substring(2);
                    }
                    return path.Split(Path.DirectorySeparatorChar);
                }
                else
                {
                    // strip off "http://" and host name, since those are not part of the path.
                    path = path.Substring(this.uri.Scheme.Length + 3 + this.uri.Host.Length + 1);
                    if (path.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                        path = path.Substring(0, path.Length - 1);
                    return path.Split('/');
                }
            }
        }

        public string JoinSegments(int i, int j)
        {
            if (i < 0)
                throw new ArgumentOutOfRangeException("i");

            StringBuilder sb = new StringBuilder();
            string[] segments = this.Segments;
            if (segments == null)
                return null;
            if (j < 0)
                j = segments.Length;
            int len = segments.Length;
            for (; i < j && i < len; i++)
            {
                if (sb.Length > 0)
                    sb.Append(this.isFile ? Path.DirectorySeparatorChar : '/');
                string s = segments[i];
                sb.Append(s);
            }
            return Unescape(sb.ToString(), isFile);
        }
    }
}
