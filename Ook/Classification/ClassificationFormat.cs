using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace OokLanguage
{
    #region Format definition
    /// <summary>
    /// Defines an editor format for the OrdinaryClassification type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "define")]
    [Name("IRdefine")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRDefine : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRDefine()
        {
            this.DisplayName = "define"; //human readable version of the name
            this.ForegroundColor = Colors.BlueViolet;
        }
    }

    /// <summary>
    /// Defines an editor format for the OrdinaryClassification type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "car")]
    [Name("IRCar")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRCar : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRCar()
        {
            this.DisplayName = "car"; //human readable version of the name
            this.ForegroundColor = Colors.Green;
        }
    }

    /// <summary>
    /// Defines an editor format for the OrdinaryClassification type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "cdr")]
    [Name("IRCdr")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRCdr : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRCdr()
        {
            this.DisplayName = "cdr"; //human readable version of the name
            this.ForegroundColor = Colors.Orange;
        }
    }




    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "comment")]
    [Name("IRComment")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRComment : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRComment()
        {
            this.DisplayName = "comment"; //human readable version of the name
            this.ForegroundColor = Colors.Red;
        }
    }
    #endregion //Format definition
}
