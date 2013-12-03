namespace IronPlot.IronRacketVSPackage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    public enum RacketTokenTypes
    {
        Define, Cdr, Car
    }

    [Export(typeof(ITaggerProvider))]
    [ContentType("racket")]
    [TagType(typeof(RacketTokenTag))]
    internal sealed class RacketTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new RacketTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class RacketTokenTag : ITag
    {
        public RacketTokenTypes type { get; private set; }

        public RacketTokenTag(RacketTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class RacketTokenTagger : ITagger<RacketTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, RacketTokenTypes> _racketTypes;

        internal RacketTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            _racketTypes = new Dictionary<string, RacketTokenTypes>();
            _racketTypes["define"] = RacketTokenTypes.Define;
            _racketTypes["cdr"] = RacketTokenTypes.Cdr;
            _racketTypes["car"] = RacketTokenTypes.Car;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<RacketTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (SnapshotSpan curSpan in spans)
            {
                ITextSnapshotLine containingLine = curSpan.Start.GetContainingLine();
                int curLoc = containingLine.Start.Position;
                string[] tokens = containingLine.GetText().ToLower().Split(' ');

                foreach (string racketToken in tokens)
                {
                    if (_racketTypes.ContainsKey(racketToken))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(curLoc, racketToken.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<RacketTokenTag>(tokenSpan,
                                                                  new RacketTokenTag(_racketTypes[racketToken]));
                    }

                    //add an extra char location because of the space
                    curLoc += racketToken.Length + 1;
                }
            }

        }
    }
}
