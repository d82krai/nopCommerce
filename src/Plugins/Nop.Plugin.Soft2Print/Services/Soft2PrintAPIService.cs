using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Soft2Print.Model;
using Nop.Plugin.Soft2Print.Model.ViewModule;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToolsRestManager = Softec.Tools.Data.REST.Manager;

namespace Nop.Plugin.Soft2Print.Services
{
    public class Soft2PrintAPIService
    {
        private class Models
        {
            public class Base
            {
                public _Error error { get; set; }

                public class _Error
                {
                    public int code { get; set; }
                    public string message { get; set; }
                }
            }


            internal class NewSession : Base
            {
                public Guid session { get; set; }
            }

            internal class Validation
            {
                public bool isValid { get; set; }
            }

            internal class PostConfirm : Base
            {
                public bool recived { get; set; }
            }

            internal class NewModuleSession : Base
            {
                public class _Data
                {
                    public Guid id { get; set; }
                    public string url { get; set; }
                    public int projectid { get; set; }
                }

                public _Data data { get; set; }
            }

            internal class ProjectInfo : Base
            {
                public class _Module
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }
                public class _Theme
                {
                    public Guid id { get; set; }
                    public string name { get; set; }
                }
                public class _ShareLock
                {
                    public bool byMe { get; set; }
                    public string by { get; set; }
                    public DateTime expectedUnlockAt { get; set; }
                }
                public class _Share
                {
                    public Guid id { get; set; }
                    public _ShareMode mode { get; set; }
                }
                public class _ShareMode
                {
                    public int id { get; set; }
                    public string name { get; set; }
                }


                public class _Data
                {
                    public int id { get; set; }
                    public string name { get; set; }
                    public DateTime created { get; set; }
                    public DateTime lastChanged { get; set; }
                    public string productCode { get; set; }
                    public _Module module { get; set; }

                    public int? inheritedBy { get; set; }

                    public bool isLocked { get; set; }
                    public _Theme theme { get; set; }
                    public string previewUrl { get; set; }
                    public _ShareLock shareLock { get; set; }
                    public IEnumerable<_Share> shares { get; set; }
                }

                public _Data data { get; set; }
            }

            internal class ProjectList : Base
            {
                public class _Data
                {
                    public IEnumerable<ProjectInfo._Data> projects { get; set; }
                }

                public _Data data { get; set; }
            }

            internal class Product : Base
            {
                public class _Data
                {
                    public int productID { get; set; }
                    public string organizationProductCode { get; set; }
                    public string name { get; set; }
                }

                public _Data data { get; set; }
            }

            internal class ProductList : Base
            {
                public class _Data
                {
                    public IEnumerable<Product._Data> products { get; set; }
                }

                public _Data data { get; set; }
            }

            internal class Theme
            {
                public string type { get; set; }
                public string themeID { get; set; }
                public string label { get; set; }
                public string preview { get; set; }

                //public string thumbnail { get; set; }
                //public string request { get; set; }

                public List<Theme> elements { get; set; }
            }

            internal class ThemeList : Base
            {
                public IEnumerable<Theme> data { get; set; }
            }

        }

        #region Util
        private ToolsRestManager _RestManager;
        private ToolsRestManager RestManager
        {
            get
            {
                if (this._RestManager == null)
                    this._RestManager = new ToolsRestManager();
                return this._RestManager;
            }
        }
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly Nop.Services.Catalog.IProductAttributeService _productAttributeService;
        private readonly Data.Repositories.IWebSessionRepository _webSessionRepository;
        private readonly Nop.Services.Catalog.IProductService _productService;
        private readonly Nop.Services.Orders.IOrderService _orderService;
        private readonly Services.ProductAttributeService _s2p_productAttributeService;
        private readonly Services.SettingService _settingService;
        private readonly ILogger _logger;
        #endregion

