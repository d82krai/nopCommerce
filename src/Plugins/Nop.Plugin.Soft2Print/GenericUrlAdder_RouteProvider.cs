using Microsoft.AspNetCore.Routing;
using Nop.Plugin.Soft2Print.Seo;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Soft2Print
{
    public partial class GenericUrlAdder_RouteProvider : IRouteProvider
    {
        private readonly IRouteProvider _BuildInGenericUrlRouteProvider;

        public GenericUrlAdder_RouteProvider()
        {
            this._BuildInGenericUrlRouteProvider = new Nop.Web.Infrastructure.GenericUrlRouteProvider();
        }



        public int Priority => (_BuildInGenericUrlRouteProvider.Priority + 1);

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapGenericPathRoute1("GenericUrl", "{GenericSeName}",
                new { controller = Controllers.DefaultControllers.Commen, action = "GenericUrl" });

            routeBuilder.MapLocalizedRoute("Product", "{SeName}",
               new { controller = Controllers.Soft2Print_Overwriter_ProductController.ControllerName, action = "ProductDetails" });
        }
    }
}
