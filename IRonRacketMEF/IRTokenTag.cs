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
            _buffer = buffer;
            _IRTypes = new Dictionary<string, IRTokenTypes>();
            _IRTypes["(define"] = IRTokenTypes.IRDefine;
            _IRTypes["(car"] = IRTokenTypes.IRCar;
            _IRTypes["(cdr"] = IRTokenTypes.IRCdr;
            _IRTypes[";"] = IRTokenTypes.IRComment;
            _IRTypes["(lambda"] = IRTokenTypes.IRLambda;
            _IRTypes["(cons"] = IRTokenTypes.IRCons;

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
                string[] tokens = containingLine.GetText().ToLower().Split(' ');

                foreach (string IRToken in tokens)
                {
                    if (_IRTypes.ContainsKey(IRToken))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc+1, IRToken.Length));
                        if( tokenSpan.IntersectsWith(curSpan) ) 
                            yield return new TagSpan<IRTokenTag>(tokenSpan, 
                                                                  new IRTokenTag(_IRTypes[IRToken]));
                    }

                    //add an extra char location because of the space
                    curLoc += IRToken.Length + 1;
                }
            }
            
        }
    }
}
