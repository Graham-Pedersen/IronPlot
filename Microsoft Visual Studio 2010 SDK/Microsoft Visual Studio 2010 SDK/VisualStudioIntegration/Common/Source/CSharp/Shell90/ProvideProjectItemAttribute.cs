//------------------------------------------------------------------------------
// <copyright file="ProvideProjectItemAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace Microsoft.VisualStudio.Shell
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Globalization;


    /// <include file='doc\ProvideProjectItemAttribute.uex' path='docs/doc[@for="ProvideProjectItemAttribute"]' />
    /// <devdoc>
    ///     This attribute associates register items to be included in the Add New Item.  
    ///     dialog for the specified project type. It is placed on a package.
    /// </devdoc>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=true)]
    public sealed class ProvideProjectItemAttribute : RegistrationAttribute
    {
        private int priority;
        private Guid factory;
        private string templateDir;
        private string itemType;

        /// <include file='doc\ProvideProjectItemAttribute.uex' path='docs/doc[@for="ProvideProjectItemAttribute.ProvideProjectItemAttribute"]' />
        /// <devdoc>
        ///     Creates a new attribute.
        /// </devdoc>
        public ProvideProjectItemAttribute(object projectFactoryType, string itemCategoryName, string templatesDir, int priority)
        {
            if (templatesDir == null || templatesDir.Length == 0)
                throw new ArgumentNullException("templatesDir");

            if (itemCategoryName == null || itemCategoryName.Length == 0)
                throw new ArgumentNullException("itemCategoryName");

            // figure out what type of object they passed in and get the GUID from it
            if (projectFactoryType is string)
                this.factory = new Guid((string)projectFactoryType);
            else if (projectFactoryType is Type)
                this.factory = ((Type)projectFactoryType).GUID;
            else if (projectFactoryType is Guid)
                this.factory = (Guid)projectFactoryType;
            else
                throw new ArgumentException(string.Format(Resources.Culture, Resources.Attributes_InvalidFactoryType, projectFactoryType));

            this.priority = priority;
            this.templateDir = templatesDir;
            this.itemType = itemCategoryName;
        }

        /// <include file='doc\ProvideProjectItemAttribute.uex' path='docs/doc[@for="ProvideProjectItemAttribute.ProjectFactoryType"]' />
        /// <devdoc>
        ///     The Project factory guid.
        /// </devdoc>
        public Guid ProjectFactoryType
        {
            get {return factory;}
        }

        /// <include file='doc\ProvideProjectItemAttribute.uex' path='docs/doc[@for="ProvideProjectItemAttribute.Priority"]' />
        /// <devdoc>
        ///     The priority of this item.
        /// </devdoc>
        public int Priority
        {
            get {return priority;}
        }

        /// <include file='doc\ProvideProjectItemAttribute.uex' path='docs/doc[@for="ProvideProjectItemAttribute.TemplateDir"]/*' />
        public string TemplateDir
        {
            get { return templateDir; }
        }

        /// <include file='doc\ProvideProjectItemAttribute.uex' path='docs/doc[@for="ProvideProjectItemAttribute.ItemType"]/*' />
        /// <summary>
        /// String describing the item type. This string is used as the folder in the
        /// left side of the "Add New Items" dialog.
        /// </summary>
        public string ItemType
        {
            get { return itemType; }
        }


        /// <summary>
        ///        The reg key name of the project.
        /// </summary>
        private string ProjectRegKeyName(RegistrationContext context) 
        {
            return string.Format(CultureInfo.InvariantCulture, "Projects\\{0}\\AddItemTemplates\\TemplateDirs\\{1}\\/1",
                                factory.ToString("B"),
                                context.ComponentType.GUID.ToString("B"));
        }

        /// <include file='doc\ProvideProjectItemAttribute.uex' path='docs/doc[@for="ProvideProjectItemAttribute.Register"]' />
        /// <devdoc>
        ///     Called to register this attribute with the given context.  The context
        ///     contains the location where the registration inforomation should be placed.
        ///     it also contains such as the type being registered, and path information.
        ///
        ///     This method is called both for registration and unregistration.  The difference is
        ///     that unregistering just uses a hive that reverses the changes applied to it.
        /// </devdoc>
        public override void Register(RegistrationContext context)
        {
            context.Log.WriteLine(string.Format(Resources.Culture, Resources.Reg_NotifyProjectItems, factory.ToString("B")));

            using (Key childKey = context.CreateKey(ProjectRegKeyName(context)))
            {
                childKey.SetValue("", itemType);

                Uri url = new Uri(context.ComponentType.Assembly.CodeBase);
                string templates = url.LocalPath;
                templates = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(templates), templateDir);
                templates = context.EscapePath(System.IO.Path.GetFullPath(templates));
                childKey.SetValue("TemplatesDir", templates);

                childKey.SetValue("SortPriority", Priority);
            }
        }

        /// <include file='doc\ProvideProjectItemAttributeProjectItem.uex' path='docs/doc[@for="ProvideProjectItemAttribute.Unregister"]/*' />
        /// <summary>
        /// Unregister this editor.
        /// </summary>
        /// <param name="context"></param>
        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(ProjectRegKeyName(context));
        }
    }
}

