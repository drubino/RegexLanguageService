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
        internal static ClassificationTypeDefinition regexQuantifier;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexCharacterClass)]
        internal static ClassificationTypeDefinition regexRange;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexCaptureGroup)]
        internal static ClassificationTypeDefinition regexCaptureGroup;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexAnchor)]
        internal static ClassificationTypeDefinition regexAnchor;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexDefault)]
        internal static ClassificationTypeDefinition regexDefault;

        #endregion
    }
}
