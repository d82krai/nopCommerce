using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;
using System.Linq;

namespace Nop.Plugin.Soft2Print
{
    public partial class GenericUrlRemover_RouteProvider : IRouteProvider
    {
        private readonly IRouteProvider _BuildInGenericUrlRouteProvider;

        public GenericUrlRemover_RouteProvider()
        {
            this._BuildInGenericUrlRouteProvider = new Nop.Web.Infrastructure.GenericUrlRouteProvider();
        }



        public int Priority => (_BuildInGenericUrlRouteProvider.Priority - 1);

        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            // Remove the classic product route to be able to add a new one.
            var productDetailsRoute = routeBuilder.Routes.LastOrDefault(i =>
            {
                if (i is INamedRouter namedRoute)
                    return (namedRoute.Name.Equals("Product"));
                return false;
            });

            if (productDetailsRoute != null)
                routeBuilder.Routes.Remove(productDetailsRoute);

            var GenericUrl = routeBuilder.Routes.LastOrDefault(i =>
            {
                if (i is INamedRouter namedRoute)
                    return (namedRoute.Name.Equals("GenericUrl"));
                return false;
            });

            if (GenericUrl != null)
                routeBuilder.Routes.Remove(GenericUrl);

        }
    }
}
