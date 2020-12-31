using System.Collections.Generic;

namespace Nop.Plugin.Soft2Print.Widget.Projects.ViewModel
{
    public class ProjectList
    {
        public ProjectList()
        {
            this.Projects = new List<Project>();
        }

        public bool Show { get; set; }
        public Model.ViewMode ViewMode { get; set; }
        public IEnumerable<Project> Projects { get; set; }
    }
}
