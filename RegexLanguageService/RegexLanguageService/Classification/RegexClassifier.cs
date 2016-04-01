using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        internal IClassificationTypeRegistryService classificationTypeRegistry = null;

        [Import]
        internal IClassificationFormatMapService classificationMapService = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            var regexTagAggregator = aggregatorFactory.CreateTagAggregator<RegexTokenTag>(buffer);
            return new RegexClassifier(buffer, regexTagAggregator, this.classificationTypeRegistry, this.classificationMapService) as ITagger<T>;
        }
    }

    internal sealed class RegexClassifier : ITagger<ClassificationTag>
    {
        private ITextBuffer textBuffer;
        private ITagAggregator<RegexTokenTag> tagAggregator;
        private IClassificationTypeRegistryService typeService;
        private IClassificationFormatMapService mapService;
        private IDictionary<RegexTokenType, IClassificationType> regexTokenTypes;

        /// <summary>
        /// Construct the classifier and define search tokens
        /// </summary>
        internal RegexClassifier(ITextBuffer buffer, 
                               ITagAggregator<RegexTokenTag> regexTagAggregator, 
                               IClassificationTypeRegistryService typeService,
                               IClassificationFormatMapService mapService)
        {
            this.textBuffer = buffer;
            this.tagAggregator = regexTagAggregator;
            this.typeService = typeService;
            this.mapService = mapService;

            regexTokenTypes = new Dictionary<RegexTokenType, IClassificationType>();
            regexTokenTypes[RegexTokenType.RegexQuantifier] = typeService.GetClassificationType(RegexStrings.RegexQuantifier);
            regexTokenTypes[RegexTokenType.RegexCharacterClass] = typeService.GetClassificationType(RegexStrings.RegexCharacterClass);
            regexTokenTypes[RegexTokenType.RegexCaptureGroup] = typeService.GetClassificationType(RegexStrings.RegexCaptureGroup);
            regexTokenTypes[RegexTokenType.RegexAnchor] = typeService.GetClassificationType(RegexStrings.RegexAnchor);
            regexTokenTypes[RegexTokenType.RegexEscapeCharacter] = typeService.GetClassificationType(RegexStrings.RegexEscapeCharacter);
            regexTokenTypes[RegexTokenType.RegexAlternation] = typeService.GetClassificationType(RegexStrings.RegexAlternation);
            regexTokenTypes[RegexTokenType.Default] = typeService.GetClassificationType(RegexStrings.RegexDefault);
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
            var tags = new List<ITagSpan<ClassificationTag>>();
            foreach (var tagSpan in tagAggregator.GetTags(spans))
            {
                var tagType = tagSpan.Tag.Type;
                var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
                tags.Add(
                    new TagSpan<ClassificationTag>(tagSpans[0], 
                                                   new ClassificationTag(regexTokenTypes[tagType])));
            }

            return tags;
        }
    }
}
