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
        public RegexTokenTypes type { get; private set; }

        public RegexTokenTag(RegexTokenTypes type)
        {
            this.type = type;
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
            regexTokenTypes["ook!"] = RegexTokenTypes.OokExclamation;
            regexTokenTypes["ook."] = RegexTokenTypes.OokPeriod;
            regexTokenTypes["ook?"] = RegexTokenTypes.OokQuestion;
            regexTokenTypes[RegexStrings.RegexQuantifier] = RegexTokenTypes.RegexQuantifier;
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
                var tokens = containingLine.GetText().ToLower().Split(' ');

                foreach (var token in tokens)
                {
                    if (regexTokenTypes.ContainsKey(token))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, token.Length));
                        if( tokenSpan.IntersectsWith(curSpan) ) 
                            yield return new TagSpan<RegexTokenTag>(tokenSpan, 
                                                                  new RegexTokenTag(regexTokenTypes[token]));
                    }
                    else if (regexQuantifiers.Contains(token))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, token.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<RegexTokenTag>(tokenSpan,
                                                                  new RegexTokenTag(regexTokenTypes[RegexStrings.RegexQuantifier]));
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
    }
}
