using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Models.Common;
using System.Linq;

namespace Nop.Plugin.Soft2Print.ModelFactory
{
    public class CommonModelFactory : Nop.Web.Factories.CommonModelFactory
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Ctor
        public CommonModelFactory(BlogSettings blogSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings,
            ForumSettings forumSettings,
            IActionContextAccessor actionContextAccessor,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            ICustomerService customerService,
            IForumService forumService,
            IGenericAttributeService genericAttributeService,
            IHostingEnvironment hostingEnvironment,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            INopFileProvider fileProvider,
            IPageHeadBuilder pageHeadBuilder,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IProductTagService productTagService,
            ISitemapGenerator sitemapGenerator,
            IStaticCacheManager cacheManager,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IThemeProvider themeProvider,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            LocalizationSettings localizationSettings,
            NewsSettings newsSettings,
            StoreInformationSettings storeInformationSettings,
            VendorSettings vendorSettings) : base(blogSettings, captchaSettings, catalogSettings, commonSettings, customerSettings, displayDefaultFooterItemSettings, forumSettings, actionContextAccessor, categoryService, currencyService, customerService, forumService, genericAttributeService, hostingEnvironment, languageService, localizationService, manufacturerService, fileProvider, pageHeadBuilder, permissionService, pictureService, productService, productTagService, sitemapGenerator, cacheManager, storeContext, themeContext, themeProvider, topicService, urlHelperFactory, urlRecordService, webHelper, workContext, localizationSettings, newsSettings, storeInformationSettings, vendorSettings)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
        }
        #endregion

        public override HeaderLinksModel PrepareHeaderLinksModel()
        {
            var result = base.PrepareHeaderLinksModel();

            var customer = _workContext.CurrentCustomer;

            if (customer.HasShoppingCartItems)
            {
                result.ShoppingCartItems = customer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList()
                    .Select(i =>
                    {
                        // Reset print count
                        if (i.Product.ProductAttributeMappings != null)
                            if (i.Product.ProductAttributeMappings.Any(o => o.ProductAttribute.Name.Equals(Services.ProductAttributeService.AttributeKeys.PrintImages)))
                            {
                                i.Quantity = 1;
                            }

                        return i;
                    })
                    .ToList()
                    .Sum(i => i.Quantity);
            }
            return result;
        }

    }
}
