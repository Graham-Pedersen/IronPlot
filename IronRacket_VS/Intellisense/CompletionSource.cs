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
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("IronRacket")]
    [Name("IRCompletion")]
    class IRCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new IRCompletionSource(textBuffer);
        }
    }

    class IRCompletionSource : ICompletionSource
    {
        private ITextBuffer _buffer;
        private bool _disposed = false;

        public IRCompletionSource(ITextBuffer buffer)
        {
            _buffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (_disposed)
                throw new ObjectDisposedException("IRCompletionSource");

            List<Completion> completions = new List<Completion>()
            {
                new Completion("(define"),
                new Completion("(car"),
                new Completion("(cdr"),
                new Completion("(lambda"),
            };

            string text =  _buffer.CurrentSnapshot.GetText();
           FileState.parsePlot.GetMethods(text,ref completions);

            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);


           
            if (triggerPoint == null)
                return;

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = triggerPoint;

            //string text = line.GetText().ToLower();
            
            //FileState.IntelliSense_filterer.Filter_list(completions, text);

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
            {
                start -= 1;
            }

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);
            //List<Completion> completions = new List<Completion>(); 
            completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}

