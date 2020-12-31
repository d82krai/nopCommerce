namespace Nop.Plugin.Soft2Print.Widget.Projects.ViewModel
{
    public class Project
    {
        public int Id { get; set; }
        public string PreviewUrl { get; set; }
        public string Name { get; set; }
        public string Created { get; set; }
        public string LastChanged { get; set; }
        public string Theme { get; set; }
        public string Product { get; set; }
        public bool Locked { get; set; }

        public Project InheritedBy { get; set; }


        public bool ShowDeleteProject { get; set; }
        public bool ShowRenameProject { get; set; }
        public bool ShowCopyProject { get; set; }


        public bool ShowDetails
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Created) ||
                    !string.IsNullOrEmpty(this.LastChanged) ||
                    !string.IsNullOrEmpty(this.Theme) ||
                    !string.IsNullOrEmpty(this.Product))
                {
                    return true;
                }

                return false;
            }
        }

    }
}
