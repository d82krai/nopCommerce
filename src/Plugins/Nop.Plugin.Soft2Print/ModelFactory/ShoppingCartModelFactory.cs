using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.ShoppingCart;
using System.Collections.Generic;
using System.Linq;
using ProductAttributeKeys = Nop.Plugin.Soft2Print.Services.ProductAttributeService.AttributeKeys;

namespace Nop.Plugin.Soft2Print.ModelFactory
{
    public partial class ShoppingCartModelFactory : Web.Factories.ShoppingCartModelFactory
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly Services.ProductAttributeService _s2p_productAttributeService;
        private readonly MediaSettings _mediaSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        #endregion

        #region Ctor
        public ShoppingCartModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            IAddressModelFactory addressModelFactory,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            IProductAttributeService productAttributeService,
            Services.ProductAttributeService _s2p_productAttributeService) : base(addressSettings,captchaSettings,catalogSettings,commonSettings,customerSettings,addressModelFactory,checkoutAttributeFormatter,checkoutAttributeParser,checkoutAttributeService,countryService,currencyService,customerService,discountService,downloadService,genericAttributeService,giftCardService,httpContextAccessor,localizationService,orderProcessingService,orderTotalCalculationService,paymentService,permissionService,pictureService,priceCalculationService,priceFormatter,productAttributeFormatter,productService,shippingService,shoppingCartService,stateProvinceService,cacheManager,storeContext,taxService,urlRecordService,vendorService,webHelper,workContext,mediaSettings,orderSettings,rewardPointsSettings,shippingSettings,shoppingCartSettings,taxSettings,vendorSettings){
            this._workContext = workContext;
            this._pictureService = pictureService;
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._mediaSettings = mediaSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._s2p_productAttributeService = _s2p_productAttributeService;
        }
        #endregion

        protected override ShoppingCartModel.ShoppingCartItemModel PrepareShoppingCartItemModel(IList<ShoppingCartItem> cart, ShoppingCartItem sci, IList<Vendor> vendors)
        {
            var result = base.PrepareShoppingCartItemModel(cart, sci, vendors);
            #region Overwrite of item is editable

            var s2pAttr = this._s2p_productAttributeService.GetS2PProductAttributes(sci.AttributesXml);
            //allow editing?
            //1. setting enabled?
            //2. simple product?
            //3. has attribute or gift card?
            //3.1 is soft2print product/project
            //4. visible individually?
            result.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             sci.Product.ProductType == ProductType.SimpleProduct &&
                                             (!string.IsNullOrEmpty(result.AttributeInfo) ||
                                              sci.Product.IsGiftCard || (s2pAttr != null)
                                              //sci.AttributesXml
                                              ) &&
                                             sci.Product.VisibleIndividually;

            if (result.Picture != null)
                if (s2pAttr != null)
                    if (s2pAttr.Preview.HasValue)
                        result.Picture.ImageUrl = this._pictureService.GetPictureUrl(s2pAttr.Preview.Value, this._mediaSettings.CartThumbPictureSize);


            #endregion

            if (sci.Product != null)
                if (sci.Product.ProductAttributeMappings != null)
                    if (sci.Product.ProductAttributeMappings.Any(i => i.ProductAttribute.Name.Equals(ProductAttributeKeys.PrintImages)))
                    {
                        var newList = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
                        newList.Add(new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem()
                        {
                            Text = result.Quantity.ToString(),
                            Value = result.Quantity.ToString(),
                            Selected = true,
                            Disabled = true
                        });
                        result.AllowedQuantities = newList;
                    }

            if (!string.IsNullOrEmpty(result.AttributeInfo))
            {
                result.AttributeInfo = result.AttributeInfo.Replace(ProductAttributeKeys.ExtraPages + ": ", string.Empty);

                var productAttrs = this._productAttributeService.GetProductAttributeMappingsByProductId(result.ProductId);
                if (productAttrs != null)
                {
                    foreach (var productAttr in productAttrs)
                    {
                        if (productAttr != null)
                        {
                            if (productAttr.ProductAttribute.Name.Equals(ProductAttributeKeys.TotalPages))
                                result.AttributeInfo = result.AttributeInfo.Replace(ProductAttributeKeys.TotalPages, productAttr.TextPrompt);
                            if (productAttr.ProductAttribute.Name.Equals(ProductAttributeKeys.MainPages))
                                result.AttributeInfo = result.AttributeInfo.Replace(ProductAttributeKeys.MainPages, productAttr.TextPrompt);
                            if (productAttr.ProductAttribute.Name.Equals(ProductAttributeKeys.PrintImages))
                                result.AttributeInfo = result.AttributeInfo.Replace(ProductAttributeKeys.PrintImages, productAttr.TextPrompt);
                        }
                    }
                }
            }



            return result;
        }

        public override MiniShoppingCartModel PrepareMiniShoppingCartModel()
        {
            var result = base.PrepareMiniShoppingCartModel();

            if (result.Items != null)
                if (result.Items.Any())
                {
                    foreach (var item in result.Items)
                    {

                        var sci = this._workContext.CurrentCustomer.ShoppingCartItems.Single(i => i.Id.Equals(item.Id));
                        var s2pAttr = this._s2p_productAttributeService.GetS2PProductAttributes(sci.AttributesXml);
                        var product = this._productService.GetProductById(item.ProductId);

                        // Reset print count
                        if (product.ProductAttributeMappings != null)
                            if (product.ProductAttributeMappings.Any(i => i.ProductAttribute.Name.Equals(ProductAttributeKeys.PrintImages)))
                            {
                                result.TotalProducts = result.TotalProducts - item.Quantity + 1;
                                item.Quantity = 1;
                            }
                        // Set S2P preview
                        if (item.Picture != null)
                            if (s2pAttr != null)
                                if (s2pAttr.Preview.HasValue)
                                    item.Picture.ImageUrl = this._pictureService.GetPictureUrl(s2pAttr.Preview.Value, this._mediaSettings.MiniCartThumbPictureSize);

                        // Re-write attributes
                        if (!string.IsNullOrEmpty(item.AttributeInfo))
                        {
                            item.AttributeInfo = item.AttributeInfo.Replace(ProductAttributeKeys.ExtraPages + ": ", string.Empty);

                            var productAttrs = this._productAttributeService.GetProductAttributeMappingsByProductId(item.ProductId);
                            if (productAttrs != null)
                            {
                                foreach (var productAttr in productAttrs)
                                {
                                    if (productAttr != null)
                                    {
                                        if (productAttr.ProductAttribute.Name.Equals(ProductAttributeKeys.TotalPages))
                                            item.AttributeInfo = item.AttributeInfo.Replace(ProductAttributeKeys.TotalPages, productAttr.TextPrompt);
                                        if (productAttr.ProductAttribute.Name.Equals(ProductAttributeKeys.MainPages))
                                            item.AttributeInfo = item.AttributeInfo.Replace(ProductAttributeKeys.MainPages, productAttr.TextPrompt);
                                        if (productAttr.ProductAttribute.Name.Equals(ProductAttributeKeys.PrintImages))
                                            item.AttributeInfo = item.AttributeInfo.Replace(ProductAttributeKeys.PrintImages, productAttr.TextPrompt);
                                    }
                                }
                            }
                        }

                    }
                }


            return result;
        }


    }
}
