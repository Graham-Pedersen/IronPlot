namespace IRLanguage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using System.Text.RegularExpressions;

    [Export(typeof(ITaggerProvider))]
    [ContentType("IronRacket")]
    [TagType(typeof(IRTokenTag))]
    internal sealed class IRTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new IRTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class IRTokenTag : ITag 
    {
        public IRTokenTypes type { get; private set; }

        public IRTokenTag(IRTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class IRTokenTagger : ITagger<IRTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, IRTokenTypes> _IRTypes;

        internal IRTokenTagger(ITextBuffer buffer)
        {

            //TODO Maybe not match on strings but on the graph representation of the code is.
            _buffer = buffer;
            _IRTypes = new Dictionary<string, IRTokenTypes>();
            _IRTypes["define"] = IRTokenTypes.IRDefine;
            _IRTypes["car"] = IRTokenTypes.IRCar;
            _IRTypes["cdr"] = IRTokenTypes.IRCdr;
            _IRTypes["IRcomment"] = IRTokenTypes.IRComment;
            _IRTypes["lambda"] = IRTokenTypes.IRLambda;
            _IRTypes["cons"] = IRTokenTypes.IRCons;
            _IRTypes["cond"] = IRTokenTypes.IRCond;
            _IRTypes["set!"] = IRTokenTypes.IRSetbang;
            _IRTypes["if"] = IRTokenTypes.IRIf;
            _IRTypes["or"] = IRTokenTypes.IROr;
            _IRTypes["and"] = IRTokenTypes.IRAnd;
            _IRTypes["let"] = IRTokenTypes.IRLet;
            _IRTypes["letrec"] = IRTokenTypes.IRLetrec;
            _IRTypes["define_var"] = IRTokenTypes.IRDefinevar;
            _IRTypes["new"] = IRTokenTypes.IRNew;
            //_IRTypes["comment"] = IRTokenTypes.IRString;



        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        private int nextIndexOf(string s)
        {
            for (int i = 1; i < s.Length; i++)
            {
                if (s[i] == '"')
                {
                    return i;
                }
            }
            return -1;
        }


        public IEnumerable<ITagSpan<IRTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (SnapshotSpan curSpan in spans)
            {
                ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
                int curLoc = containingLine.Start.Position;
                string test = curSpan.GetText();
                string test2 = spans.ToString();
                string line = containingLine.GetText().ToLower();
                Regex.Replace(line, @"\s+", "");
                line = line.Replace("[", "(");
                string[] tokens = line.Split('(');

                foreach (string IRToken in tokens)
                {
                    //IRToken.Trim();
                    int len = 0;
                    bool add = true;
                    bool add2 = false;
                    if (Regex.IsMatch(IRToken, @"define\s[a-z]+"))
                    {

                        
                        bool set = add = false;
                        string[] temptok = IRToken.Split(' ');
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, IRToken.Length));
                        if (temptok.Length >= 2)
                        {
                            len = IRToken.IndexOf(temptok[1]) + temptok[1].Length + 1;
                            if (curLoc + len < curSpan.Snapshot.Length)
                            {
                                tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, len));
                                set = true;
                            }
                            else { len = 0; }
                        }


                            if (tokenSpan.IntersectsWith(curSpan))
                                yield return new TagSpan<IRTokenTag>(tokenSpan,
                                                                      new IRTokenTag(_IRTypes["define_var"]));
                        if (set)
                        {
                            add2 = true;
                            add = true;
                            //curLoc += len + 1;
                                            
                        }
                        else
                        {
                            curLoc += IRToken.Length + 1;
                        }
                        
                    }
                    else if (_IRTypes.ContainsKey(IRToken.Trim()))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, IRToken.Length));
                        if( tokenSpan.IntersectsWith(curSpan) ) 
                            yield return new TagSpan<IRTokenTag>(tokenSpan, 
                                                                  new IRTokenTag(_IRTypes[IRToken.Trim()]));
                        curLoc += IRToken.Length + 1;
                        add = false;
                    }
                    if (IRToken.Contains("\""))
                    {
                        string temp = IRToken.Substring(0);
                        int lastindexof = 0;
                        int t = 0;
                        int location =0;
                        if (temp.Contains("\""))
                        {
                        more:
                            location = temp.IndexOf('\"');
                            string temp2 = temp.Substring(location);    
                            if ((lastindexof = nextIndexOf(temp.Substring(location))) <= 1)
                            {
                                curLoc += temp.Length;
                                continue;
                            }
                            curLoc += t = (temp.IndexOf('"'));
                            var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, lastindexof+1));
                            yield return new TagSpan<IRTokenTag>(tokenSpan,
                                                                  new IRTokenTag(_IRTypes["IRcomment"]));
                            curLoc += lastindexof + 1;
                            temp = temp.Substring(lastindexof+t+1);
                            if (temp.Contains("\""))
                            {
                                goto more;
                            }
                            if (add2)
                            {
                                curLoc += temp.Length+1;
                                continue;
                            }
                            curLoc += temp.Length;
                            continue;
                        }

                    }

                    //add an extra char location because of the space
                    if (add)
                    {           
                           curLoc += IRToken.Trim().Length + 1;
                           if (IRToken.Trim().Length != 0)
                           {
                               curLoc++;
                           }
                    }
                    //if (add2) { curLoc += IRToken.Length + 2; }
                }
            }
            
        }
    }
}
