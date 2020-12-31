using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Soft2Print.Services;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Soft2Print.Widget.Projects.Controllers
{
    public class Soft2Print_Widget_ProjectListController : BasePluginController
    {
        #region Constants
        public const string ControllerName = "Soft2Print_Widget_ProjectList";
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly ModelFactory.ProjectModelFactory _projectModelFactory;
        #endregion

        #region Ctor

        public Soft2Print_Widget_ProjectListController(IWorkContext workContext,
            Soft2PrintAPIService soft2PrintAPIService,
            Services.PluginSettingService pluginSettingService,
            ModelFactory.ProjectModelFactory projectModelFactory)

        {
            this._workContext = workContext;
            this._soft2PrintAPIService = soft2PrintAPIService;
            this._pluginSettingService = pluginSettingService;
            this._projectModelFactory = projectModelFactory;
        }

        #endregion


        #region Methods

        public IActionResult LoadProjectList()
        {

            var model = new ViewModel.ProjectList() { Show = false, };

            var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                            this._workContext.CurrentCustomer.Id,
                                                            this._workContext.WorkingLanguage.LanguageCulture,
                                                            _workContext.CurrentCustomer.IsRegistered(),
                                                            createNew: _workContext.CurrentCustomer.IsRegistered());

            if (!s2pSessionID.Equals(Guid.Empty))
            {
                var list = this._soft2PrintAPIService.GetProjectList(s2pSessionID);
                if (list != null)
                    if (list.Any())
                    {
                        model.Show = true;
                        model.ViewMode = (Model.ViewMode)this._pluginSettingService.ProductDetails_ViewMode().Value;

                        var projects = new List<ViewModel.Project>();

                        var alreadyAdded = new List<int>();
                        foreach (var project in list.OrderByDescending(i => i.created))
                            if (!alreadyAdded.Contains(project.id))
                                projects.Add(this._projectModelFactory.PrepareProjectModel(project, list, new Action<int>((projectId) => { alreadyAdded.Add(projectId); })));

                        model.Projects = projects;
                    }
            }

            if (_workContext.CurrentCustomer.IsRegistered() && this._pluginSettingService.AccountLink_Show().Value)
                return View(Plugin.Location + "Views/ProjectList.Dedicated.ColumnsTwo.cshtml", model);
            else
                return View(Plugin.Location + "Views/ProjectList.Dedicated.ColumnsOne.cshtml", model);
        }

        public IActionResult CopyProject(int id)
        {
            var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                           this._workContext.CurrentCustomer.Id,
                                                           this._workContext.WorkingLanguage.LanguageCulture,
                                                           _workContext.CurrentCustomer.IsRegistered(),
                                                           createNew: _workContext.CurrentCustomer.IsRegistered());

            this._soft2PrintAPIService.CopyProject(s2pSessionID, id);

            // This should reload the page
            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult DeleteProject(int id)
        {
            var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                           this._workContext.CurrentCustomer.Id,
                                                           this._workContext.WorkingLanguage.LanguageCulture,
                                                           _workContext.CurrentCustomer.IsRegistered(),
                                                           createNew: _workContext.CurrentCustomer.IsRegistered());

            this._soft2PrintAPIService.DeleteProject(s2pSessionID, id);

            // This should reload the page
            return Redirect(Request.Headers["Referer"].ToString());
        }


        #endregion
    }
}
