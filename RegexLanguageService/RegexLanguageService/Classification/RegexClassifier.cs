using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using RegexLanguageService;

namespace RegexLanguageService.Classification
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(RegexStrings.RegexContentType)]
    [TagType(typeof(ClassificationTag))]
    internal sealed class RegexClassifierProvider : ITaggerProvider
    {
        [Export]
        [Name(RegexStrings.RegexContentType)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition RegexContentType = null;

        [Export]
        [FileExtension(RegexStrings.RegexFileExtension)]
        [ContentType(RegexStrings.RegexContentType)]
        internal static FileExtensionToContentTypeDefinition RegexFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var regexTagAggregator = aggregatorFactory.CreateTagAggregator<RegexTokenTag>(buffer);
            return new RegexClassifier(buffer, regexTagAggregator, ClassificationTypeRegistry) as ITagger<T>;
        }
    }

    internal sealed class RegexClassifier : ITagger<ClassificationTag>
    {
        ITextBuffer textBuffer;
        ITagAggregator<RegexTokenTag> tagAggregator;
        IDictionary<RegexTokenTypes, IClassificationType> regexTokenTypes;

        /// <summary>
        /// Construct the classifier and define search tokens
        /// </summary>
        internal RegexClassifier(ITextBuffer buffer, 
                               ITagAggregator<RegexTokenTag> regexTagAggregator, 
                               IClassificationTypeRegistryService typeService)
        {
            textBuffer = buffer;
            tagAggregator = regexTagAggregator;
            regexTokenTypes = new Dictionary<RegexTokenTypes, IClassificationType>();
            regexTokenTypes[RegexTokenTypes.RegexQuantifier] = typeService.GetClassificationType(RegexStrings.RegexQuantifier);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Search the given span for any instances of classified tags
        /// </summary>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (var tagSpan in tagAggregator.GetTags(spans))
            {
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                yield return 
                    new TagSpan<ClassificationTag>(tagSpans[0], 
                                                   new ClassificationTag(regexTokenTypes[tagSpan.Tag.Type]));
            }
        }
    }
}
