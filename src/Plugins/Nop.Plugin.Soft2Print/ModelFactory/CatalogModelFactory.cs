using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Topics;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Soft2Print.ModelFactory
{
    public partial class CatalogModelFactory : Web.Factories.CatalogModelFactory
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        #endregion

        #region Ctor
        public CatalogModelFactory(BlogSettings blogSettings,
            CatalogSettings catalogSettings,
            DisplayDefaultMenuItemSettings displayDefaultMenuItemSettings,
            ForumSettings forumSettings,
            ICategoryService categoryService,
            ICategoryTemplateService categoryTemplateService,
            ICurrencyService currencyService,
            IEventPublisher eventPublisher,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IProductTagService productTagService,
            ISearchTermService searchTermService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            VendorSettings vendorSettings,



            IProductTemplateService productTemplateService) : base(blogSettings, catalogSettings, displayDefaultMenuItemSettings, forumSettings, categoryService, categoryTemplateService, currencyService, eventPublisher, httpContextAccessor,
                localizationService, manufacturerService, manufacturerTemplateService, pictureService, priceFormatter, productModelFactory, productService, productTagService, searchTermService, specificationAttributeService, cacheManager,
                storeContext, topicService, urlRecordService, vendorService, webHelper, workContext, mediaSettings, vendorSettings)
        {
            this._productService = productService;
            this._productTemplateService = productTemplateService;
        }
        #endregion

        public override CategoryModel PrepareCategoryModel(Category category, CatalogPagingFilteringModel command)
        {
            var model = base.PrepareCategoryModel(category, command);

            if (model != null)
                if (model.Products != null)
                    foreach (var modelProduct in model.Products)
                        if (modelProduct != null)
                        {
                            var product = this._productService.GetProductById(modelProduct.Id);
                            var templateName = this._productTemplateService.GetProductTemplateById(product.ProductTemplateId).Name;
                            if (templateName.Equals(Plugin.Soft2PrintProductTemplateName))
                                modelProduct.ProductPrice.DisableBuyButton = true;
                        }

            return model;
        }
    }
}
