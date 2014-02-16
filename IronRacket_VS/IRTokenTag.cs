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
            _IRTypes[";"] = IRTokenTypes.IRComment;
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



        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
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
                    if (Regex.IsMatch(IRToken, @"define\s[a-z]+"))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, IRToken.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<IRTokenTag>(tokenSpan,
                                                                  new IRTokenTag(_IRTypes["define_var"]));
                        curLoc += IRToken.Length + 1;
                        continue;
                    }
                    if (_IRTypes.ContainsKey(IRToken.Trim()))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, IRToken.Length));
                        if( tokenSpan.IntersectsWith(curSpan) ) 
                            yield return new TagSpan<IRTokenTag>(tokenSpan, 
                                                                  new IRTokenTag(_IRTypes[IRToken.Trim()]));
                    }

                    //add an extra char location because of the space
                    curLoc += IRToken.Length + 1;
                }
            }
            
        }
    }
}
