using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLanguageService.Parser
{
    public class RegexToken
    {
        public string Value { get; private set; }
        public RegexTokenType TokenType { get; private set; }

        public RegexToken(string value, RegexTokenType regexTokenType)
        {
            this.Value = value;
            this.TokenType = regexTokenType;
        }
    }
}
