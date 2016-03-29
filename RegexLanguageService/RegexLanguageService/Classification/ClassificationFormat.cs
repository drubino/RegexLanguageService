using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using RegexLanguageService;

namespace RegexLanguageService.Classification
{
    /// <summary>
    /// Defines the editor format for the Regex Quantifier classification type.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexQuantifier)]
    [Name(RegexStrings.RegexQuantifier)]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class RegexQuantifier : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "quantifier" classification type
        /// </summary>
        public RegexQuantifier()
        {
            DisplayName = "Quantifier"; //human readable version of the name
            ForegroundColor = Colors.BlueViolet;
        }
    }
}
