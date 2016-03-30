using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace RegexLanguageService.Intellisense
{
    /// <summary>
    /// Factory for quick info sources
    /// </summary>
    [Export(typeof(IQuickInfoSourceProvider))]
    [ContentType(RegexStrings.RegexContentType)]
    [Name(RegexStrings.RegexQuickInfo)]
    class RegexQuickInfoSourceProvider : IQuickInfoSourceProvider
    {

        [Import]
        IBufferTagAggregatorFactoryService aggService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return new RegexQuickInfoSource(textBuffer, aggService.CreateTagAggregator<RegexTokenTag>(textBuffer));
        }
    }

    /// <summary>
    /// Provides QuickInfo information to be displayed in a text buffer
    /// </summary>
    class RegexQuickInfoSource : IQuickInfoSource
    {
        private ITagAggregator<RegexTokenTag> tagAggregator;
        private ITextBuffer textBuffer;
        private bool isDisposed = false;


        public RegexQuickInfoSource(ITextBuffer buffer, ITagAggregator<RegexTokenTag> aggregator)
        {
            this.tagAggregator = aggregator;
            this.textBuffer = buffer;
        }

        /// <summary>
        /// Determine which pieces of Quickinfo content should be displayed
        /// </summary>
        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> quickInfoContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;

            if (this.isDisposed)
                throw new ObjectDisposedException(nameof(RegexQuickInfoSource));

            var triggerPoint = (SnapshotPoint) session.GetTriggerPoint(this.textBuffer.CurrentSnapshot);

            if (triggerPoint == null)
                return;

            foreach (var currentTag in tagAggregator.GetTags(new SnapshotSpan(triggerPoint, triggerPoint)))
            {
                var tagType = currentTag.Tag.Type;
                var tagSpan = currentTag.Span.GetSpans(textBuffer).First();
                switch (tagType)
                {
                    case RegexTokenTypes.RegexQuantifier:
                        
                        applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                        quickInfoContent.Add("Quantifier");
                        break;
                    case RegexTokenTypes.RegexSingleCharacterMatch:
                        applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                        quickInfoContent.Add("Range");
                        break;
                    case RegexTokenTypes.RegexCaptureGroup:
                        applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                        quickInfoContent.Add("Capture Group");
                        break;
                    case RegexTokenTypes.RegexEscapeCharacter:
                        applicableToSpan = this.textBuffer.CurrentSnapshot.CreateTrackingSpan(tagSpan, SpanTrackingMode.EdgeExclusive);
                        quickInfoContent.Add("Escape Character");
                        break;
                    default:
                        throw new InvalidOperationException(string.Format("Unrecognized RegexTokenType {0}", tagType));
                }
            }
        }

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}

