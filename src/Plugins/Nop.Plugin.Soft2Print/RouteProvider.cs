using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Soft2Print
{
    public partial class RouteProvider : IRouteProvider
    {
        public class DefaultRoute
        {
            public const string HomePage = "HomePage";
        }

        #region Constants

        public const string TranslationXML = "S2P_Main_Configure_GetTranslationXML";

        public const string OpenProject = "S2P_Main_ViewModule";
        internal const string CreateProject_ByProduct_WithoutAttr = "S2P_Main_CreateProject_ByProduct_WithoutAttr";
        internal const string CreateProject_ByProduct_WithAttr = "S2P_Main_CreateProject_ByProduct_WithAttr";
        internal const string CreateProject_ByProduct_WithAttr_WithTheme = "S2P_Main_CreateProject_ByProduct_WithAttr_WithTheme";

        public const string CreateProject_Product_Box = RouteProvider.CreateProject_ByProduct_WithoutAttr;
        public const string CreateProject_Product_Details = RouteProvider.CreateProject_ByProduct_WithAttr;

        private const string AddToCart = "S2P_AddToCart";
        private const string Prices = "S2P_Prices";
        #endregion

        public int Priority => -1;

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute(TranslationXML, "Soft2Print/TranslationXML", new { controller = Controllers.Soft2Print_AdminController.ControllerName, action = "GetTranslationXML" });

            routeBuilder.MapRoute(AddToCart, "Soft2Print/Module/AddToCart", new { controller = Controllers.Soft2Print_Externel_S2PController.ControllerName, action = "AddToCart" });
            routeBuilder.MapRoute(Prices, "Soft2Print/Module/Prices", new { controller = Controllers.Soft2Print_Externel_S2PController.ControllerName, action = "Prices" });

            routeBuilder.MapRoute(CreateProject_ByProduct_WithAttr_WithTheme, "Soft2Print/NewProject/WithAttr/{id}/{themeIdentifier}", new { controller = Controllers.Soft2Print_ViewModuleController.ControllerName, action = "CreateProject_ByProductID_WithAttr_AdnTheme" });
            routeBuilder.MapRoute(CreateProject_ByProduct_WithAttr, "Soft2Print/NewProject/WithAttr/{id}", new { controller = Controllers.Soft2Print_ViewModuleController.ControllerName, action = "CreateProject_ByProductID_WithAttr" });
            routeBuilder.MapRoute(CreateProject_ByProduct_WithoutAttr, "Soft2Print/NewProject/{id}", new { controller = Controllers.Soft2Print_ViewModuleController.ControllerName, action = "CreateProject_ByProductID_WithoutAttr" });


            #region ViewModule
            routeBuilder.MapRoute(OpenProject, "ViewModule/{id}", new { controller = Controllers.Soft2Print_ViewModuleController.ControllerName, action = "ViewModule" });
            #endregion

        }
    }
}
