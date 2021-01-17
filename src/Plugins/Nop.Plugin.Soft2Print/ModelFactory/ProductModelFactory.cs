using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Shipping.Date;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Soft2Print.ModelFactory
{
    public partial class ProductModelFactory : Web.Factories.ProductModelFactory
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductAttributeService _productAttributeService;
        #endregion

        #region Ctor
        public ProductModelFactory(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CustomerSettings customerSettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IProductAttributeParser productAttributeParser,
            IProductAttributeService productAttributeService,
            IProductService productService,
            IProductTagService productTagService,
            IProductTemplateService productTemplateService,
            IReviewTypeService reviewTypeService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            SeoSettings seoSettings,
            VendorSettings vendorSettings) : base(captchaSettings, catalogSettings, customerSettings, categoryService, currencyService, customerService, dateRangeService, dateTimeHelper, downloadService, localizationService, manufacturerService, permissionService, pictureService, priceCalculationService, priceFormatter, productAttributeParser, productAttributeService, productService, productTagService, productTemplateService, reviewTypeService, specificationAttributeService, cacheManager, storeContext, taxService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, orderSettings, seoSettings, vendorSettings)
        {
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._productAttributeService = productAttributeService;
        }
        #endregion

        public override IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products, bool preparePriceModel = true, bool preparePictureModel = true, int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false, bool forceRedirectionAfterAddingToCart = false)
        {
            var result = base.PrepareProductOverviewModels(products, preparePriceModel, preparePictureModel, productThumbPictureSize, prepareSpecificationAttributes, forceRedirectionAfterAddingToCart);

            if (result != null)
                foreach (var productasd in result)
                {
                    var product = this._productService.GetProductById(productasd.Id);
                    var templateName = this._productTemplateService.GetProductTemplateById(product.ProductTemplateId).Name;
                    if (templateName.Equals(Plugin.Soft2PrintProductTemplateName))
                        if (productasd.ProductPrice != null)
                            productasd.ProductPrice.DisableBuyButton = true;
                }

            return result;
        }

        public override ProductDetailsModel PrepareProductDetailsModel(Product product, ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            var result = base.PrepareProductDetailsModel(product, updatecartitem, isAssociatedProduct);

            var templateName = this._productTemplateService.GetProductTemplateById(product.ProductTemplateId).Name;
            if (templateName.Equals(Plugin.Soft2PrintProductTemplateName))
            {
                // Is Soft2Print product
                if (result.AddToCart != null)
                    result.AddToCart.DisableBuyButton = true;

                foreach (var attr in result.ProductAttributes.ToArray())
                {
                    var attribute = _productAttributeService.GetProductAttributeById(attr.ProductAttributeId);

                    if (attribute.Name.ToLower().StartsWith(Services.ProductAttributeService.AttributeKeys.Prefix.ToLower()))
                        result.ProductAttributes.Remove(attr);
                }
            }

            return result;
        }
    }
}
