using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace OokLanguage
{
    internal static class OrdinaryClassificationDefinition
    {
        #region Type definition

        /// <summary>
        /// Defines the "ordinary" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("define")]
        internal static ClassificationTypeDefinition IRDefine = null;

        /// <summary>
        /// Defines the "ordinary" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("car")]
        internal static ClassificationTypeDefinition IRCar = null;

        /// <summary>
        /// Defines the "ordinary" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("cdr")]
        internal static ClassificationTypeDefinition IRCdr = null;


        [Export(typeof(ClassificationTypeDefinition))]
        [Name(";")]
        internal static ClassificationTypeDefinition IRComment = null;

        #endregion
    }
}
