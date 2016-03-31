using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RegexLanguageService
{
    public enum RegexTokenType
    {
        Default,
        RegexQuantifier,
        RegexCharacterClass,
        RegexCaptureGroup,
        RegexAnchor
    }
}
