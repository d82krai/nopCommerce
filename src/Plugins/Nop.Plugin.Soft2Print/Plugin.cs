//------------------------------------------------------------------------------
// Contributor(s): mb, New York. 
//------------------------------------------------------------------------------

using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using Nop.Services.Tasks;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Soft2Print
{
    public class Plugin : BasePlugin, IConsumer<OrderPaidEvent>, IConsumer<OrderPlacedEvent>, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Constants
        public const string Location = "~/Plugins/Soft2Print/";

        /// <summary>
        /// Use this to identify the template type
        /// </summary>
        public const string Soft2PrintProductTemplateName = "Soft2Print product";
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IScheduleTaskService _scheduleTaskService;

        private readonly Services.Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly Services.SettingService _settingService;


        #endregion

        #region Ctor
        public Plugin(
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IPluginFinder pluginFinder,
            ILogger logger,
            IProductTemplateService productTemplateService,
            IProductAttributeService productAttributeService,
            IScheduleTaskService scheduleTaskService,
            IWebHelper webHelper,
            Services.Soft2PrintAPIService soft2PrintAPIService,
            Services.PluginSettingService pluginSettingService,
            Services.SettingService settingService
            )
        {
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._settingService = settingService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._storeContext = storeContext;
            this._productTemplateService = productTemplateService;
            this._productAttributeService = productAttributeService;
            this._scheduleTaskService = scheduleTaskService;

            this._soft2PrintAPIService = soft2PrintAPIService;
            this._pluginSettingService = pluginSettingService;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            this.InstallAttributes();
            this.InstallTasks();

            return $"{_webHelper.GetStoreLocation()}Admin/{Controllers.Soft2Print_AdminController.ControllerName}/Configure";
        }

        public void HandleEvent(OrderPaidEvent eventMessage)
        {
            this._soft2PrintAPIService.ConfirmOrder(eventMessage.Order);
        }

        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            this._soft2PrintAPIService.CreateOrder(eventMessage.Order);
        }


        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (this._pluginSettingService.OpenModuleButton_ProductBox_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
                return Components.OpenModuleButton.ComponentName;
            else if (this._pluginSettingService.OpenModuleButton_ProductDetails_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
                return Components.OpenModuleButton.ComponentName;
            else if (this._pluginSettingService.ThemeList_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
                return Components.ThemeList.ComponentName;
            else
                throw new Exception("Unkown widget zone");
        }

        public IList<string> GetWidgetZones()
        {
            var list = this._pluginSettingService.OpenModuleButton_ProductBox_WidgetZone(this._storeContext.CurrentStore.Id).Value.Split(',').ToList();
            list.AddRange(this._pluginSettingService.OpenModuleButton_ProductDetails_WidgetZone(this._storeContext.CurrentStore.Id).Value.Split(','));
            list.AddRange(this._pluginSettingService.ThemeList_WidgetZone(this._storeContext.CurrentStore.Id).Value.Split(','));
            return list.Distinct().ToArray();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItems = rootNode.ChildNodes.ToList();

            menuItems.Add(
                new SiteMapNode()
                {
                    SystemName = "s2p",
                    Title = "Soft2Print",
                    IconClass = "fa-dot-circle-o",
                    Visible = true,
                    ChildNodes = new SiteMapNode[] {
                        new SiteMapNode(){
                            SystemName = "S2Pwebsite",
                            Title = "Info",
                            IconClass = "fa-dot-circle-o",
                            Visible = true,
                            OpenUrlInNewTab = true,
                            Url ="https://www.soft2print.com"
                         },
                        new SiteMapNode(){
                            SystemName = "S2Pwebsite",
                            Title = "Soft2Print backend",
                            IconClass = "fa-dot-circle-o",
                            Visible = true,
                            OpenUrlInNewTab = true,
                            Url ="https://admin.soft2print.com"
                        },
                        new SiteMapNode()
                        {
                            SystemName = "S2PHelp",
                            Title = "Help",
                            IconClass = "fa-dot-circle-o",
                            Visible = true,
                            ControllerName = Controllers.Soft2Print_AdminController.ControllerName,
                            ActionName = "Help"
                        },
                        new SiteMapNode()
                        {
                            SystemName = "S2PProductList",
                            Title = "Product list",
                            IconClass = "fa-dot-circle-o",
                            Visible = true,
                            ControllerName = Controllers.Soft2Print_AdminController.ControllerName,
                            ActionName = "ProductList"
                        },
                        new SiteMapNode(){
                            SystemName = "S2PContac",
                            Title = "Contact",
                            IconClass = "fa-dot-circle-o",
                            Visible = true,
                            OpenUrlInNewTab = true,
                            Url ="https://www.soft2print.com/contact.asp"
                        },
                     }
                });


            rootNode.ChildNodes = menuItems;


            /*
             * 
        <siteMapNode SystemName="Manufacturer templates" nopResource="Admin.System.Templates.Manufacturer" controller="Template" action="ManufacturerTemplates" IconClass="fa-genderless"/>
             */


        }


        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            #region Settings
            // API
            this._settingService.AddSettings(Services.SettingService.Keys.APIUrl, "https://api.soft2print.com/Organization");

            // Account
            this._settingService.AddSettingsNotAllStores(Services.SettingService.Keys.AccountKey, "Account name");
            this._settingService.AddSettingsNotAllStores(Services.SettingService.Keys.AccountSecret, Guid.Empty);

            #endregion

            // Add settings
            this._pluginSettingService.AddSettings(Services.PluginSettingService.Keys.OpenModuleButton.General.GetUnknownWidgetZoneWarning, true);
            this._pluginSettingService.AddSettings(Services.PluginSettingService.Keys.OpenModuleButton.ProductBox.WidgetZone, "productbox_addinfo_middle");
            this._pluginSettingService.AddSettings(Services.PluginSettingService.Keys.OpenModuleButton.ProductDetails.WidgetZone, "productdetails_inside_overview_buttons_before");
            this._pluginSettingService.AddSettings(Services.PluginSettingService.Keys.ViewModule.OpenMode, (int)Model.ViewModule.OpenMode.FullScreen);

            this._pluginSettingService.AddSettings(Services.PluginSettingService.Keys.ThemeList.GetUnknownWidgetZoneWarning, true);
            this._pluginSettingService.AddSettings(Services.PluginSettingService.Keys.ThemeList.WidgetZone, "productdetails_before_collateral");

            // Add reasources
            foreach (var defaultResource in ResourceCollection.GetDefaultValues())
                _localizationService.AddOrUpdatePluginLocaleResource(defaultResource.Key, defaultResource.Value);

            // Add Product template
            if (!this._productTemplateService.GetAllProductTemplates().Any(i => i.Name.Equals(Soft2PrintProductTemplateName)))
            {
                // User the "Simple product" template as base for our template
                var simpleProducts = this._productTemplateService.GetAllProductTemplates().First(i => i.Name.ToLower().Equals("Simple product".ToLower()));
                this._productTemplateService.InsertProductTemplate(new Core.Domain.Catalog.ProductTemplate()
                {
                    Name = Soft2PrintProductTemplateName,
                    DisplayOrder = 500,
                    ViewPath = simpleProducts.ViewPath,
                    IgnoredProductTypes = simpleProducts.IgnoredProductTypes
                });
            }

            this.InstallAttributes();
            this.InstallTasks();

            base.Install();
        }

        /// <summary>
        /// This is used to install all the tasks need,
        /// </summary>
        private void InstallTasks()
        {
            var ImageCleanUpTaskName = "Nop.Plugin.Soft2Print.ScheduledTask.ImageCleanUpTask";
            if (!this._scheduleTaskService.GetAllTasks(true).Any(i => i.Type.ToLower().Equals(ImageCleanUpTaskName.ToLower())))
            {
                this._scheduleTaskService.InsertTask(new Core.Domain.Tasks.ScheduleTask()
                {
                    Enabled = false,
                    Name = "Soft2Print Task - Cleanup images",
                    Seconds = Convert.ToInt32(new TimeSpan(15, 0, 0, 0).TotalSeconds),
                    StopOnError = false,
                    Type = ImageCleanUpTaskName
                });
            }
        }
        private void InstallAttributes()
        {
            var allProductAttributes = this._productAttributeService.GetAllProductAttributes().ToArray();
            {
                var createMissingAttributes = new System.Action<string>((attrName) =>
                {
                    if (!allProductAttributes.Any(i => i.Name.Equals(attrName)))
                    {
                        this._productAttributeService.InsertProductAttribute(new Core.Domain.Catalog.ProductAttribute()
                        {
                            Name = attrName,
                            Description = string.Empty
                        });
                    }
                });

                createMissingAttributes(Services.ProductAttributeService.AttributeKeys.TotalPages);
                createMissingAttributes(Services.ProductAttributeService.AttributeKeys.ExtraPages);
                createMissingAttributes(Services.ProductAttributeService.AttributeKeys.MainPages);
                createMissingAttributes(Services.ProductAttributeService.AttributeKeys.PrintImages);
                createMissingAttributes(Services.ProductAttributeService.AttributeKeys.ThemeIdentifier);
                createMissingAttributes(Services.ProductAttributeService.AttributeKeys.ThemeStrcuture);
                createMissingAttributes(Services.ProductAttributeService.AttributeKeys.ThemeStrcutureOnSite);

                allProductAttributes = this._productAttributeService.GetAllProductAttributes().ToArray();
                var onSiteAttr = allProductAttributes.First(i => i.Name.Equals(Services.ProductAttributeService.AttributeKeys.ThemeStrcutureOnSite));

                if (!this._productAttributeService.GetPredefinedProductAttributeValues(onSiteAttr.Id).Any())
                    this._productAttributeService.InsertPredefinedProductAttributeValue(new Core.Domain.Catalog.PredefinedProductAttributeValue()
                    {
                        IsPreSelected = true,
                        Name = "Show theme list on product page",
                        ProductAttributeId = onSiteAttr.Id
                    });
            }
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            // Remove reasources
            foreach (var defaultResource in ResourceCollection.GetDefaultValues())
                _localizationService.DeletePluginLocaleResource(defaultResource.Key);

            base.Uninstall();
        }




        #endregion


    }


}