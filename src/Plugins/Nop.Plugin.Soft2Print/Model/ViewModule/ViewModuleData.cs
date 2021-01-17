namespace Nop.Plugin.Soft2Print.Model.ViewModule
{
    public class ViewModuleData
    {
        public ViewModuleData()
        {

        }
        public ViewModuleData(string moduleUrl, int projectID)
        {
            this.ModuleUrl = moduleUrl;
            this.ProjectID = projectID;
        }

        public const string TempDataPrafix = "module";

        /// <summary>
        /// Use this to valida that you are working on the correct project
        /// </summary>
        public int ProjectID { get; set; }
        /// <summary>
        /// The Module url including everything.
        /// </summary>
        public string ModuleUrl { get; set; }
    }
}
