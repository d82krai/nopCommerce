using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.EPP.AlbumPrint.Domain;
using Nop.EPP.AlbumPrint.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Web.Framework.Components;
using Nop.Web.Models.Catalog;
using static Nop.Web.Models.Catalog.ProductDetailsModel;

namespace Nop.EPP.AlbumPrint.Components
{
    [ViewComponent(Name = "ProductViewTracker")]
    public class ProductViewTrackerViewComponent : NopViewComponent
    {
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IProductViewTrackerService _productViewTrackerService;
        private readonly IWorkContext _workContext;

        public ProductViewTrackerViewComponent(ICustomerService customerService,
            IProductService productService,
            IProductViewTrackerService productViewTrackerService,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _productService = productService;
            _productViewTrackerService = productViewTrackerService;
            _workContext = workContext;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (additionalData is AddToCartModel model && !Request.Path.Value.Contains("AlbumPrint"))
                return View("~/Plugins/EPP.AlbumPrint/Views/PublicInfo.cshtml", model);
            else
                return Content("");

            ////Read from the product service
            //var productById = _productService.GetProductById(model.Id);
            ////If the product exists we will log it
            //if (productById != null)
            //{
            //    //Setup the product to save
            //    var record = new ProductViewTrackerRecord
            //    {
            //        ProductId = model.Id,
            //        ProductName = productById.Name,
            //        CustomerId = _workContext.CurrentCustomer.Id,
            //        IpAddress = _workContext.CurrentCustomer.LastIpAddress,
            //        IsRegistered = _customerService.IsRegistered(_workContext.CurrentCustomer)
            //    };
            //    //Map the values we're interested in to our new entity
            //    //_productViewTrackerService.Log(record);
            //}

            //return Content("Deepak Rai");

        }
    }
}
