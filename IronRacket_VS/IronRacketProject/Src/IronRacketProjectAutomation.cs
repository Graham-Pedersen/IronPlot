

using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Project.Automation;

namespace Microsoft.VisualStudio.Project.Samples.IronRacketProject
{
    [ComVisible(true)]
    public class OAIronRacketProject : OAProject
    {
        #region Constructors
        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="project">Custom project.</param>
        public OAIronRacketProject(IronRacketProjectNode project)
            : base(project)
        {
        }
        #endregion
    }

    [ComVisible(true)]
    [Guid("D7EDB436-6F5A-4EF4-9E3F-67C15C2FA301")]
    public class OAIronRacketProjectFileItem : OAFileItem
    {
        #region Constructors
        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="project">Automation project.</param>
        /// <param name="node">Custom file node.</param>
        public OAIronRacketProjectFileItem(OAProject project, FileNode node)
            : base(project, node)
        {
        }
        #endregion
    }
}
