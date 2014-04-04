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
            this.ForegroundColor = Colors.SteelBlue;
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
    [ClassificationType(ClassificationTypeNames = "while")]
    [Name("IRWhile")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRWhile : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRWhile()
        {
            this.DisplayName = "while"; //human readable version of the name
            this.ForegroundColor = Colors.DodgerBlue;
            this.IsBold = false;

        }
    }



    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "begin")]
    [Name("IRBegin")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRBegin : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRBegin()
        {
            this.DisplayName = "while"; //human readable version of the name
            this.ForegroundColor = Colors.DodgerBlue;
            this.IsBold = false;

        }
    }




  [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "null?")]
    [Name("IRNullhuh")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRNullhuh : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRNullhuh()
        {
            this.DisplayName = "null?"; //human readable version of the name
            this.ForegroundColor = Colors.Indigo;
            this.IsBold = false;

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "map")]
    [Name("IRMap")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRMap : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRMap()
        {
            this.DisplayName = "map"; //human readable version of the name
            this.ForegroundColor = Colors.DodgerBlue;
            this.IsBold = false;

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "equal?")]
    [Name("IREqualhuh")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IREqualhuh : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IREqualhuh()
        {
            this.DisplayName = "equal?"; //human readable version of the name
            this.ForegroundColor = Colors.Indigo;
            this.IsBold = false;

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "not")]
    [Name("IRNot")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRNot : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRNot()
        {
            this.DisplayName = "not"; //human readable version of the name
            this.ForegroundColor = Colors.SteelBlue;
            this.IsBold = true;

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "call")]
    [Name("IRCall")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRCall : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRCall()
        {
            this.DisplayName = "call"; //human readable version of the name
            this.ForegroundColor = Colors.BlueViolet;
            this.IsBold = false;

        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "scall")]
    [Name("IRScall")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRScall : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRScall()
        {
            this.DisplayName = "scall"; //human readable version of the name
            this.ForegroundColor = Colors.BlueViolet;
            this.IsBold = false;

        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "displayln")]
    [Name("IRDisplayln")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRDisplayln : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRDisplayln()
        {
            this.DisplayName = "displayln"; //human readable version of the name
            this.ForegroundColor = Colors.Black;
            this.IsBold = true;

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "bool")]
    [Name("IRBool")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRBool : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRBool()
        {
            this.DisplayName = "bool"; //human readable version of the name
            this.ForegroundColor = Colors.Blue;
            this.IsBold = true;

        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "using")]
    [Name("IRUsing")]
    //this should be visible to the end user
    [UserVisible(false)]
    //set the priority to be after the default classifiers
    [Order(Before = Priority.Default)]
    internal sealed class IRUsing : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "ordinary" classification type
        /// </summary>
        public IRUsing()
        {
            this.DisplayName = "using"; //human readable version of the name
            this.ForegroundColor = Colors.Blue;
            this.IsBold = false;

        }
    }



    #endregion //Format definition
}
