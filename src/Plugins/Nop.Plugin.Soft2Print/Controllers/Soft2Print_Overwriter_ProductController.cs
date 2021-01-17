using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Services.Catalog;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Factories;
using Nop.Web.Models.Catalog;
using System.Linq;

namespace Nop.Plugin.Soft2Print.Controllers
{
    /// <summary>
    /// The purpos of this overwriting controller is to:
    /// open up soft2print products in the module in a edit state when a customer click edit in the shopping cart.
    /// this gives a disadvantage of not supporting attribute changes on soft2prinbt prodducts, but if necessary this could be implimentet as a setting and then handled. 
    /// </summary>
    public class Soft2Print_Overwriter_ProductController : Web.Controllers.ProductController
    {
        #region Constants
        public const string ControllerName = "Soft2Print_Overwriter_Product";
        #endregion

        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        //private readonly IThemeContext _themeContext;
        private readonly ILogger _logger;
        //private readonly ISettingService _settingService;
        //private readonly IStoreService _storeService;
        private readonly IProductService _productService;
        //private readonly IProductTemplateService _productTemplateService;
        private readonly Services.ProductAttributeService _productAttributeService;
        private readonly Services.Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Data.Repositories.IProjectAttributeRepository _projectAttributeRepository;

        #region Ctor
        public Soft2Print_Overwriter_ProductController(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            IAclService aclService,
            ICompareProductsService compareProductsService,
            ICustomerActivityService customerActivityService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IProductModelFactory productModelFactory,
            IProductService productService,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            ShoppingCartSettings shoppingCartSettings,

            Services.ProductAttributeService productAttributeService,
            Services.Soft2PrintAPIService soft2PrintAPIService,
            Data.Repositories.IProjectAttributeRepository projectAttributeRepository,

        ILogger logger) : base(captchaSettings, catalogSettings, aclService, compareProductsService, customerActivityService, eventPublisher, localizationService, orderService, permissionService, productModelFactory, productService, recentlyViewedProductsService, storeContext, storeMappingService, urlRecordService, webHelper, workContext, workflowMessageService, localizationSettings, shoppingCartSettings)
        {
            //this._shoppingCartService = shoppingCartService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            //this._themeContext = themeContext;
            this._logger = logger;
            //this._settingService = settingService;
            //this._storeService = storeService;

            this._productAttributeService = productAttributeService;
            this._soft2PrintAPIService = soft2PrintAPIService;
            this._productService = productService;
            //this._productTemplateService = productTemplateService;

            this._projectAttributeRepository = projectAttributeRepository;

        }
        #endregion


        public override IActionResult ProductDetails(int productId, int updatecartitemid = 0)
        {
            var actionResult = base.ProductDetails(productId, updatecartitemid);

            //if (actionResult is ViewResult viewResult)
            //    viewResult.ViewName = this._soft2PrintBaseService.GetViewUrl(overwrittenControllerName, viewResult.ViewName);

            if (actionResult is ViewResult view)
                if (view.Model != null)
                    if (view.Model is ProductDetailsModel model)
                    {
                        if (model.AddToCart != null)
                        {
                            int id = model.AddToCart.UpdatedShoppingCartItemId;

                            var cartItem = this._workContext.CurrentCustomer.ShoppingCartItems.SingleOrDefault(i => i.Id.Equals(id));
                            if (cartItem != null)
                            {
                                var soft2printAttr = this._productAttributeService.GetS2PProductAttributes(cartItem.AttributesXml);
                                if (soft2printAttr != null)
                                {
                                    if (soft2printAttr.JobID.HasValue)
                                    {
                                        this._logger.Warning("#117.");
                                        var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                       this._workContext.CurrentCustomer.Id,
                                                       this._workContext.WorkingLanguage.LanguageCulture,
                                                       this._workContext.CurrentCustomer.IsRegistered());
                                        this._logger.Warning("#122.");
                                        var newProject = this._soft2PrintAPIService.CopyProject(s2pSessionID, soft2printAttr.ProjectID.Value);
                                        this._logger.Warning("#124.");
                                        var updateErrors = this._shoppingCartService.UpdateShoppingCartItem(
                                           this._workContext.CurrentCustomer,
                                           cartItem.Id,
                                           customerEnteredPrice: cartItem.CustomerEnteredPrice,
                                           quantity: cartItem.Quantity,
                                           attributesXml: this._productAttributeService.UpdateS2PProductAttributes(cartItem.AttributesXml, new Model.S2PProductAttributes()
                                           {
                                               ProjectID = newProject.id
                                           }));
                                        this._logger.Warning("#134.");
                                        this._projectAttributeRepository.Copy(soft2printAttr.ProjectID.Value, newProject.id);
                                        this._logger.Warning("#136.");
                                        if (updateErrors != null)
                                            if (updateErrors.Any())
                                                this._logger.Warning("There was a problem updateing the cart item: " + string.Join(", ", updateErrors));
                                        this._logger.Warning("#140.");
                                        return RedirectToRoute(Nop.Plugin.Soft2Print.RouteProvider.OpenProject, new { id = newProject.id });
                                    }
                                    else
                                        return RedirectToRoute(Nop.Plugin.Soft2Print.RouteProvider.OpenProject, new { id = soft2printAttr.ProjectID });
                                }
                            }
                        }
                    }
            return actionResult;
        }




    }
}
