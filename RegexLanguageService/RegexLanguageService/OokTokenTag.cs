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

namespace OokLanguage
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("ook!")]
    [TagType(typeof(OokTokenTag))]
    internal sealed class OokTokenTagProvider : ITaggerProvider
    {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new OokTokenTagger(buffer) as ITagger<T>;
        }
    }

    public class OokTokenTag : ITag 
    {
        public RegexTokenTypes type { get; private set; }

        public OokTokenTag(RegexTokenTypes type)
        {
            this.type = type;
        }
    }

    internal sealed class OokTokenTagger : ITagger<OokTokenTag>
    {

        ITextBuffer _buffer;
        IDictionary<string, RegexTokenTypes> regexTokenTypes;

        internal OokTokenTagger(ITextBuffer buffer)
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

        public IEnumerable<ITagSpan<OokTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
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
                            yield return new TagSpan<OokTokenTag>(tokenSpan, 
                                                                  new OokTokenTag(regexTokenTypes[token]));
                    }
                    else if (regexQuantifiers.Contains(token))
                    {
                        var tokenSpan = new SnapshotSpan(curSpan.Snapshot, new Span(currentLocation, token.Length));
                        if (tokenSpan.IntersectsWith(curSpan))
                            yield return new TagSpan<OokTokenTag>(tokenSpan,
                                                                  new OokTokenTag(regexTokenTypes[RegexStrings.RegexQuantifier]));
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
