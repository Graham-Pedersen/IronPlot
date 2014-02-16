
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Project.Samples.IronRacketProject
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class ResourcesDescriptionAttribute : DescriptionAttribute
    {
        #region Fields
        private bool replaced;
        #endregion

        #region Constructors
        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="description">Attribute description.</param>
        public ResourcesDescriptionAttribute(string description)
            : base(description)
        {
        }
        #endregion

        #region Overriden Implementation
        /// <summary>
        /// Gets attribute description.
        /// </summary>
        public override string Description
        {
            get
            {
                if(!replaced)
                {
                    replaced = true;
                    DescriptionValue = Resources.GetString(base.Description);
                }

                return base.Description;
            }
        }
        #endregion
    }
}
