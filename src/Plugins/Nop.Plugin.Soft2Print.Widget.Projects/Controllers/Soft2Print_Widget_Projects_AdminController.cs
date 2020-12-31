using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System.Linq;
using System.Xml.Linq;

namespace Nop.Plugin.Soft2Print.Widget.Projects.Controllers
{
    public class Soft2Print_Widget_Projects_AdminController : BasePluginController
    {
        #region Constants
        public const string ControllerName = "Soft2Print_Widget_Projects_Admin";
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly Services.PluginSettingService _pluginSettingService;
        #endregion

        #region Ctor
        public Soft2Print_Widget_Projects_AdminController(
            IWorkContext workContext,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            Services.PluginSettingService pluginSettingService)

        {
            this._workContext = workContext;
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._pluginSettingService = pluginSettingService;
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
                General_Action_ShowCopy = this._pluginSettingService.Genaral_Button_ShowCopy().Value,
                General_Action_ShowDelete = this._pluginSettingService.Genaral_Button_ShowDelete().Value,
                General_Action_ShowRename = this._pluginSettingService.Genaral_Button_ShowRename().Value,
                General_Details_DefaultProjectName = this._pluginSettingService.Genaral__Detials_DefaultProjectName().Value,
                General_Details_GetWarningIfS2PThemeIsShown = this._pluginSettingService.Genaral__Detials_GetWarningIfS2PThemeIsShown().Value,
                General_Details_ShowCreated = this._pluginSettingService.Genaral__Detials_ShowCreated().Value,
                General_Details_ShowInheritedBy = this._pluginSettingService.Genaral__Detials_ShowInheritedBy().Value,
                General_Details_ShowLastChanged = this._pluginSettingService.Genaral__Detials_ShowLastChanged().Value,
                General_Details_ShowPreview = this._pluginSettingService.Genaral__Detials_ShowPreview().Value,
                General_Details_ShowTheme = this._pluginSettingService.Genaral__Detials_ShowTheme().Value,
                General_Details_ShowProduct = this._pluginSettingService.Genaral__Detials_ShowProduct().Value,


                ProductDetails_Show = this._pluginSettingService.ProductDetails_Show().Value,
                ProductDetails_HideIfGuest = this._pluginSettingService.ProductDetails_HideIfGuest().Value,
                ProductDetails_WidgetZone = this._pluginSettingService.ProductDetails_WidgetZone().Value,
                ProductDetails_ViewMode = this._pluginSettingService.ProductDetails_ViewMode().Value,
                ProductDetails_ViewModeValues = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(new Model.ViewMode[] {/* Model.ViewMode.List,*/ Model.ViewMode.Grid }.Select(i => new { key = i.ToString(), value = (int)i }), "value", "key", this._pluginSettingService.ProductDetails_ViewMode().Value),

                HeaderLink_Show = this._pluginSettingService.HeaderLink_Show().Value,
                HeaderLink_HideIfGuest = this._pluginSettingService.HeaderLink_HideIfGuest().Value,
                HeaderLink_WidgetZone = this._pluginSettingService.HeaderLink_WidgetZone().Value,

                AccountLink_Show = this._pluginSettingService.AccountLink_Show().Value,
                AccountLink_WidgetZone = this._pluginSettingService.AccountLink_WidgetZone().Value

            };

            return View(Plugin.Location + "Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ViewModel.Configure model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageShippingSettings))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral_Button_ShowCopy().Key, model.General_Action_ShowCopy);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral_Button_ShowDelete().Key, model.General_Action_ShowDelete);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral_Button_ShowRename().Key, model.General_Action_ShowRename);

            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_DefaultProjectName().Key, model.General_Details_DefaultProjectName);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_GetWarningIfS2PThemeIsShown().Key, model.General_Details_GetWarningIfS2PThemeIsShown);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_ShowCreated().Key, model.General_Details_ShowCreated);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_ShowInheritedBy().Key, model.General_Details_ShowInheritedBy);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_ShowLastChanged().Key, model.General_Details_ShowLastChanged);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_ShowPreview().Key, model.General_Details_ShowPreview);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_ShowTheme().Key, model.General_Details_ShowTheme);
            this._pluginSettingService.AddSettings(this._pluginSettingService.Genaral__Detials_ShowProduct().Key, model.General_Details_ShowProduct);

            this._pluginSettingService.AddSettings(this._pluginSettingService.ProductDetails_Show().Key, model.ProductDetails_Show);
            this._pluginSettingService.AddSettings(this._pluginSettingService.ProductDetails_HideIfGuest().Key, model.ProductDetails_HideIfGuest);
            this._pluginSettingService.AddSettings(this._pluginSettingService.ProductDetails_WidgetZone().Key, model.ProductDetails_WidgetZone);
            this._pluginSettingService.AddSettings(this._pluginSettingService.ProductDetails_ViewMode().Key, model.ProductDetails_ViewMode);

            this._pluginSettingService.AddSettings(this._pluginSettingService.HeaderLink_Show().Key, model.HeaderLink_Show);
            this._pluginSettingService.AddSettings(this._pluginSettingService.HeaderLink_HideIfGuest().Key, model.HeaderLink_HideIfGuest);
            this._pluginSettingService.AddSettings(this._pluginSettingService.HeaderLink_WidgetZone().Key, model.HeaderLink_WidgetZone);

            this._pluginSettingService.AddSettings(this._pluginSettingService.AccountLink_Show().Key, model.AccountLink_Show);
            this._pluginSettingService.AddSettings(this._pluginSettingService.AccountLink_WidgetZone().Key, model.AccountLink_WidgetZone);


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


        #endregion
    }
}
