using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace IRLanguage
{
    
    [Export(typeof(IQuickInfoSourceProvider))]
    [ContentType("IronRacket")]
    [Name("IRQuickInfo")]
    class IRQuickInfoSourceProvider : IQuickInfoSourceProvider
    {

        [Import]
        IBufferTagAggregatorFactoryService aggService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new IRQuickInfoSource(textBuffer, aggService.CreateTagAggregator<IRTokenTag>(textBuffer));
        }
    }

    class IRQuickInfoSource : IQuickInfoSource
    {
        private ITagAggregator<IRTokenTag> _aggregator;
        private ITextBuffer _buffer;
        private bool _disposed = false;


        public IRQuickInfoSource(ITextBuffer buffer, ITagAggregator<IRTokenTag> aggregator)
        {
            _aggregator = aggregator;
            _buffer = buffer;
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (_disposed)
                throw new ObjectDisposedException("TestQuickInfoSource");

            var triggerPoint = (SnapshotPoint) session.GetTriggerPoint(_buffer.CurrentSnapshot);

            if (triggerPoint == null)
                return;

            foreach (IMappingTagSpan<IRTokenTag> curTag in _aggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
            {
                if (curTag.Tag.type == IRTokenTypes.IRDefine)
                {
                    var tagSpan = curTag.Span.GetSpans(_buffer).First();
                    applicableToSpan = _buffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                    quickInfoContent.Add("Defining a new function!");
                }
            }
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

