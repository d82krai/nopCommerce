using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.EPP.AlbumPrint.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(AlbumPrintDefaults.AlbumPrintPageRouteName, "AlbumPrint/UploadPhotos",
                new { controller = "UploadPhotos", action = "Index" });

            endpointRouteBuilder.MapControllerRoute(AlbumPrintDefaults.DownloadPhotoRouteName, "AlbumPrint/DownloadPhotos/{id}",
                new { controller = "DownloadPhotos", action = "Download" });

            endpointRouteBuilder.MapControllerRoute("AddAlbumProductToCart-Details",
                "addalbumproducttocart/details/{productId:min(0)}/{shoppingCartTypeId:min(0)}",
                new { controller = "UploadPhotos", action = "AddAlbumProductToCart_Details" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
