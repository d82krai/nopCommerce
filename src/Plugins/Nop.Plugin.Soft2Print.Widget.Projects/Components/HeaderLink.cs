using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Soft2Print.Widget.Projects.Components
{
    [ViewComponent(Name = HeaderLink.ComponentName)]
    public class HeaderLink : NopViewComponent
    {
        #region Constants
        public const string ComponentName = "ComponentS2PProjectHeaderLink";
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly ModelFactory.ProjectModelFactory _projectModelFactory;
        #endregion

        #region Ctor
        public HeaderLink(IWorkContext workContext,
            IStoreContext storeContext,
        Services.PluginSettingService pluginSettingService,
            ModelFactory.ProjectModelFactory projectModelFactory
            )
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._pluginSettingService = pluginSettingService;
            this._projectModelFactory = projectModelFactory;
        }
        #endregion

        #region Methods
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return this.Invoke(widgetZone);
        }
        private IViewComponentResult Invoke(string widgetZone)
        {
            var model = new ViewModel.HeaderLink() { Show = this._pluginSettingService.HeaderLink_Show(this._storeContext.CurrentStore.Id).Value, usePublicUrl = true };

            if (this._workContext.CurrentCustomer.IsRegistered() && this._pluginSettingService.AccountLink_Show(this._storeContext.CurrentStore.Id).Value)
                model.usePublicUrl = false;

            if (!this._workContext.CurrentCustomer.IsRegistered() && this._pluginSettingService.HeaderLink_HideIfGuest(this._storeContext.CurrentStore.Id).Value)
                model.Show = false;


            return View(Plugin.Location + "Views/HeaderLink.cshtml", model);
        }
        #endregion
    }
}
