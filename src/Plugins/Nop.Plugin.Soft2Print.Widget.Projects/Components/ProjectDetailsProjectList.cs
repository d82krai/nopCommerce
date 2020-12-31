using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Soft2Print.Services;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Soft2Print.Widget.Projects.Components
{
    [ViewComponent(Name = ProjectDetailsProjectList.ComponentName)]
    public class ProjectDetailsProjectList : NopViewComponent
    {
        #region Constants
        public const string ComponentName = "ComponentS2PProjectDetailsProjectList";
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly ModelFactory.ProjectModelFactory _projectModelFactory;
        private readonly ILogger _logger;
        #endregion

        #region Ctor
        public ProjectDetailsProjectList(IWorkContext workContext,
            IStoreContext storeContext,
            IProductService productService,
            IProductTemplateService productTemplateService,
            Soft2PrintAPIService soft2PrintAPIService,
            Services.PluginSettingService pluginSettingService,
            ModelFactory.ProjectModelFactory projectModelFactory,
            ILogger logger
            )
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._productService = productService;
            this._productTemplateService = productTemplateService;
            this._pluginSettingService = pluginSettingService;
            this._soft2PrintAPIService = soft2PrintAPIService;
            this._projectModelFactory = projectModelFactory;
            this._logger = logger;
        }
        #endregion

        #region Methods
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (additionalData is int productID)
            {
                return this.Invoke(widgetZone, productID);
            }
            else if (additionalData is Nop.Web.Models.Catalog.ProductDetailsModel model)
            {
                return this.Invoke(widgetZone, model.Id);
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Soft2Print is not able to use this widget zone since the value is not an integer.");
                sb.AppendLine($"- component: '{ComponentName}'");
                sb.AppendLine($"- widget zone: '{widgetZone}'");
                sb.AppendLine($"- value: {additionalData.ToString()}");
                this._logger.Error(sb.ToString());
                return this.Content(string.Empty);
            }
        }
        private IViewComponentResult Invoke(string widgetZone, int productID)
        {
            var isAuthenticated = this._workContext.CurrentCustomer.IsRegistered();

            var product = this._productService.GetProductById(productID);
            var templateName = this._productTemplateService.GetProductTemplateById(product.ProductTemplateId).Name;

            var model = new ViewModel.ProjectList() { Show = false, };

            if (templateName.Equals(Soft2Print.Plugin.Soft2PrintProductTemplateName) && (isAuthenticated || !this._pluginSettingService.ProductDetails_HideIfGuest(this._storeContext.CurrentStore.Id).Value))
            {
                if (this._pluginSettingService.ProductDetails_Show(this._storeContext.CurrentStore.Id).Value)
                {
                    var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                                    this._workContext.CurrentCustomer.Id,
                                                                    this._workContext.WorkingLanguage.LanguageCulture,
                                                                    isAuthenticated,
                                                                    createNew: isAuthenticated);

                    if (!s2pSessionID.Equals(Guid.Empty))
                    {
                        var list = this._soft2PrintAPIService.GetProjectList(s2pSessionID, productID);
                        if (list != null)
                            if (list.Any())
                            {
                                model.Show = true;
                                model.ViewMode = (Model.ViewMode)this._pluginSettingService.ProductDetails_ViewMode(this._storeContext.CurrentStore.Id).Value;

                                var projects = new List<ViewModel.Project>();

                                var alreadyAdded = new List<int>();
                                foreach (var project in list.OrderByDescending(i => i.created).Select(i =>
                                {
                                    i.productCode = string.Empty;
                                    return i;
                                }))
                                    if (!alreadyAdded.Contains(project.id))
                                        projects.Add(this._projectModelFactory.PrepareProjectModel(project, list, new Action<int>((projectId) => { alreadyAdded.Add(projectId); })));

                                model.Projects = projects;
                            }
                    }
                }

            }

            // Hide the project list as defualt
            return View(Plugin.Location + "Views/ProjectList.ProductDetails.cshtml", model);
        }
        #endregion
    }
}
