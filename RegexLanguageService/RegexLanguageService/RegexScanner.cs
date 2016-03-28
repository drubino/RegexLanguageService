using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLanguageService
{
    internal class RegexScanner : IScanner
    {
        private IVsTextBuffer buffer;
        private string source;

        public RegexScanner(IVsTextBuffer buffer)
        {
            this.buffer = buffer;
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            tokenInfo.Type = TokenType.Unknown;
            tokenInfo.Color = TokenColor.Text;
            return true;
        }

        public void SetSource(string source, int offset)
        {
            this.source = source.Substring(offset);
        }
    }
}