        #region Ctor
        public Soft2PrintAPIService(
            IWorkContext workContext,
            IStoreContext storeContext,
            Data.Repositories.IWebSessionRepository webSessionRepository,
            Nop.Services.Catalog.IProductService productService,
            Nop.Services.Catalog.IProductAttributeService productAttributeService,
            Nop.Services.Orders.IOrderService orderService,
            Services.ProductAttributeService s2p_productAttributeService,
            Services.SettingService settingService,
            ILogger logger)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._webSessionRepository = webSessionRepository;
            this._productAttributeService = productAttributeService;
            this._productService = productService;
            this._orderService = orderService;
            this._s2p_productAttributeService = s2p_productAttributeService;
            this._settingService = settingService;
            this._logger = logger;
        }
        #endregion

        #region Properties
        private string _APIUrl;
        private string APIUrl
        {

            get
            {
                if (string.IsNullOrEmpty(this._APIUrl))
                    this._APIUrl = this._settingService.APIUrl(this._storeContext.CurrentStore.Id).Value;

                return this._APIUrl;
            }
        }
        private string _AccountKey;
        private string AccountKey
        {

            get
            {
                if (string.IsNullOrEmpty(this._AccountKey))
                    this._AccountKey = this._settingService.AccountKey(this._storeContext.CurrentStore.Id).Value;

                return this._AccountKey;
            }
        }
        private string _AccountSecret;
        private string AccountSecret
        {

            get
            {
                if (string.IsNullOrEmpty(this._AccountSecret))
                    this._AccountSecret = this._settingService.AccountSecret(this._storeContext.CurrentStore.Id).Value;

                return this._AccountSecret;
            }
        }
        #endregion


        public Guid GetActiveSession(int customerID, string cultureCode, bool isAuthenticated, bool createNew = true)
        {
            var sessionID = this._webSessionRepository.GetLastSessionID(customerID);
            if (sessionID.HasValue)
                if (this.ValidateToken(sessionID.Value))
                    return sessionID.Value;
                else
                    this._webSessionRepository.MarkAsInvalid(sessionID.Value);

            if (createNew)
            {
                var result = new Models.NewSession();

                string url = this.APIUrl + string.Format(@"/Authentication/{0}/{1}/{2}/?cultureCode={3}", this.AccountKey, this.AccountSecret, customerID, cultureCode);
                if (!isAuthenticated)
                    url = this.APIUrl + string.Format(@"/Authentication/{0}/{1}/?cultureCode={3}", this.AccountKey, this.AccountSecret, customerID, cultureCode);

                result = this.RestManager.Call<Models.NewSession>(Softec.Tools.Data.REST.Method.GET, url);
                if (result.error != null)
                    LogError("There was a problem getting a new token", url, null, result);

                this._webSessionRepository.Create(result.session, customerID);

                return result.session;
            }
            else
                return Guid.Empty;
        }

        public Guid GetLastSession(int customerID)
        {
            return this._webSessionRepository.GetLastSessionID(customerID).GetValueOrDefault(Guid.Empty);
        }

        public bool ValidateToken(Guid token)
        {
            var result = this.RestManager.Call<Models.Validation>(Softec.Tools.Data.REST.Method.GET, this.APIUrl + string.Format(@"/Authentication/validate/{0}", token));
            return result.isValid;
        }
        public bool ValidateAuthInfo(string key, string secret)
        {
            var result = this.RestManager.Call<Models.Validation>(Softec.Tools.Data.REST.Method.GET, this.APIUrl + string.Format(@"/Authentication/validate/{0}/{1}", key, secret));
            return result.isValid;
        }

        public bool MigrateToUser(int fromCustomer, int toCustomer)
        {
            var lastSession = this.GetLastSession(fromCustomer);
            if (!lastSession.Equals(Guid.Empty))
            {
                string url = this.APIUrl + string.Format(@"/Authentication/Migrate/{0}/{1}/{2}/{3}", this.AccountKey, this.AccountSecret, lastSession, toCustomer);
                var result = this.RestManager.Call<Models.PostConfirm>(Softec.Tools.Data.REST.Method.POST, url);
                if (result.error != null)
                    LogError("There was a problem migrating the customer and token", url, null, result);

                return result.recived;
            }
            else
                return true;
        }

        public ViewModuleData CreateNewProject(Guid sessionID, int productID, Guid? themeID, string exitUrl)
        {
            var product = this._productService.GetProductById(productID);
            var productAttributeMappings = this._productAttributeService.GetProductAttributeMappingsByProductId(product.Id);


            JObject structures = null;

            // themeStructure
            {
                string themeStructure = string.Empty;

                var themeStrcutureMapping = productAttributeMappings.FirstOrDefault(i => i.ProductAttribute.Name.Equals(Services.ProductAttributeService.AttributeKeys.ThemeStrcuture));
                if (themeStrcutureMapping != null)
                {
                    themeStructure = themeStrcutureMapping.DefaultValue;
                }

                if (!string.IsNullOrEmpty(themeStructure))
                {
                    if (structures == null)
                        structures = new JObject();

                    structures.Add(new JProperty("theme", themeStructure));
                }
            }


            if (!themeID.HasValue)
            {
                var themeIDMapping = productAttributeMappings.FirstOrDefault(i => i.ProductAttribute.Name.Equals(Services.ProductAttributeService.AttributeKeys.ThemeIdentifier));
                if (themeIDMapping != null)
                {
                    themeID = Guid.Parse(themeIDMapping.DefaultValue);
                }

                // Here we should figure out if the product have a pre defined theme
            }

            var bodyParam = new JObject(
            new JProperty("exitUrl", exitUrl)
        );



            if (themeID.HasValue)
                bodyParam.Add(new JProperty("themeID", themeID.Value));
            if (structures != null)
                bodyParam.Add(new JProperty("structures", structures));

            var url = this.APIUrl + string.Format(@"/{0}/Project/New/{1}", sessionID, product.Sku);
            var moduleSession = this.RestManager.Call<Models.NewModuleSession>(Softec.Tools.Data.REST.Method.POST, url, new Softec.Tools.Data.REST.Parameter(bodyParam));

            if (moduleSession.error != null)
                LogError("It was not possible to create a new module instance !", url, bodyParam, moduleSession);

            return new ViewModuleData(moduleSession.data.url, moduleSession.data.projectid);
        }
        public ViewModuleData OpenProject(Guid sessionID, int projectID, string exitUrl)
        {
            var bodyParam = new JObject(
                 new JProperty("exitUrl", exitUrl)
            );

            var url = this.APIUrl + string.Format(@"/{0}/Project/Open/{1}", sessionID, projectID);
            var moduleSession = this.RestManager.Call<Models.NewModuleSession>(Softec.Tools.Data.REST.Method.POST, url, new Softec.Tools.Data.REST.Parameter(bodyParam));

            if (moduleSession.error != null)
                LogError("It was not possible to create a new module instance !", url, bodyParam, moduleSession);

            return new ViewModuleData(moduleSession.data.url, moduleSession.data.projectid);
        }

        public ProjectInfo GetProjectInfo(Guid sessionID, int projectID)
        {
            var url = this.APIUrl + string.Format(@"/{0}/Project/{1}", sessionID, projectID);
            var projectInfo = this.RestManager.Call<Models.ProjectInfo>(Softec.Tools.Data.REST.Method.GET, url);

            if (projectInfo.error != null)
                LogError("It was not possible to get project data !", url, null, projectInfo);

            return ConvertToProjectModel(projectInfo.data);
        }
        public ProjectInfo GetProjectInfoByJobID(Guid sessionID, int JobId)
        {
            var url = this.APIUrl + string.Format(@"/{0}/Project/ByJob/{1}", sessionID, JobId);
            var projectInfo = this.RestManager.Call<Models.ProjectInfo>(Softec.Tools.Data.REST.Method.GET, url);

            if (projectInfo.error != null)
                LogError("It was not possible to get project data !", url, null, projectInfo);

            return ConvertToProjectModel(projectInfo.data);
        }
        public ProjectInfo CopyProject(Guid sessionID, int projectID)
        {
            var url = this.APIUrl + string.Format(@"/{0}/Project/Copy/{1}", sessionID, projectID);
            var projectInfo = this.RestManager.Call<Models.ProjectInfo>(Softec.Tools.Data.REST.Method.POST, url);

            if (projectInfo.error != null)
                LogError("It was not possible to get project data !", url, null, projectInfo);

            return ConvertToProjectModel(projectInfo.data);
        }

        public IEnumerable<ProjectInfo> GetProjectList(Guid sessionID, int? productID = null)
        {
            var url = this.APIUrl + string.Format(@"/{0}/Project/List", sessionID);
            if (productID.HasValue)
            {
                var product = this._productService.GetProductById(productID.Value);

                url = this.APIUrl + string.Format(@"/{0}/Project/List/{1}", sessionID, product.Sku);
            }

            var projectList = this.RestManager.Call<Models.ProjectList>(Softec.Tools.Data.REST.Method.GET, url);

            if (projectList.error != null)
                LogError("It was not possible to get the project list !", url, null, projectList);

            if (projectList.data != null)
                if (projectList.data.projects != null)
                    if (projectList.data.projects.Any())
                        return projectList.data.projects.Select(i => this.ConvertToProjectModel(i));

            return new ProjectInfo[] { };
        }
        public IEnumerable<ProductInfo> GetProductList()
        {
            string url = this.APIUrl + string.Format(@"/{0}/{1}/Inventory/Product/List", this.AccountKey, this.AccountSecret);

            var productList = this.RestManager.Call<Models.ProductList>(Softec.Tools.Data.REST.Method.GET, url);

            if (productList.error != null)
                LogError("It was not possible to get the product list !", url, null, productList);

            if (productList.data != null)
                if (productList.data.products != null)
                    if (productList.data.products.Any())
                        return productList.data.products.Select(i => new ProductInfo() { S2P_ProductID = i.productID, SKU = i.organizationProductCode, Name = i.name });

            return new ProductInfo[] { };
        }

        public IEnumerable<ThemeInfo> GetThemeList(Guid sessionID, int productID, string structure)
        {
            var product = this._productService.GetProductById(productID);

            var themes = new List<ThemeInfo>();

            Action<Models.Theme> AddToThemeList = null;
            AddToThemeList = (theme) =>
            {
                if (theme.type.ToLower().Equals("category"))
                {
                    if (theme.elements != null)
                        foreach (var subTheme in theme.elements)
                            AddToThemeList(subTheme);
                }
                else // Theme
                {
                    var themeID = Guid.Parse(theme.themeID);
                    if (!themes.Any(i => i.Id.Equals(themeID)))
                    {
                        themes.Add(new ThemeInfo()
                        {
                            Id = Guid.Parse(theme.themeID),
                            Label = theme.label,
                            Preview = theme.preview
                        });
                    }
                }
            };

            var url = this.APIUrl + string.Format(@"/{0}/Library/Theme/GetStructurefull/{1}/{2}", sessionID, product.Sku, structure);

            var themeList = this.RestManager.Call<Models.ThemeList>(Softec.Tools.Data.REST.Method.GET, url);

            if (themeList.error != null)
                LogError("It was not possible to get the theme list !", url, null, themeList);

            if (themeList.data != null)
            {
                foreach (var theme in themeList.data)
                    AddToThemeList(theme);

                return themes;
            }

            return new ThemeInfo[] { };
        }

        private ProjectInfo ConvertToProjectModel(Models.ProjectInfo._Data projectInfo)
        {
            var result = new ProjectInfo()
            {
                id = projectInfo.id,
                created = projectInfo.created,
                inheritedBy = projectInfo.inheritedBy,
                isLocked = projectInfo.isLocked,
                lastChanged = projectInfo.lastChanged,
                name = projectInfo.name,
                previewUrl = projectInfo.previewUrl,
                productCode = projectInfo.productCode,
                module = new ProjectInfo._Module()
                {
                    id = projectInfo.module.id,
                    name = projectInfo.module.name
                },
                theme = null,
                shares = null,
                shareLock = null
            };
            if (projectInfo.theme != null)
            {
                result.theme = new ProjectInfo._Theme()
                {
                    id = projectInfo.theme.id,
                    name = projectInfo.name
                };
            }
            if (projectInfo.shareLock != null)
            {
                result.shareLock = new ProjectInfo._ShareLock()
                {
                    by = projectInfo.shareLock.by,
                    byMe = projectInfo.shareLock.byMe,
                    expectedUnlockAt = projectInfo.shareLock.expectedUnlockAt
                };
            }
            if (projectInfo.shares != null)
                if (projectInfo.shares.Any())
                    result.shares = projectInfo.shares.Select(i => new ProjectInfo._Share()
                    {
                        id = i.id,
                        mode = new ProjectInfo._ShareMode()
                        {
                            id = i.mode.id,
                            name = i.mode.name
                        }
                    });


            return result;
        }

        public void DeleteProject(Guid sessionID, int projectID)
        {
            var url = this.APIUrl + string.Format(@"/{0}/Project/Delete/{1}", sessionID, projectID);

            var result = this.RestManager.Call<Models.PostConfirm>(Softec.Tools.Data.REST.Method.POST, url);

            if (result.error != null)
                LogError("It was not possible to delete the project !", url, null, result);
        }
        public void RenameProject(Guid sessionID, int projectID, string name)
        {
            var url = this.APIUrl + string.Format(@"/{0}/Project/Rename/{1}/{2}", sessionID, projectID, name);

            var result = this.RestManager.Call<Models.PostConfirm>(Softec.Tools.Data.REST.Method.POST, url);

            if (result.error != null)
                LogError("It was not possible to rename the project !", url, null, result);
        }

        public void CreateOrder(Order order)
        {
            if (this.IsS2POrder(order))
            {
                var s2pJobs = new List<JObject>();
                foreach (var orderItem in order.OrderItems)
                {
                    var s2pValues = this._s2p_productAttributeService.GetS2PProductAttributes(orderItem.AttributesXml);
                    if (s2pValues != null)
                    {
                        s2pJobs.Add(new JObject(
                                new JProperty("id", s2pValues.JobID),
                                new JProperty("quantity", orderItem.Quantity),
                                new JProperty("price", orderItem.PriceExclTax * order.CurrencyRate),
                                new JProperty("useImageEnhancement", true)
                            ));
                    }
                }
                var body = new JObject(
                        new JProperty("currency", order.CustomerCurrencyCode),
                        new JProperty("jobs", new JArray(s2pJobs.ToArray())),
                        new JProperty("confirmInstantly", false)
                        );

                var key = this._settingService.AccountKey(order.StoreId);
                var secret = this._settingService.AccountSecret(order.StoreId);
                if (order.Customer.CustomerRoles.Any(i => i.SystemName.ToLower().Equals("guests")))
                {
                    // TODO: Is it needed to do some merge here?
                }

                var session = this.GetLastSession(order.CustomerId);
                var url = this._settingService.APIUrl(order.StoreId).Value + string.Format(@"/{0}/{1}/Order/New/{2}/{3}", key.Value, secret.Value, session, order.Id);

                var result = this.RestManager.Call<Models.PostConfirm>(Softec.Tools.Data.REST.Method.POST, url, new Softec.Tools.Data.REST.Parameter(body));

                if (result.error != null)
                {
                    this.LogError("For some reason we was not able to send the order to Soft2Print.", url, body, result);

                    var sb = new StringBuilder();
                    sb.AppendLine("For some reason we was not able to send the order to Soft2Print.");
                    sb.AppendLine();
                    sb.AppendLine("Url: " + url);
                    sb.AppendLine("Method: POST");
                    sb.AppendLine("Body: ");
                    sb.AppendLine(body.ToString());
                    sb.AppendLine();
                    sb.AppendLine("Result: ");
                    sb.AppendLine(JsonConvert.SerializeObject(result));

                    order.OrderNotes.Add(new OrderNote()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = order,
                        OrderId = order.Id,
                        DisplayToCustomer = false,
                        Note = sb.ToString()
                    });
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("The order is sendt to soft2print");
                    sb.AppendLine();
                    sb.AppendLine("Url: " + url);
                    sb.AppendLine("Method: POST");
                    sb.AppendLine("Body: ");
                    sb.AppendLine(body.ToString(Formatting.None));
                    sb.AppendLine();
                    sb.AppendLine("Result: ");
                    sb.AppendLine(JsonConvert.SerializeObject(result));

                    order.OrderNotes.Add(new OrderNote()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = order,
                        OrderId = order.Id,
                        DisplayToCustomer = false,
                        Note = sb.ToString()
                    });
                }
                this._orderService.UpdateOrder(order);
            }
        }
        public void ConfirmOrder(Order order)
        {
            if (this.IsS2POrder(order))
            {
                var key = this._settingService.AccountKey(order.StoreId);
                var secret = this._settingService.AccountSecret(order.StoreId);

                var url = this._settingService.APIUrl(order.StoreId).Value + string.Format(@"/{0}/{1}/Order/Confirm/{2}", key.Value, secret.Value, order.Id);

                var result = this.RestManager.Call<Models.PostConfirm>(Softec.Tools.Data.REST.Method.POST, url);

                if (result.error != null)
                {
                    this.LogError("For some reason we was not able to confirm the order to Soft2Print.", url, null, result);

                    var sb = new StringBuilder();
                    sb.AppendLine("For some reason we was not able to confirm the order to Soft2Print.");
                    sb.AppendLine();
                    sb.AppendLine("Url: " + url);
                    sb.AppendLine("Method: POST");
                    sb.AppendLine("Result: ");
                    sb.AppendLine(JsonConvert.SerializeObject(result));

                    order.OrderNotes.Add(new OrderNote()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = order,
                        OrderId = order.Id,
                        DisplayToCustomer = false,
                        Note = sb.ToString()
                    });
                }
                else
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("The order is confirmed to soft2print");
                    sb.AppendLine();
                    sb.AppendLine("Url: " + url);
                    sb.AppendLine("Method: POST");
                    sb.AppendLine("Result: ");
                    sb.AppendLine(JsonConvert.SerializeObject(result));

                    order.OrderNotes.Add(new OrderNote()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        Order = order,
                        OrderId = order.Id,
                        DisplayToCustomer = false,
                        Note = sb.ToString()
                    });
                }
                this._orderService.UpdateOrder(order);
            }
        }
        private bool IsS2POrder(Order order)
        {
            var result = false;
            if (order != null)
                if (order.OrderItems != null)
                    foreach (var orderItem in order.OrderItems)
                    {
                        if (orderItem == null)
                            continue;
                        if (string.IsNullOrEmpty(orderItem.AttributesXml))
                            continue;

                        var s2pValues = this._s2p_productAttributeService.GetS2PProductAttributes(orderItem.AttributesXml);
                        if (s2pValues != null)
                        {
                            result = true;
                            break;
                        }
                    }

            return result;
        }

        private void LogError(string message, string url, object body = null, object result = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine("Here we log the data postet and resived so that we have some values to analyse the problem:");
            sb.AppendLine();
            sb.AppendLine("Url: " + url);
            if (body != null)
            {
                sb.AppendLine("body:");
                sb.AppendLine(body.ToString());
                sb.AppendLine();
                sb.AppendLine();
            }
            if (result != null)
            {
                sb.AppendLine("Result:");
                sb.AppendLine(JsonConvert.SerializeObject(result));
            }
            this._logger.Warning(sb.ToString());
        }
    }
}
