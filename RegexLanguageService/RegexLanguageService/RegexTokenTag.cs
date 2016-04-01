using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using RegexLanguageService;
using RegexLanguageService.Parser;

namespace RegexLanguageService
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(RegexStrings.RegexContentType)]
    [TagType(typeof(RegexTokenTag))]
    internal sealed class RegexTokenTagProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new RegexTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class RegexTokenTag : ITag 
    {
        public RegexTokenType Type { get; private set; }

        public RegexTokenTag(RegexTokenType type)
        {
            this.Type = type;
        }
    }

    internal sealed class RegexTokenTagger : ITagger<RegexTokenTag>
    {
        #region Fields

        private ITextBuffer textBuffer;
        private Dictionary<string, RegexTokenType> regexTokenTypes;

        #endregion //Fields

        #region Events

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        #endregion //Events

        #region Constructors

        internal RegexTokenTagger(ITextBuffer buffer)
        {
            this.textBuffer = buffer;
            this.regexTokenTypes = new Dictionary<string, RegexTokenType>();
            this.regexTokenTypes[RegexStrings.RegexDefault] = RegexTokenType.Default;
            this.regexTokenTypes[RegexStrings.RegexQuantifier] = RegexTokenType.RegexQuantifier;
            this.regexTokenTypes[RegexStrings.RegexCharacterClass] = RegexTokenType.RegexCharacterClass;
            this.regexTokenTypes[RegexStrings.RegexCaptureGroup] = RegexTokenType.RegexCaptureGroup;
            this.regexTokenTypes[RegexStrings.RegexAnchor] = RegexTokenType.RegexAnchor;
            this.regexTokenTypes[RegexStrings.RegexEscapeCharacter] = RegexTokenType.RegexEscapeCharacter;
            this.regexTokenTypes[RegexStrings.RegexAlternation] = RegexTokenType.RegexAlternation;
        }

        #endregion //Constructors

        #region Methods

        public IEnumerable<ITagSpan<RegexTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var tagSpans = new List<TagSpan<RegexTokenTag>>();
            foreach (SnapshotSpan curSpan in spans)
            {
                var containingLine = curSpan.Start.GetContainingLine();
                var currentLocation = containingLine.Start.Position;
                var text = containingLine.GetText();
                var regexTokens = RegexParser.Parse(text).ToList();

                foreach (var token in regexTokens)
                {
                    var tokenValue = token.Value;
                    var tokenType = token.TokenType;
                    var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, tokenValue.Length));
                    switch (tokenType)
                    {
                        case RegexTokenType.Default:
                            if (tokenSpan.IntersectsWith(curSpan))
                                tagSpans.Add(new TagSpan<RegexTokenTag>(tokenSpan, new RegexTokenTag(regexTokenTypes[RegexStrings.RegexDefault])));
                            break;
                        case RegexTokenType.RegexQuantifier:
                            if (tokenSpan.IntersectsWith(curSpan))
                                tagSpans.Add(new TagSpan<RegexTokenTag>(tokenSpan, new RegexTokenTag(regexTokenTypes[RegexStrings.RegexQuantifier])));
                            break;
                        case RegexTokenType.RegexCharacterClass:
                            if (tokenSpan.IntersectsWith(curSpan))
                                tagSpans.Add(new TagSpan<RegexTokenTag>(tokenSpan, new RegexTokenTag(regexTokenTypes[RegexStrings.RegexCharacterClass])));
                            break;
                        case RegexTokenType.RegexCaptureGroup:
                            if (tokenSpan.IntersectsWith(curSpan))
                                tagSpans.Add(new TagSpan<RegexTokenTag>(tokenSpan, new RegexTokenTag(regexTokenTypes[RegexStrings.RegexCaptureGroup])));
                            break;
                        case RegexTokenType.RegexAnchor:
                            if (tokenSpan.IntersectsWith(curSpan))
                                tagSpans.Add(new TagSpan<RegexTokenTag>(tokenSpan, new RegexTokenTag(regexTokenTypes[RegexStrings.RegexAnchor])));
                            break;
                        case RegexTokenType.RegexEscapeCharacter:
                            if (tokenSpan.IntersectsWith(curSpan))
                                tagSpans.Add(new TagSpan<RegexTokenTag>(tokenSpan, new RegexTokenTag(regexTokenTypes[RegexStrings.RegexEscapeCharacter])));
                            break;
                        case RegexTokenType.RegexAlternation:
                            if (tokenSpan.IntersectsWith(curSpan))
                                tagSpans.Add(new TagSpan<RegexTokenTag>(tokenSpan, new RegexTokenTag(regexTokenTypes[RegexStrings.RegexAlternation])));
                            break;
                        default:
                            throw new InvalidOperationException(string.Format("Unrecognized RegexTokenType {0}", tokenType));
                    }
                    currentLocation += tokenValue.Length;
                }
            }

            return tagSpans;
        }

        #endregion //Methods
    }
}
