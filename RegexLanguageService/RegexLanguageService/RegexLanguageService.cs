using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegexLanguageService
{
    public class RegexLanguageService : LanguageService
    {
        #region Fields

        private LanguagePreferences languagePreferences;
        private RegexScanner scanner;

        #endregion //Fields

        #region Properties

        public override string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion //Properties

        #region Methods

        public override string GetFormatFilterList()
        {
            throw new NotImplementedException();
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if (this.languagePreferences == null)
            {
                this.languagePreferences = new LanguagePreferences(this.Site, typeof(RegexLanguageService).GUID, this.Name);
                this.languagePreferences.Init();
            }
            return this.languagePreferences;
        }

        public override IScanner GetScanner(IVsTextLines buffer)
        {
            if(this.scanner == null)
                this.scanner = new RegexScanner(buffer);

            return this.scanner;
        }

        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return new RegexAuthoringScope();
        }

        #endregion //Methods
    }
}
