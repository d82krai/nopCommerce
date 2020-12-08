using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.EPP.AlbumPrint.Controllers
{
    [AutoValidateAntiforgeryToken]
    //[AuthorizeAdmin]
    //[Area(AreaNames.Admin)]
    public class UploadPhotosController : BasePluginController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICategoryService _categoryService;

        #endregion

        #region Ctor 

        public UploadPhotosController(
            IProductService productService,
            CatalogSettings catalogSettings,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            ShoppingCartSettings shoppingCartSettings,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IProductModelFactory productModelFactory,
            IProductAttributeService productAttributeService,
            ICategoryService categoryService
            )
        {
            _productService = productService;
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _urlRecordService = urlRecordService;
            _shoppingCartSettings = shoppingCartSettings;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _storeContext = storeContext;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _productModelFactory = productModelFactory;
            _productAttributeService = productAttributeService;
            _categoryService = categoryService;
        }

        #endregion

        #region Methods

        public IActionResult Index(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !_aclService.Authorize(product) ||
                //Store mapping
                !_storeMappingService.Authorize(product) ||
                //availability dates
                !_productService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("Homepage");

                return RedirectToRoutePermanent("Product", new { SeName = _urlRecordService.GetSeName(parentGroupedProduct) });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, storeId: _storeContext.CurrentStore.Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(product) });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = _urlRecordService.GetSeName(product) });
                }
            }

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) &&
                _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                //a vendor should have access only to his products
                if (_workContext.CurrentVendor == null || _workContext.CurrentVendor.Id == product.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.Admin }));
                }
            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct",
                string.Format(_localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            //model
            var model = _productModelFactory.PrepareProductDetailsModel(product, updatecartitem, false);

            foreach (var prodAttr in model.ProductAttributes)
            {
                foreach (var attrVal in prodAttr.Values)
                {
                    var attrValFromDb = _productAttributeService.GetProductAttributeValueById(attrVal.Id);
                    if (attrValFromDb != null)
                    {
                        if (attrValFromDb.AssociatedProductId > 0)
                        {
                            var associatedProduct = _productService.GetProductById(attrValFromDb.AssociatedProductId);
                            if (associatedProduct != null)
                            {
                                attrVal.CustomProperties.Add("DisplayOrder", attrValFromDb.DisplayOrder);
                                attrVal.CustomProperties.Add("AssociatedProduct", associatedProduct);

                                var productCategories = _categoryService.GetProductCategoriesByProductId(associatedProduct.Id);
                                foreach (var cat in productCategories)
                                {
                                    var productCategory = cat; //productCategories.FirstOrDefault();
                                    if (productCategory != null)
                                    {
                                        var category = _categoryService.GetCategoryById(productCategory.CategoryId);
                                        if (!attrVal.CustomProperties.ContainsKey("Category"))
                                            attrVal.CustomProperties.Add("Category", category);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return View("~/Plugins/EPP.AlbumPrint/Views/UploadPhotos.cshtml", model);
        }

        private IActionResult InvokeHttp404()
        {
            Response.StatusCode = 404;
            return new EmptyResult();
        }

        #endregion
    }
}
