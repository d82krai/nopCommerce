using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Soft2Print.Widget.Projects
{
    public partial class RouteProvider : IRouteProvider
    {
        public const string TranslationXML = "S2P_Projects_Configure_GetTranslationXML";

        public const string ProjectList = "S2P_Projects_ProjectList";
        public const string ProjectList_Public = "S2P_Projects_PublicProjectList";

        public const string CopyProject = "S2P_Projects_Copy";
        public const string DeleteProject = "S2P_Projects_Delete";

        public const string OpenProject = Nop.Plugin.Soft2Print.RouteProvider.OpenProject;

        public int Priority => -1;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(TranslationXML, "Soft2Print/Projects/TranslationXML", new { controller = Controllers.Soft2Print_Widget_Projects_AdminController.ControllerName, action = "GetTranslationXML" });

            routeBuilder.MapRoute(ProjectList, "customer/projects", new { controller = "Soft2Print_Widget_ProjectList", action = "LoadProjectList" });
            routeBuilder.MapRoute(ProjectList_Public, "projects", new { controller = "Soft2Print_Widget_ProjectList", action = "LoadProjectList" });

            routeBuilder.MapRoute(CopyProject, "Soft2Print/project/copy/{id}", new { controller = "Soft2Print_Widget_ProjectList", action = "CopyProject" });
            routeBuilder.MapRoute(DeleteProject, "Soft2Print/project/delete/{id}", new { controller = "Soft2Print_Widget_ProjectList", action = "DeleteProject" });

        }
    }
}
