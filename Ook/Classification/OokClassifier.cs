// Copyright (c) Microsoft Corporati
// All rights reserved

namespace OokLanguage
{
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Adornments;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    [Export(typeof(ITaggerProvider))]
    [ContentType("IronRacket")]
    [TagType(typeof(ClassificationTag))]
    internal sealed class OokClassifierProvider : ITaggerProvider
    {

        [Export]
        [Name("IronRacket")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition OokContentType = null;

        [Export]
        [FileExtension(".plot")]
        [ContentType("IronRacket")]
        internal static FileExtensionToContentTypeDefinition OokFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {

            ITagAggregator<OokTokenTag> ookTagAggregator = 
                                            aggregatorFactory.CreateTagAggregator<OokTokenTag>(buffer);

            return new OokClassifier(buffer, ookTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal sealed class OokClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer _buffer;
        ITagAggregator<OokTokenTag> _aggregator;
        IDictionary<OokTokenTypes, IClassificationType> _ookTypes;

        internal OokClassifier(ITextBuffer buffer, 
                               ITagAggregator<OokTokenTag> ookTagAggregator, 
                               IClassificationTypeRegistryService typeService)
        {
            _buffer = buffer;
            _aggregator = ookTagAggregator;
            _ookTypes = new Dictionary<OokTokenTypes, IClassificationType>();
            _ookTypes[OokTokenTypes.IRDefine] = typeService.GetClassificationType("define");
            _ookTypes[OokTokenTypes.IRCar] = typeService.GetClassificationType("car");
            _ookTypes[OokTokenTypes.IRCdr] = typeService.GetClassificationType("cdr");
            _ookTypes[OokTokenTypes.IRComment] = typeService.GetClassificationType(";");
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
                                                   new ClassificationTag(_ookTypes[tagSpan.Tag.type]));
            }
        }
    }
}
