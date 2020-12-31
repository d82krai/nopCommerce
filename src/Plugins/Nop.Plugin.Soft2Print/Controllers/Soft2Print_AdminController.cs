using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Nop.Plugin.Soft2Print.Controllers
{
    /// <summary>
    /// The purpos of this class is simply to handel the backend configuration handling of this plugin
    /// </summary>

    public class Soft2Print_AdminController : BasePluginController
    {
        #region Constants
        public const string ControllerName = "Soft2Print_Admin";
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPluginFinder _pluginFinder;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly Services.Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Services.SettingService _settingService;
        private readonly ILogger _logger;
        #endregion

        #region Ctor
        public Soft2Print_AdminController(
            IWorkContext workContext,
            IStoreContext storeContext,
            IPluginFinder pluginFinder,
            IProductService productService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            Services.PluginSettingService pluginSettingService,
            Services.Soft2PrintAPIService soft2PrintAPIService,
Services.SettingService settingService,
            ILogger logger)

        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._pluginFinder = pluginFinder;
            this._productService = productService;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._pluginSettingService = pluginSettingService;
            this._soft2PrintAPIService = soft2PrintAPIService;
            this._settingService = settingService;
            this._logger = logger;
        }

        #endregion


        #region Methods    
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            var model = new ViewModel.Configure()
            {
                API_Url = this._settingService.APIUrl().Value,

                Account_Key = this._settingService.AccountKey(this._storeContext.CurrentStore.Id).Value,
                Account_Secret = this._settingService.AccountSecret(this._storeContext.CurrentStore.Id).Value,

                ThemeList_GetUnknownWidgetZoneWarning = this._pluginSettingService.ThemeList_GetUnknownWidgetZoneWarning().Value,
                ThemeList_WidgetZone = this._pluginSettingService.ThemeList_WidgetZone().Value,

                OpenModuleButton_General_GetUnknownWidgetZoneWarning = this._pluginSettingService.OpenModuleButton_General_GetUnknownWidgetZoneWarning().Value,

                OpenModuleButton_ProductBox_WidgetZone = this._pluginSettingService.OpenModuleButton_ProductBox_WidgetZone().Value,
                OpenModuleButton_ProductDetails_WidgetZone = this._pluginSettingService.OpenModuleButton_ProductDetails_WidgetZone().Value,

                ViewModule_OpenMode = this._pluginSettingService.ViewModule_OpenMode().Value,
                ViewModule_OpenModeValues = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(new Model.ViewModule.OpenMode[] { Model.ViewModule.OpenMode.FullScreen, Model.ViewModule.OpenMode.EaseInFullScreen }.Select(i => new { key = this._localizationService.GetLocaleStringResourceByName(ResourceCollection.Key.Admin.ViewModule.OpenModes.prefix + i.ToString()).ResourceValue, value = (int)i }), "value", "key", this._pluginSettingService.ViewModule_OpenMode().Value)
            };

            return View(Plugin.Location + "Views/Configure.cshtml", model);
        }


        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ViewModel.Configure model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            this._settingService.AddSettings(this._settingService.APIUrl().Key, model.API_Url);

            this._settingService.AddSettings(this._settingService.AccountKey().Key, model.Account_Key, this._storeContext.CurrentStore.Id);
            this._settingService.AddSettings(this._settingService.AccountSecret().Key, model.Account_Secret, this._storeContext.CurrentStore.Id);

            this._pluginSettingService.AddSettings(this._pluginSettingService.ThemeList_GetUnknownWidgetZoneWarning().Key, model.ThemeList_GetUnknownWidgetZoneWarning);
            this._pluginSettingService.AddSettings(this._pluginSettingService.ThemeList_WidgetZone().Key, model.ThemeList_WidgetZone);

            this._pluginSettingService.AddSettings(this._pluginSettingService.OpenModuleButton_General_GetUnknownWidgetZoneWarning().Key, model.OpenModuleButton_General_GetUnknownWidgetZoneWarning);

            this._pluginSettingService.AddSettings(this._pluginSettingService.OpenModuleButton_ProductBox_WidgetZone().Key, model.OpenModuleButton_ProductBox_WidgetZone);
            this._pluginSettingService.AddSettings(this._pluginSettingService.OpenModuleButton_ProductDetails_WidgetZone().Key, model.OpenModuleButton_ProductDetails_WidgetZone);

            this._pluginSettingService.AddSettings(this._pluginSettingService.ViewModule_OpenMode().Key, model.ViewModule_OpenMode);

            return Configure();
        }

        public IActionResult GetTranslationXML()
        {

            var rootElement = new XElement("Language");

            bool anyMissingTrnaslations = false;
            foreach (var item in ResourceCollection.GetDefaultValues())
            {
                var translation = this._localizationService.GetLocaleStringResourceByName(item.Key, this._workContext.WorkingLanguage.Id, false);
                if (translation == null)
                {
                    rootElement.Add(new XElement("LocaleResource",
                                    new XAttribute("Name", item.Key),
                                    new XElement("Value", item.Value)));
                    anyMissingTrnaslations = true;
                }
            }



            if (anyMissingTrnaslations)
            {
                Response.ContentType = "text/xml";
                return Content(rootElement.ToString());
            }
            else
            {
                return Content($"There is no missing language string resources for this language ({ this._workContext.WorkingLanguage.Name })");
            }
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Help()
        {
            var model = new ViewModel.Help();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Soft2Print.Widget.Projects");
            if (pluginDescriptor != null && pluginDescriptor.Installed)
            {
                model.IsProjectListPluginInstalled = true;
            }

            return View(Plugin.Location + "Views/Help.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult ProductList()
        {
            var model = new ViewModel.ProductList();

            var productList = new List<ViewModel.ProductList.ProductItem>();
            foreach (var item in this._soft2PrintAPIService.GetProductList().OrderBy(i => i.SKU))
            {
                bool isCreatedButNotPublished = false;

                var existingProduct = this._productService.GetProductBySku(item.SKU);
                if (existingProduct != null)
                {
                    if (existingProduct.Published)
                        continue;

                    isCreatedButNotPublished = true;
                }

                productList.Add(new ViewModel.ProductList.ProductItem()
                {
                    Sku = item.SKU,
                    Name = item.Name,
                    IsCreatedButNotPublished = isCreatedButNotPublished
                });
            }

            model.Products = productList;

            return View(Plugin.Location + "Views/ProductList.cshtml", model);
        }
        #endregion
    }
}
