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
using RegexLanguageService;

namespace RegexLanguageService.Intellisense
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType(RegexStrings.RegexContentType)]
    [Name(RegexStrings.RegexCompletion)]
    class RegexCompletionSourceProvider : ICompletionSourceProvider
    {
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new RegexCompletionSource(textBuffer);
        }
    }

    class RegexCompletionSource : ICompletionSource
    {
        private ITextBuffer textBuffer;
        private bool isDisposed = false;
        
        public RegexCompletionSource(ITextBuffer buffer)
        {
            this.textBuffer = buffer;
        }

        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (this.isDisposed)
                throw new ObjectDisposedException(nameof(RegexCompletionSource));

            List<Completion> completions = new List<Completion>();
            
            ITextSnapshot snapshot = this.textBuffer.CurrentSnapshot;
            var triggerPoint = (SnapshotPoint)session.GetTriggerPoint(snapshot);

            if (triggerPoint == null)
                return;

            var line = triggerPoint.GetContainingLine();
            SnapshotPoint start = triggerPoint;

            while (start > line.Start && !char.IsWhiteSpace((start - 1).GetChar()))
            {
                start -= 1;
            }

            var applicableTo = snapshot.CreateTrackingSpan(new SnapshotSpan(start, triggerPoint), SpanTrackingMode.EdgeInclusive);

            completionSets.Add(new CompletionSet("All", "All", applicableTo, completions, Enumerable.Empty<Completion>()));
        }

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}

