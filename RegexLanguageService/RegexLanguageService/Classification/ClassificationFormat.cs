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
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexCharacterClass)]
    [Name(RegexStrings.RegexCharacterClass)]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class RegexCharacterClass : ClassificationFormatDefinition
    {
        public RegexCharacterClass()
        {
            this.DisplayName = "Character Class";
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
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexAnchor)]
    [Name(RegexStrings.RegexAnchor)]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class RegexAnchor : ClassificationFormatDefinition
    {
        public RegexAnchor()
        {
            this.DisplayName = "Anchor";
            this.ForegroundColor = Colors.Yellow;
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
            this.DisplayName = "Escape Characer";
            this.BackgroundColor = Colors.DarkSlateGray;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexAlternation)]
    [Name(RegexStrings.RegexAlternation)]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class RegexAlternation : ClassificationFormatDefinition
    {
        public RegexAlternation()
        {
            this.DisplayName = "Alternation";
            this.ForegroundColor = Colors.Orange;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = RegexStrings.RegexDefault)]
    [Name(RegexStrings.RegexDefault)]
    [UserVisible(false)]
    [Order(Before = Priority.Default)]
    internal sealed class RegexDefault : ClassificationFormatDefinition
    {
        public RegexDefault()
        {
            this.DisplayName = "Default";
        }
    }
}
