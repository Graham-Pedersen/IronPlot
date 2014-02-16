

using System;
using System.Runtime.InteropServices;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.Project.Samples.IronRacketProject
{
    /// <summary>
    /// Represent the methods for creating projects within the solution.
    /// </summary>
    [Guid("7C65038C-1B2F-41E1-A629-BED71D161F6F")]
    public class IronRacketProjectFactory : ProjectFactory
    {
        #region Fields
        private IronRacketProjectPackage package;
        #endregion

        #region Constructors
        /// <summary>
        /// Explicit default constructor.
        /// </summary>
        /// <param name="package">Value of the project package for initialize internal package field.</param>
        public IronRacketProjectFactory(IronRacketProjectPackage package)
            : base(package)
        {
            this.package = package;
        }
        #endregion

        #region Overriden implementation
        /// <summary>
        /// Creates a new project by cloning an existing template project.
        /// </summary>
        /// <returns></returns>
        protected override ProjectNode CreateProject()
        {
            IronRacketProjectNode project = new IronRacketProjectNode(this.package);
            project.SetSite((IOleServiceProvider)((IServiceProvider)this.package).GetService(typeof(IOleServiceProvider)));
            return project;
        }
        #endregion
    }
}
