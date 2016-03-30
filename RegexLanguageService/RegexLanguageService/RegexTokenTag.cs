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
        public RegexTokenTypes Type { get; private set; }

        public RegexTokenTag(RegexTokenTypes type)
        {
            this.Type = type;
        }
    }

    internal sealed class RegexTokenTagger : ITagger<RegexTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, RegexTokenTypes> regexTokenTypes;

        internal RegexTokenTagger(ITextBuffer buffer)
        {
            _buffer = buffer;
            regexTokenTypes = new Dictionary<string, RegexTokenTypes>();
            regexTokenTypes[RegexStrings.RegexQuantifier] = RegexTokenTypes.RegexQuantifier;
            regexTokenTypes[RegexStrings.RegexSingleCharacterMatch] = RegexTokenTypes.RegexSingleCharacterMatch;
            regexTokenTypes[RegexStrings.RegexCaptureGroup] = RegexTokenTypes.RegexCaptureGroup;
            regexTokenTypes[RegexStrings.RegexEscapeCharacter] = RegexTokenTypes.RegexEscapeCharacter;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged
        {
            add { }
            remove { }
        }

        public IEnumerable<ITagSpan<RegexTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            foreach (SnapshotSpan curSpan in spans)
            {
                var containingLine = curSpan.Start.GetContainingLine();
                var currentLocation = containingLine.Start.Position;
                var tokens = containingLine.GetText().Split(' ');

                foreach (var token in tokens)
                {
                    if (regexQuantifiers.Contains(token) || (token.StartsWith("{") && token.EndsWith("}")))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, token.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<RegexTokenTag>(tokenSpan,
                                                                  new RegexTokenTag(regexTokenTypes[RegexStrings.RegexQuantifier]));
                    }
                    else if (token.StartsWith("[") && token.EndsWith("]"))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, token.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<RegexTokenTag>(tokenSpan,
                                                                  new RegexTokenTag(regexTokenTypes[RegexStrings.RegexSingleCharacterMatch]));
                    }
                    else if (token.StartsWith("(") && token.EndsWith(")"))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, token.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<RegexTokenTag>(tokenSpan,
                                                                  new RegexTokenTag(regexTokenTypes[RegexStrings.RegexCaptureGroup]));
                    }
                    else if (escapeCharacters.Contains(token))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, token.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<RegexTokenTag>(tokenSpan,
                                                                  new RegexTokenTag(regexTokenTypes[RegexStrings.RegexEscapeCharacter]));
                    }

                    //add an extra char location because of the space
                    currentLocation += token.Length + 1;
                }
            }
        }

        private static string[] regexQuantifiers = new[]
        {
            "?",
            "*",
            "+"
        };

        private static string[] escapeCharacters = new[]
        {
            @"\t",
            @"\n",
            @"\r",
            @"\f", 
            @"\cX",
            @"\N",
            @"\NNN",
            @"\b", 
            @"\B", 
            @"\d", 
            @"\D",
            @"\s",     
            @"\S", 
            @"\w", 
            @"\W", 
            @"\Q",
            @"\U", 
            @"\L"
        };
    }
}
