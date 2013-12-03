

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


    [Export(typeof(ITaggerProvider))]
    [ContentType("racket")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class RacketClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name("racket")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition RacketContentType = null;

        [Export]
        [FileExtension(".plot")]
        [ContentType("racket")]
        internal static FileExtensionToContentTypeDefinition RacketFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<RacketTokenTag> racketTagAggregator =
                                            aggregatorFactory.CreateTagAggregator<RacketTokenTag>(buffer);

            return new RacketClassifier(buffer, racketTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal sealed class RacketClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<RacketTokenTag> _aggregator;
        IDictionary<RacketTokenTypes, IClassificationType> _racketTypes;

        internal RacketClassifier(ITextBuffer buffer,
                               ITagAggregator<RacketTokenTag> racketTagAggregator,
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = racketTagAggregator;
            _racketTypes = new Dictionary<RacketTokenTypes, IClassificationType>();
            _racketTypes[RacketTokenTypes.Define] = typeService.GetClassificationType("define");
            _racketTypes[RacketTokenTypes.Cdr] = typeService.GetClassificationType("cdr");
            _racketTypes[RacketTokenTypes.Car] = typeService.GetClassificationType("car");
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {

            foreach (var tagSpan in this._aggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return
                    new TagSpan<ClassificationTag>(tagSpans[0],
                                                   new ClassificationTag(_racketTypes[tagSpan.Tag.type]));
            }
        }
    }
}
