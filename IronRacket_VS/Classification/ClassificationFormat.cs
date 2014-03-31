using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace IRLanguage
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
            this.IsBold = true;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "define_var")]
    [Name("IRDefinevar")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRDefinevar : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRDefinevar()
        {
            this.DisplayName = "define_var"; //human readable version of the name
            //36C99F
            this.ForegroundColor = Colors.MediumTurquoise;//Color.FromRgb(0x36,0xC9,0x9F);
            this.ForegroundOpacity = 100D;
            this.IsBold = true;
        }
    }



    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "let")]
    [Name("IRLet")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRLet : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRLet()
        {
            this.DisplayName = "let"; //human readable version of the name
            this.ForegroundColor = Colors.ForestGreen;
            this.IsBold = true;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "letrec")]
    [Name("IRLetrec")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRLetrec : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRLetrec()
        {
            this.DisplayName = "letrec"; //human readable version of the name
            this.ForegroundColor = Colors.Peru;
            this.IsBold = true;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "cond")]
    [Name("IRCond")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRCond : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRCond()
        {
            this.DisplayName = "cond"; //human readable version of the name
            this.ForegroundColor = Colors.Purple;
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "and")]
    [Name("IRAnd")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRAnd : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRAnd()
        {
            this.DisplayName = "and"; //human readable version of the name
            this.ForegroundColor = Colors.SteelBlue;
            this.IsBold = true;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "or")]
    [Name("IROr")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IROr : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IROr()
        {
            this.DisplayName = "or"; //human readable version of the name
            this.ForegroundColor = Colors.Bisque;
            this.IsBold = true;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "set!")]
    [Name("IRSetbang")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRSetbang : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRSetbang()
        {
            this.DisplayName = "set!"; //human readable version of the name
            this.ForegroundColor = Colors.Tomato;
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "if")]
    [Name("IRIf")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRIf : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRIf()
        {
            this.DisplayName = "if"; //human readable version of the name
            this.ForegroundColor = Colors.Blue;
            this.IsBold = true;
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
            this.IsBold = true;
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
            this.IsBold = true;
        }
    }




    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "IRcomment")]
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
            this.DisplayName = "IRcomment"; //human readable version of the name
            this.ForegroundColor = Colors.Maroon;
            this.IsBold = false;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "lambda")]
    [Name("IRLambda")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRLambda : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRLambda()
        {
            this.DisplayName = "lambda"; //human readable version of the name
            this.ForegroundColor = Colors.YellowGreen;
            this.IsBold = true;
        }
    }





    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "cons")]
    [Name("IRCons")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRCons : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRCons()
        {
            this.DisplayName = "cons"; //human readable version of the name
            this.ForegroundColor = Colors.SkyBlue;
            this.IsBold = true;
            
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "new")]
    [Name("IRNew")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRNew : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRNew()
        {
            this.DisplayName = "new"; //human readable version of the name
            this.ForegroundColor = Colors.Blue;
            this.IsBold = true;

        }
    }



    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "keyword")]
    [Name("IRkeyword")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRKeyword : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRKeyword()
        {
            this.DisplayName = "keyword"; //human readable version of the name
            this.ForegroundColor = Colors.Blue;
            this.IsBold = false;
        }
    }

    #endregion //Format definition
}
