//------------------------------------------------------------------------------
// Contributor(s): mb, New York. 
//------------------------------------------------------------------------------

using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Plugins;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Soft2Print.Widget.Projects
{
    /// <summary>
    /// Fedex computation method
    /// </summary>
    public class Plugin : BasePlugin, IWidgetPlugin
    {
        #region Constants
        public const string Location = "~/Plugins/Soft2Print.Widget.Projects/";
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;

        private readonly Services.PluginSettingService _pluginSettingService;

        #endregion

        #region Ctor
        public Plugin(

            ILocalizationService localizationService,
            IStoreContext storeContext,
            IPluginFinder pluginFinder,
            ISettingService settingService,
            ILogger logger,
            IProductTemplateService productTemplateService,
            IWebHelper webHelper,
            Services.PluginSettingService pluginSettingService
            )
        {
            this._localizationService = localizationService;
            this._pluginFinder = pluginFinder;
            this._settingService = settingService;
            this._logger = logger;
            this._webHelper = webHelper;
            this._storeContext = storeContext;

            this._pluginSettingService = pluginSettingService;
        }


        #endregion

        #region Utilities       

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Soft2Print_Widget_Projects_Admin/Configure";
        }



        #region Widget
        public string GetWidgetViewComponentName(string widgetZone)
        {
            var viewComponentName = "WidgetsProjectProductDetails";

            if ((widgetZone.ToLower().Equals("header_links_before") || widgetZone.ToLower().Equals("header_links_after")) &&
              this._pluginSettingService.HeaderLink_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
            {
                viewComponentName = Components.HeaderLink.ComponentName;
            }
            else if (this._pluginSettingService.HeaderLink_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
            {
                // i think that this is something that should be logged.
                viewComponentName = Components.HeaderLink.ComponentName;
            }
            else if ((widgetZone.ToLower().Equals("account_navigation_before") || widgetZone.ToLower().Equals("account_navigation_after")) &&
              this._pluginSettingService.AccountLink_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
            {
                viewComponentName = Components.AccountLink.ComponentName;
            }
            else if (this._pluginSettingService.AccountLink_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
            {
                // i think that this is something that should be logged.
                viewComponentName = Components.AccountLink.ComponentName;
            }
            else if (this._pluginSettingService.ProductDetails_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
            {
                viewComponentName = Components.ProjectDetailsProjectList.ComponentName;
            }

            return viewComponentName;
        }

        public IList<string> GetWidgetZones()
        {
            var list = this._pluginSettingService.ProductDetails_WidgetZone(this._storeContext.CurrentStore.Id).Value.Split(',').ToList();
            list.AddRange(this._pluginSettingService.HeaderLink_WidgetZone(this._storeContext.CurrentStore.Id).Value.Split(','));
            list.AddRange(this._pluginSettingService.AccountLink_WidgetZone(this._storeContext.CurrentStore.Id).Value.Split(','));
            return list;
        }
        #endregion


        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            // Add settings
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Button.ShowCopy, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Button.ShowDelete, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Button.ShowRename, true);

            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Detials.ShowCreated, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Detials.ShowLastChanged, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Detials.ShowPreview, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Detials.ShowTheme, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Detials.ShowProduct, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Detials.GetWarningIfS2PThemeIsShown, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.Genaral.Detials.ShowInheritedBy, true);
            this._pluginSettingService.AddSettings<string>(Services.PluginSettingService.Keys.Genaral.Detials.DefaultProjectName, "Created: {0:dd MMM yy}");

            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.ProductDetails.Show, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.ProductDetails.HideIfGuest, false);
            this._pluginSettingService.AddSettings<string>(Services.PluginSettingService.Keys.ProductDetails.WidgetZone, "productdetails_before_collateral");
            this._pluginSettingService.AddSettings<int>(Services.PluginSettingService.Keys.ProductDetails.ViewMode, (int)Model.ViewMode.Grid);

            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.HeaderLink.Show, true);
            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.HeaderLink.HideIfGuest, false);
            this._pluginSettingService.AddSettings<string>(Services.PluginSettingService.Keys.HeaderLink.WidgetZone, "header_links_before");

            this._pluginSettingService.AddSettings<bool>(Services.PluginSettingService.Keys.AccountLink.Show, true);
            this._pluginSettingService.AddSettings<string>(Services.PluginSettingService.Keys.AccountLink.WidgetZone, "account_navigation_after");


            // Add reasources
            foreach (var defaultResource in ResourceCollection.GetDefaultValues())
                _localizationService.AddOrUpdatePluginLocaleResource(defaultResource.Key, defaultResource.Value);



            //var settings = new Soft2PrintSettings_Product_ProjectList
            //{
            //    WidgetZones = "productdetails_before_collateral",
            //    ViewMode = Base.Models.Widget_ProjectList._ViewMode.Grid,
            //    ShowDeleteProject = true,
            //    ShowRenameProject = true,
            //    ShowCopyProject = true,

            //};

            //_settingService.SaveSetting(settings);

            base.Install();
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

        #region Properties

        #endregion
    }


}