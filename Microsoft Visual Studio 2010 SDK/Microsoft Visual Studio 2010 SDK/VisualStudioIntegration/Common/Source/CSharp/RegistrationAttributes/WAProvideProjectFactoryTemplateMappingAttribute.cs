/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.VisualStudio.Shell
{
    /// <summary>
    /// This attribute is used to declare a new project system that supports Web Application Projects
    /// and define a mapping between the real project system and the 'fake' one that is defined only
    /// to store some WAP specific properties in the registry.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class WAProvideProjectFactoryTemplateMappingAttribute : RegistrationAttribute
    {
        private const string WAPFactoryGuid = "{349c5851-65df-11da-9384-00065b846f21}";
        private string _flavoredFactoryGuid;
        private Type _languageTemplateFactoryType;

        public WAProvideProjectFactoryTemplateMappingAttribute(string flavoredFactoryGuid, Type languageTemplateFactoryType)
        {
            _flavoredFactoryGuid = flavoredFactoryGuid;
            _languageTemplateFactoryType = languageTemplateFactoryType;
        }

        public string FlavoredFactoryGuid
        {
            get
            {
                return _flavoredFactoryGuid;
            }
        }

        public Type LanguageTemplateFactoryType
        {
            get
            {
                return _languageTemplateFactoryType;
            }
        }

        private string LanguageTemplatesKey
        {
            get { return string.Format(CultureInfo.InvariantCulture, "Projects\\{0}\\LanguageTemplates", WAPFactoryGuid); }
        }


        public override void Register(RegistrationContext context)
        {
            using (Key languageTemplatesKey = context.CreateKey(LanguageTemplatesKey))
            {
                languageTemplatesKey.SetValue(FlavoredFactoryGuid, LanguageTemplateFactoryType.GUID.ToString("B"));
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveKey(LanguageTemplatesKey);
        }
    }
}
