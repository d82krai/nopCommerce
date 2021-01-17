using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Nop.Plugin.Soft2Print.Widget.Projects.ModelFactory
{
    public class ProjectModelFactory
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly Services.PluginSettingService _pluginSettingService;
        #endregion

        #region Ctor
        public ProjectModelFactory(
            IWorkContext workContext,
            IStoreContext storeContext,
            ILogger logger,
            ILocalizationService localizationService,
            IProductService productService,
            Services.PluginSettingService pluginSettingService
            )
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._logger = logger;
            this._localizationService = localizationService;
            this._productService = productService;
            this._pluginSettingService = pluginSettingService;

        }
        #endregion

        #region Properties
        private Dictionary<string, string> _ThemeNameCache;
        private Dictionary<string, string> ThemeNameCache
        {
            get
            {
                if (this._ThemeNameCache == null)
                    this._ThemeNameCache = new Dictionary<string, string>();

                return this._ThemeNameCache;
            }
        }

        #endregion

        #region Methods
        public ViewModel.Project PrepareProjectModel(Soft2Print.Model.ProjectInfo project, IEnumerable<Soft2Print.Model.ProjectInfo> allProjects, System.Action<int> AddToAllreadyAdded = null)
        {
            AddToAllreadyAdded?.Invoke(project.id);

            var cultureInfo = new CultureInfo(this._workContext.WorkingLanguage.LanguageCulture);

            var showCreated = this._pluginSettingService.Genaral__Detials_ShowCreated(this._storeContext.CurrentStore.Id).Value;
            var showLastChanged = this._pluginSettingService.Genaral__Detials_ShowLastChanged(this._storeContext.CurrentStore.Id).Value;

            var showRenameProject = this._pluginSettingService.Genaral_Button_ShowRename(this._storeContext.CurrentStore.Id).Value;
            var showCopyProject = this._pluginSettingService.Genaral_Button_ShowCopy(this._storeContext.CurrentStore.Id).Value;
            var showDeleteProject = this._pluginSettingService.Genaral_Button_ShowDelete(this._storeContext.CurrentStore.Id).Value;


            var showPreview = this._pluginSettingService.Genaral__Detials_ShowPreview(this._storeContext.CurrentStore.Id).Value;
            var showProduct = this._pluginSettingService.Genaral__Detials_ShowProduct(this._storeContext.CurrentStore.Id).Value;

            var defaultProjectname = this._pluginSettingService.Genaral__Detials_DefaultProjectName(this._storeContext.CurrentStore.Id).Value;

            var showTheme = this._pluginSettingService.Genaral__Detials_ShowTheme(this._storeContext.CurrentStore.Id).Value;
            var getThemeNameWarning = this._pluginSettingService.Genaral__Detials_GetWarningIfS2PThemeIsShown(this._storeContext.CurrentStore.Id).Value;

            var projectModel = new ViewModel.Project()
            {
                Id = project.id,
                Locked = project.isLocked,

                ShowRenameProject = showRenameProject,
                ShowCopyProject = showCopyProject,
                ShowDeleteProject = showDeleteProject
            };

            if (!string.IsNullOrEmpty(project.name))
                projectModel.Name = project.name;
            else
                projectModel.Name = string.Format(defaultProjectname, project.created);

            if (showProduct)
                if (!string.IsNullOrEmpty(project.productCode))
                {
                    var product = this._productService.GetProductBySku(project.productCode);
                    projectModel.Product = product.Name;
                }

            if (showPreview)
                projectModel.PreviewUrl = project.previewUrl;
            if (showCreated)
                projectModel.Created = project.created.ToString("g", cultureInfo);
            if (showLastChanged)
                projectModel.LastChanged = project.lastChanged.ToString("g", cultureInfo);

            if (showTheme)
            {
                if (project.theme != null)
                {
                    var themeKey = ResourceCollection.Key.General.Details.Theme + "." + project.theme.id;

                    if (ThemeNameCache.ContainsKey(themeKey))
                    {
                        projectModel.Theme = ThemeNameCache[themeKey];
                    }
                    else
                    {
                        var themename = this._localizationService.GetLocaleStringResourceByName(themeKey, _workContext.WorkingLanguage.Id, false);
                        if (themename != null)
                        {
                            ThemeNameCache.Add(themeKey, themename.ResourceValue);
                            projectModel.Theme = themename.ResourceValue;
                        }
                        else
                        {
                            if (getThemeNameWarning)
                            {
                                this._logger.Warning(string.Format(@"is was not possible to find a language resource string for: '{0}'
We simply used the name from soft2print to display to the customer, but since this could be a vary teknical name we recoment to enter a translation in your own system.", themeKey));
                            }

                            if (!string.IsNullOrEmpty(project.theme.name))
                            {
                                ThemeNameCache.Add(themeKey, project.theme.name);
                                projectModel.Theme = project.theme.name;
                            }


                        }


                    }
                }
            }

            if (project.inheritedBy.HasValue && this._pluginSettingService.Genaral__Detials_ShowInheritedBy(this._storeContext.CurrentStore.Id).Value)
            {
                var inheritedBy = allProjects.FirstOrDefault(i => i.id.Equals(project.inheritedBy.Value));
                if (inheritedBy != null)
                    projectModel.InheritedBy = this.PrepareProjectModel(inheritedBy, allProjects, AddToAllreadyAdded);
            }


            projectModel.ShowRenameProject = false;

            return projectModel;
        }
        #endregion
    }
}
