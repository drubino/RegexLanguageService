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
        /// Defines the "ookExclamation" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook!")]
        internal static ClassificationTypeDefinition ookExclamation = null;

        /// <summary>
        /// Defines the "ookQuestion" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook?")]
        internal static ClassificationTypeDefinition ookQuestion = null;

        /// <summary>
        /// Defines the "ookPeriod" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("ook.")]
        internal static ClassificationTypeDefinition ookPeriod = null;

        /// <summary>
        /// Defines the "ookPeriod" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(RegexStrings.RegexQuantifier)]
        internal static ClassificationTypeDefinition regexQuantifier = null;

        #endregion
    }
}
