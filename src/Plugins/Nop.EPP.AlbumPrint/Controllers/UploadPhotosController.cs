using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.EPP.AlbumPrint.Domain;
using Nop.EPP.AlbumPrint.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
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

        private readonly IEppApService _eppApService;
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
        private readonly IWebHelper _webHelper;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IRepository<ProductViewTrackerRecord> _productViewTrackerRecordRepository;
        private readonly IEmailAccountService _emailAccountService;
        private readonly EmailAccountSettings _emailAccountSettings;
        private readonly ICustomerService _customerService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Ctor 

        public UploadPhotosController(
            IEppApService eppApService,
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
            ICategoryService categoryService,
            IWebHelper webHelper,
            IProductAttributeParser productAttributeParser,
            IRepository<ProductViewTrackerRecord> productViewTrackerRecordRepository,
            IEmailAccountService emailAccountService,
            EmailAccountSettings emailAccountSettings,
            ICustomerService customerService,
            IQueuedEmailService queuedEmailService,
            IVendorService vendorService
            )
        {
            _eppApService = eppApService;
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
            _webHelper = webHelper;
            _productAttributeParser = productAttributeParser;
            _productViewTrackerRecordRepository = productViewTrackerRecordRepository;
            _emailAccountService = emailAccountService;
            _emailAccountSettings = emailAccountSettings;
            _customerService = customerService;
            _queuedEmailService = queuedEmailService;
            _vendorService = vendorService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Index(int productId, int updatecartitemid = 0)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                (!product.Published && !_catalogSettings.AllowViewUnpublishedProductPage) ||
                //ACL (access control list) 
                !await _aclService.AuthorizeAsync(product) ||
                //Store mapping
                !await _storeMappingService.AuthorizeAsync(product) ||
                //availability dates
                !_productService.ProductIsAvailable(product);
            //Check whether the current user has a "Manage products" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            var hasAdminAccess = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) && await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts);
            if (notAvailable && !hasAdminAccess)
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = await _productService.GetProductByIdAsync(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("Homepage");

                return RedirectToRoutePermanent("Product", new { SeName = await _urlRecordService.GetSeNameAsync(parentGroupedProduct) });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                var currentStore = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(currentCustomer, storeId: currentStore.Id);
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = await _urlRecordService.GetSeNameAsync(product) });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = await _urlRecordService.GetSeNameAsync(product) });
                }
            }

            //save as recently viewed
            await _recentlyViewedProductsService.AddProductToRecentlyViewedListAsync(product.Id);

            //display "edit" (manage) link
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel) &&
                await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageProducts))
            {
                //a vendor should have access only to his products
                if (currentVendor == null || currentVendor.Id == product.VendorId)
                {
                    DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = AreaNames.Admin }));
                }
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("PublicStore.ViewProduct",
                 string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.ViewProduct"), product.Name), product);

            //model
            var model = await _productModelFactory.PrepareProductDetailsModelAsync(product, updatecartitem, false);

            foreach (var prodAttr in model.ProductAttributes)
            {
                foreach (var attrVal in prodAttr.Values)
                {
                    var attrValFromDb = await _productAttributeService.GetProductAttributeValueByIdAsync(attrVal.Id);
                    if (attrValFromDb != null)
                    {
                        if (attrValFromDb.AssociatedProductId > 0)
                        {
                            var associatedProduct = await _productService.GetProductByIdAsync(attrValFromDb.AssociatedProductId);
                            if (associatedProduct != null)
                            {
                                attrVal.CustomProperties.Add("DisplayOrder", attrValFromDb.DisplayOrder);
                                attrVal.CustomProperties.Add("AssociatedProduct", associatedProduct);

                                var attrProdModel = await _productModelFactory.PrepareProductDetailsModelAsync(associatedProduct, updatecartitem, false);
                                attrVal.CustomProperties.Add("AssociatedProductModel", attrProdModel);

                                var attrProductUrl = Url.RouteUrl("Product", new { SeName = await _urlRecordService.GetSeNameAsync(associatedProduct) }, _webHelper.GetCurrentRequestProtocol());
                                attrVal.CustomProperties.Add("AssociatedProductUrl", attrProductUrl);

                                var productCategories = await _categoryService.GetProductCategoriesByProductIdAsync(associatedProduct.Id);
                                foreach (var cat in productCategories)
                                {
                                    var productCategory = cat; //productCategories.FirstOrDefault();
                                    if (productCategory != null)
                                    {
                                        var category = await _categoryService.GetCategoryByIdAsync(productCategory.CategoryId);
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

        //add product to cart using AJAX
        //currently we use this method on the product details pages
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> AddAlbumProductToCart_Details(int productId, int shoppingCartTypeId, IFormCollection form)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Homepage")
                });
            }

            //we can add only simple products
            if (product.ProductType != ProductType.SimpleProduct)
            {
                return Json(new
                {
                    success = false,
                    message = "Only simple products could be added to the cart"
                });
            }

            var photoUploadId = form["photoUploadId"].FirstOrDefault();


            //update existing shopping cart item
            var updatecartitemid = 0;
            foreach (var formKey in form.Keys)
                if (formKey.Equals($"addtocart_{productId}.UpdatedShoppingCartItemId", StringComparison.InvariantCultureIgnoreCase))
                {
                    int.TryParse(form[formKey], out updatecartitemid);
                    break;
                }

            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                var currentStore = await _storeContext.GetCurrentStoreAsync();
                //search with the same cart type as specified
                var cart = await _shoppingCartService.GetShoppingCartAsync(currentCustomer, (ShoppingCartType)shoppingCartTypeId, currentStore.Id);

                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found? let's ignore it. in this case we'll add a new item
                //if (updatecartitem == null)
                //{
                //    return Json(new
                //    {
                //        success = false,
                //        message = "No shopping cart item found to update"
                //    });
                //}
                //is it this product?
                if (updatecartitem != null && product.Id != updatecartitem.ProductId)
                {
                    return Json(new
                    {
                        success = false,
                        message = "This product does not match a passed shopping cart item identifier"
                    });
                }
            }

            var addToCartWarnings = new List<string>();

            //customer entered price
            var customerEnteredPriceConverted = await _productAttributeParser.ParseCustomerEnteredPriceAsync(product, form);

            //entered quantity
            var quantity = _productAttributeParser.ParseEnteredQuantity(product, form);

            //product and gift card attributes
            var attributes = await _productAttributeParser.ParseProductAttributesAsync(product, form, addToCartWarnings);

            //rental attributes
            _productAttributeParser.ParseRentalDates(product, form, out var rentalStartDate, out var rentalEndDate);

            var cartType = updatecartitem == null ? (ShoppingCartType)shoppingCartTypeId :
                //if the item to update is found, then we ignore the specified "shoppingCartTypeId" parameter
                updatecartitem.ShoppingCartType;

            await SaveItem(updatecartitem, addToCartWarnings, product, cartType, attributes, customerEnteredPriceConverted, rentalStartDate, rentalEndDate, quantity);

            //return result
            var shopingCart = await GetProductToCartDetails(addToCartWarnings, cartType, product, photoUploadId);
            return shopingCart;
        }


        private IActionResult InvokeHttp404()
        {
            Response.StatusCode = 404;
            return new EmptyResult();
        }

        #endregion

        protected async virtual Task SaveItem(ShoppingCartItem updatecartitem, List<string> addToCartWarnings, Product product,
           ShoppingCartType cartType, string attributes, decimal customerEnteredPriceConverted, DateTime? rentalStartDate,
           DateTime? rentalEndDate, int quantity)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            if (updatecartitem == null)
            {
                //add to the cart
                addToCartWarnings.AddRange(await _shoppingCartService.AddToCartAsync(customer,
                    product, cartType, currentStore.Id,
                    attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity, true));
            }
            else
            {
                var cart = await _shoppingCartService.GetShoppingCartAsync(customer, updatecartitem.ShoppingCartType, currentStore.Id);

                var otherCartItemWithSameParameters = await _shoppingCartService.FindShoppingCartItemInTheCartAsync(
                    cart, updatecartitem.ShoppingCartType, product, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate);
                if (otherCartItemWithSameParameters != null &&
                    otherCartItemWithSameParameters.Id == updatecartitem.Id)
                {
                    //ensure it's some other shopping cart item
                    otherCartItemWithSameParameters = null;
                }
                //update existing item
                addToCartWarnings.AddRange(await _shoppingCartService.UpdateShoppingCartItemAsync(customer,
                    updatecartitem.Id, attributes, customerEnteredPriceConverted,
                    rentalStartDate, rentalEndDate, quantity + (otherCartItemWithSameParameters?.Quantity ?? 0), true));
                if (otherCartItemWithSameParameters != null && !addToCartWarnings.Any())
                {
                    //delete the same shopping cart item (the other one)
                    await _shoppingCartService.DeleteShoppingCartItemAsync(otherCartItemWithSameParameters);
                }
            }
        }

        protected virtual async Task<IActionResult> GetProductToCartDetails(List<string> addToCartWarnings, ShoppingCartType cartType,
           Product product, string cloudFolderId)
        {
            if (addToCartWarnings.Any())
            {
                //cannot be added to the cart/wishlist
                //let's display warnings
                return Json(new
                {
                    success = false,
                    message = addToCartWarnings.ToArray()
                });
            }

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        await _customerActivityService.InsertActivityAsync("PublicStore.AddToWishlist",
                            string.Format(await _localizationService.GetResourceAsync("ActivityLog.PublicStore.AddToWishlist"), product.Name), product);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist")
                            });
                        }

                        var currentCustomer = await _workContext.GetCurrentCustomerAsync();
                        var currentStore = await _storeContext.GetCurrentStoreAsync();
                        //display notification message and update appropriate blocks
                        var shoppingCarts = (await _shoppingCartService.GetShoppingCartAsync(currentCustomer, ShoppingCartType.Wishlist, (await _storeContext.GetCurrentStoreAsync()).Id)).FirstOrDefault();
                        //var shoppingCarts = (await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, (await _storeContext.GetCurrentStoreAsync()).Id));

                        //var headerQuantity = await _localizationService.GetResourceAsync("Wishlist.HeaderQuantity");
                        var headerQuantity = await _localizationService.GetLocalizedAsync("Wishlist.HeaderQuantity");
                        var updatetopwishlistsectionhtml = string.Format(headerQuantity, shoppingCarts.Sum(item => item.Quantity));

                        return Json(new
                        {
                            success = true,
                            message = string.Format(
                                await _localizationService.GetResourceAsync("Products.ProductHasBeenAddedToTheWishlist.Link"),
                                Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml
                        });
                    }

                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart",
                            string.Format(_localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name), product);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart")
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCarts = _shoppingCartService.GetShoppingCart(_workContext.CurrentCustomer, ShoppingCartType.ShoppingCart, _storeContext.CurrentStore.Id);

                        //***Deepak
                        var shoppingCartId = shoppingCarts.Where(m => m.ProductId == product.Id).FirstOrDefault().Id;
                        EppAp eppAp = new EppAp()
                        {
                            ProductId = product.Id,
                            CustomerId = _workContext.CurrentCustomer.Id,
                            ShoppingCartId = shoppingCartId,
                            CloudFolderId = cloudFolderId,
                            AddedToShoppingCartOn = DateTime.Now,
                            CloudProvider = "Wasabi"
                        };
                        _eppApService.Insert(eppAp);

                        var vendor = _vendorService.GetVendorById(product.VendorId);
                        var customer = _customerService.GetCustomerById(eppAp.CustomerId);
                        var emailAccount = _emailAccountService.GetEmailAccountById(_emailAccountSettings.DefaultEmailAccountId);
                        if (emailAccount == null)
                            emailAccount = _emailAccountService.GetAllEmailAccounts().FirstOrDefault();
                        if (emailAccount == null)
                            throw new NopException("Email account can't be loaded");
                        var email = new QueuedEmail
                        {
                            Priority = QueuedEmailPriority.High,
                            EmailAccountId = emailAccount.Id,
                            FromName = emailAccount.DisplayName,
                            From = emailAccount.Email,
                            ToName = _customerService.GetCustomerFullName(customer),
                            To = customer.Email,
                            Subject = "[EPrintPost] Link to download your photos",
                            Body = $"Hi {customer.SystemName},<br><br> Here is your link to download the files you uploaded:<br>" + _storeContext.CurrentStore.Url + $"DownloadPhotos/Download/{eppAp.CloudFolderId}<br><br>Thanks<br>EPrintPost Team",
                            CreatedOnUtc = DateTime.UtcNow,
                            DontSendBeforeDateUtc = DateTime.UtcNow,
                            Bcc = vendor.Email
                        };
                        _queuedEmailService.InsertQueuedEmail(email);

                        var updatetopcartsectionhtml = string.Format(
                            _localizationService.GetResource("ShoppingCart.HeaderQuantity"),
                            shoppingCarts.Sum(item => item.Quantity));

                        var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
                            ? RenderViewComponentToString("FlyoutShoppingCart")
                            : string.Empty;

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"),
                                Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml,
                            updateflyoutcartsectionhtml
                        });
                    }
            }
        }
    }

    public class ProductVm
    {
        public string ProductPhoto { get; set; }
        public string ProductUrl { get; set; }
    }
}
