using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using RegexLanguageService;

namespace RegexLanguageService.Classification
{
    internal static class OrdinaryClassificationDefinition
    {
        #region Type definition
        
        /// <summary>
        /// Defines the "regexQuantifier" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexQuantifier)]
        internal static ClassificationTypeDefinition regexQuantifier = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexSingleCharacterMatch)]
        internal static ClassificationTypeDefinition regexRange = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexCaptureGroup)]
        internal static ClassificationTypeDefinition regexCaptureGroup = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexEscapeCharacter)]
        internal static ClassificationTypeDefinition regexEscapeCharacter = null;

        #endregion
    }
}
