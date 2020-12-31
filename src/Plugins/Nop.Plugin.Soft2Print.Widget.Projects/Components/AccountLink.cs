using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Soft2Print.Widget.Projects.Components
{
    [ViewComponent(Name = AccountLink.ComponentName)]
    public class AccountLink : NopViewComponent
    {
        #region Constants
        public const string ComponentName = "ComponentS2PProjectAccountLink";
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly ModelFactory.ProjectModelFactory _projectModelFactory;
        #endregion

        #region Ctor
        public AccountLink(IWorkContext workContext,
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
            var model = new ViewModel.AccountLink() { Show = false, Active = false };

            if (this._workContext.CurrentCustomer.IsRegistered())
                model.Show = this._pluginSettingService.AccountLink_Show(this._storeContext.CurrentStore.Id).Value;

            string projectListUrl = Url.RouteUrl(RouteProvider.ProjectList).ToLower().Trim();
            if (Request.Path.ToString().ToLower().Trim().Equals(projectListUrl))
                model.Active = true;

            return View(Plugin.Location + "Views/AccountLink.cshtml", model);
        }
        #endregion
    }
}
