using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace RegexLanguageService.Intellisense
{
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("Template QuickInfo Controller")]
    [ContentType("text")]
    internal class QuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        #region Properties

        [Import]
        internal IQuickInfoBroker QuickInfoBroker { get; set; }

        #endregion //Properties

        #region Methods

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView,
            IList<ITextBuffer> subjectBuffers)
        {
            return new QuickInfoController(textView, subjectBuffers, this);
        }

        #endregion //Methods

    }
}