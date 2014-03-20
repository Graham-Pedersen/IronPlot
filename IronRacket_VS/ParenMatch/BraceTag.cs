﻿using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace IRLanguage.ParenMatch
{
    [Export(typeof(EditorFormatDefinition))]
    [Name("bracematch")]
    [Order(After = "default")]
    [UserVisible(true)]
    public class NoteFormatDefinition : MarkerFormatDefinition
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NoteFormatDefinition"/> class.
        /// </summary>
        public NoteFormatDefinition()
        {
            //Set the visual properties of text tagged with the NoteTag.
            this.BackgroundColor = Colors.Black;
            this.ForegroundColor = Colors.Blue;
            this.Border = new Pen(new SolidColorBrush(Colors.Red), 2.0);
            this.ZOrder = 5;
            this.ForegroundBrush = new SolidColorBrush(Colors.Red);
        }

        #endregion Constructors


    }
}
