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
            this.DisplayName = "Quantifier"; //human readable version of the name
            this.ForegroundColor = Colors.BlueViolet;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexSingleCharacterMatch)]
    [Name(RegexStrings.RegexSingleCharacterMatch)]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class RegexSingleCharacterMatch : ClassificationFormatDefinition
    {
        public RegexSingleCharacterMatch()
        {
            this.DisplayName = "Range";
            this.ForegroundColor = Colors.Green;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexCaptureGroup)]
    [Name(RegexStrings.RegexCaptureGroup)]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class RegexCaptureGroupMatch : ClassificationFormatDefinition
    {
        public RegexCaptureGroupMatch()
        {
            this.DisplayName = "Capture Group";
            this.ForegroundColor = Colors.Crimson;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexEscapeCharacter)]
    [Name(RegexStrings.RegexEscapeCharacter)]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class RegexEscapeCharacter : ClassificationFormatDefinition
    {
        public RegexEscapeCharacter()
        {
            this.DisplayName = "Escape Character";
            this.BackgroundColor = Colors.DarkGray;
        }
    }
}
