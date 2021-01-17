using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Nop.Plugin.Soft2Print.Controllers
{

    public class Soft2Print_Externel_S2PController : BasePluginController
    {
        #region Constants
        public const string ControllerName = "Soft2Print_Externel_S2P";
        #endregion

        #region Fields

        private readonly Services.Soft2PrintAPIService _soft2PrintAPIService;

        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        private readonly Data.Repositories.IWebSessionRepository _authenticationRepository;
        private readonly Data.Repositories.IProjectAttributeRepository _projectAttributeRepository;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly ITopicModelFactory _topicModelFactory;

        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDownloadService _downloadService;
        private readonly IPictureService _pictureService;

        private readonly IPriceCalculationService _priceCalculationService;

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly ILanguageService _languageService;
        private readonly LocalizationSettings _localizationSettings;

        private readonly Services.ProductAttributeService _s2p_productAttributeService;
        #endregion

        #region Ctor

        public Soft2Print_Externel_S2PController(
            Services.Soft2PrintAPIService soft2PrintAPIService,

            IWorkContext workContext,
            IStoreContext storeContext,

            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            ILogger logger,
            ITopicModelFactory topicModelFactory,
            IShoppingCartService shoppingCartService,
            ICustomerService customerService,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,
            IDownloadService downloadService,
            IPictureService pictureService,

            IPriceCalculationService priceCalculationService,
            IGenericAttributeService genericAttributeService,
            ICurrencyService currencyService,
             ILanguageService languageService,
            LocalizationSettings localizationSettings,

            Data.Repositories.IWebSessionRepository authenticationRepository,
            Data.Repositories.IProjectAttributeRepository projectAttributeRepository,

            Services.ProductAttributeService s2p_productAttributeService
            )

        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._logger = logger;

            this._soft2PrintAPIService = soft2PrintAPIService;

            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._settingService = settingService;

            this._authenticationRepository = authenticationRepository;
            this._projectAttributeRepository = projectAttributeRepository;

            this._topicModelFactory = topicModelFactory;
            this._shoppingCartService = shoppingCartService;
            this._customerService = customerService;
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;
            this._downloadService = downloadService;
            this._pictureService = pictureService;
            this._priceCalculationService = priceCalculationService;
            this._genericAttributeService = genericAttributeService;
            this._currencyService = currencyService;
            this._languageService = languageService;
            this._localizationSettings = localizationSettings;

            this._s2p_productAttributeService = s2p_productAttributeService;
        }

        #endregion

        #region Public Methods


        public IActionResult AddToCart()
        {
            Guid id = Guid.Parse(Request.Query["id"]);
            int jobID = int.Parse(Request.Query["jobid"]);
            string productSKU = Request.Query["productcode"];

            int additionalSheetCount = 0;
            int.TryParse(Request.Query["additionalSheetCount"], out additionalSheetCount);
            int sheetCount = 0;
            int.TryParse(Request.Query["sheetCount"], out sheetCount);

            int? quantity = null;

            //this._storeContext.CurrentStore.Id
            var customer = this._customerService.GetCustomerById(this._authenticationRepository.GetCustomerID(id));
            var projectInfo = this._soft2PrintAPIService.GetProjectInfoByJobID(id, jobID);
            var product = this.GetProductBySku(productSKU, projectInfo.id);

            var projectAttributes = this._projectAttributeRepository.Get(projectInfo.id);
            if (projectAttributes == null)
                projectAttributes = new Data.Entities.S2P_ProjectAttributes() { Attributes = "<Attributes></Attributes>" };

            var existingShoppingCartItem = customer.ShoppingCartItems.FirstOrDefault(i =>
            {
                var existingAttr = this._s2p_productAttributeService.GetS2PProductAttributes(i.AttributesXml);
                if (existingAttr != null)
                    if (existingAttr.ProjectID.Equals(projectInfo.id))
                        return true;

                return false;
            });

            var attributeList = new List<string>();
            var productAttributeMappings = this._productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            var AddAtribute = AddAnAttribute(attributeList, productAttributeMappings);

            var isBook = (sheetCount > 0);
            var isPrint = (Request.Query.Any(i => i.Key.Equals("prints")));
            if (isBook)
            {
                var additionalpages = additionalSheetCount * 2;
                var totelPage = sheetCount * 2;
                var pages = (sheetCount - additionalSheetCount) * 2;
                if (additionalpages > 0)
                    AddAtribute(Services.ProductAttributeService.AttributeKeys.ExtraPages, additionalpages, true);
                AddAtribute(Services.ProductAttributeService.AttributeKeys.MainPages, pages, true);
                AddAtribute(Services.ProductAttributeService.AttributeKeys.TotalPages, totelPage, true);
            }
            else if (isPrint)
            {
                quantity = Request.Query["prints"].ToString().Split(';').Select(i => Convert.ToInt32(i.Split(':')[1])).Sum();
                AddAtribute(Services.ProductAttributeService.AttributeKeys.PrintImages, quantity, false);
            }

            // Add Other Infos
            {
                // Project id
                // Project name
                // Theme

            }

            int previewID;
            using (var webClient = new WebClient())
            {
                byte[] imageBytes = webClient.DownloadData(projectInfo.previewUrl);

                var result = this._pictureService.InsertPicture(imageBytes, MimeTypes.ImagePng, "Project_" + projectInfo.id);
                previewID = result.Id;
            }



            if (existingShoppingCartItem != null)
            {
                var oldQuantity = existingShoppingCartItem.Quantity;
                var existingAttr = this._s2p_productAttributeService.GetS2PProductAttributes(existingShoppingCartItem.AttributesXml);
                if (existingAttr != null)
                    if (existingAttr.JobID.HasValue)
                        if (!quantity.HasValue)
                            quantity += oldQuantity + 1;

                this._shoppingCartService.UpdateShoppingCartItem(
                    customer,
                    existingShoppingCartItem.Id,
                    customerEnteredPrice: 0,
                    quantity: quantity.GetValueOrDefault(oldQuantity),
                    attributesXml: this._s2p_productAttributeService.UpdateS2PProductAttributes(projectAttributes.Attributes.Replace("<Attributes>", "<Attributes>" + string.Join("", attributeList)), new Model.S2PProductAttributes()
                    {
                        JobID = jobID,
                        ProjectID = projectInfo.id,
                        Preview = previewID
                    }));
            }
            else
            {

                var errors = this._shoppingCartService.AddToCart(
                          customer,
                          product,
                          Core.Domain.Orders.ShoppingCartType.ShoppingCart,
                          this._storeContext.CurrentStore.Id,
                          attributesXml: this._s2p_productAttributeService.UpdateS2PProductAttributes(projectAttributes.Attributes.Replace("<Attributes>", "<Attributes>" + string.Join("", attributeList)), new Model.S2PProductAttributes()
                          {
                              JobID = jobID,
                              ProjectID = projectInfo.id,
                              Preview = previewID
                          }),
                          quantity: quantity.GetValueOrDefault(1));

                if (errors != null)
                    if (errors.Any())
                    {
                        var sb = new System.Text.StringBuilder();
                        sb.AppendLine("There was a problem while trying to add to cart. this is what the problem is descriped as:");
                        foreach (var item in errors)
                            sb.AppendLine(item);

                        this._logger.Warning(sb.ToString());
                    }
            }

            return RedirectToRoute("ShoppingCart");
        }

        private Action<string, object, bool> AddAnAttribute(List<string> attributeList, IList<Core.Domain.Catalog.ProductAttributeMapping> productAttributeMappings)
        {
            return new Action<string, object, bool>((key, value, isRequired) =>
            {
                if (productAttributeMappings != null)
                    foreach (var productAttributeMapping in productAttributeMappings)
                        if (productAttributeMapping.ProductAttribute.Name.ToLower().Equals(key.ToLower()))
                        {
                            if (productAttributeMapping.ProductAttributeValues != null)
                                if (productAttributeMapping.ProductAttributeValues.Any())
                                {
                                    var attributeValue = productAttributeMapping.ProductAttributeValues.First();
                                    attributeList.Add($"<ProductAttribute ID=\"{productAttributeMapping.Id}\"><ProductAttributeValue><Value>{attributeValue.Id}</Value><Quantity>{value}</Quantity></ProductAttributeValue></ProductAttribute>");
                                    break;
                                }

                            if (isRequired || productAttributeMapping.IsRequired)
                                attributeList.Add($"<ProductAttribute ID=\"{productAttributeMapping.Id}\"><ProductAttributeValue><Value>{value}</Value></ProductAttributeValue></ProductAttribute>");
                            break;
                        }
            });
        }


        [Attribute.NoCache]
        public IActionResult Prices()
        {
            switch (Request.Query["call"].ToString().ToLower())
            {
                case "getformatstrings":
                    {
                        return this.GetPriceFormat(
                                        id: Guid.Parse(Request.Query["id"])
                                        );
                    }
                case "getprice":
                    {
                        return this.GetPrice(
                                        id: Guid.Parse(Request.Query["id"]),
                                        projectID: int.Parse(Request.Query["projectId"]),
                                        productSKU: Request.Query["product"],
                                        quantity: int.Parse(Request.Query["quantity"]),
                                        additionalSheetCount: int.Parse(Request.Query["additionalSheetCount"])
                                        );
                    }
                case "getprices":
                    {
                        return this.GetPrices(
                                        id: Guid.Parse(Request.Query["id"]),
                                        projectID: int.Parse(Request.Query["projectId"]),
                                        productSKUs: Request.Query["products"].ToString().Split(';')
                                        );
                    }
                default:
                    return NotFound();
            }
        }

        private ContentResult GetPriceFormat(Guid id)
        {
            var customer = this._customerService.GetCustomerById(this._authenticationRepository.GetCustomerID(id));
            var targetCurrency = GetCurrency(customer);

            string result;
            if (!string.IsNullOrEmpty(targetCurrency.CustomFormatting))
            {
                return Content($"{{0:{targetCurrency.CustomFormatting}}}");
            }
            else
            {
                return Content("{0:0.00 " + targetCurrency.CurrencyCode + "}");
            }
        }

        private ContentResult GetPrice(Guid id, int projectID, string productSKU, int quantity, int additionalSheetCount)
        {
            return Content(string.Format("{0:0.0#}", this.GetPriceInternel(id, projectID, productSKU, quantity, additionalSheetCount)));
        }
        private ContentResult GetPrices(Guid id, int projectID, string[] productSKUs)
        {
            var prices = new Dictionary<string, decimal>();

            foreach (var productSKU in productSKUs.Distinct())
                prices.Add(productSKU, this.GetPriceInternel(id, projectID, productSKU));

            return Content(string.Join(";", prices.Select(i => i.Key + ":" + string.Format("{0:0.0#}", i.Value))));
        }

        private decimal GetPriceInternel(Guid id, int projectID, string productSKU, int quantity = 1, int additionalSheetCount = 0)
        {
            var customer = this._customerService.GetCustomerById(this._authenticationRepository.GetCustomerID(id));
            var product = this.GetProductBySku(productSKU, projectID);

            var projectAttributes = this._projectAttributeRepository.Get(projectID);
            if (projectAttributes == null)
                projectAttributes = new Data.Entities.S2P_ProjectAttributes() { Attributes = "<Attributes></Attributes>" };

            var attributeList = new List<string>();

            if (additionalSheetCount > 0)
            {
                var productAttributeMappings = this._productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
                Action<string, object, bool> AddAtribute = AddAnAttribute(attributeList, productAttributeMappings);

                AddAtribute(Services.ProductAttributeService.AttributeKeys.ExtraPages, additionalSheetCount * 2, true);
            }

            var attributeXML = projectAttributes.Attributes.Replace("<Attributes>", "<Attributes>" + string.Join("", attributeList));

            decimal discount;
            List<Nop.Services.Discounts.DiscountForCaching> discountList;

            var unitPrice = this._priceCalculationService.GetUnitPrice(product, customer, Core.Domain.Orders.ShoppingCartType.ShoppingCart, quantity, attributeXML, 0, null, null, true, out discount, out discountList);

            var targetCurrency = GetCurrency(customer);
            return _currencyService.ConvertFromPrimaryStoreCurrency(unitPrice * quantity, targetCurrency);
        }

        private Nop.Core.Domain.Catalog.Product GetProductBySku(string productSKU, int projectID)
        {
            var projectAttributes = this._projectAttributeRepository.Get(projectID);
            if (projectAttributes == null)
                return this._productService.GetProductBySku(productSKU);

            var product = this._productService.GetProductById(projectAttributes.ProductID);
            if (product != null)
                return product;

            return this._productService.GetProductBySku(productSKU);
        }

        private Currency GetCurrency(Customer customer)
        {
            //find a currency previously selected by a customer
            var customerCurrencyId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CurrencyIdAttribute, _storeContext.CurrentStore.Id);

            var allStoreCurrencies = _currencyService.GetAllCurrencies(storeId: _storeContext.CurrentStore.Id);

            //check customer currency availability
            var customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == customerCurrencyId);
            if (customerCurrency == null)
            {
                //it not found, then try to get the default currency for the current language (if specified)
                customerCurrency = allStoreCurrencies.FirstOrDefault(currency => currency.Id == this.WorkingLanguage(customer).DefaultCurrencyId);
            }

            //if the default currency for the current store not found, then try to get the first one
            if (customerCurrency == null)
                customerCurrency = allStoreCurrencies.FirstOrDefault();

            //if there are no currencies for the current store try to get the first one regardless of the store
            if (customerCurrency == null)
                customerCurrency = _currencyService.GetAllCurrencies().FirstOrDefault();

            return customerCurrency;

        }

        /// <summary>
        /// Gets or sets current user working language
        /// </summary>
        private Language WorkingLanguage(Customer customer)
        {
            //get current customer language identifier
            var customerLanguageId = this._genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.LanguageIdAttribute, _storeContext.CurrentStore.Id);

            var allStoreLanguages = _languageService.GetAllLanguages(storeId: _storeContext.CurrentStore.Id);

            //check customer language availability
            var customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == customerLanguageId);
            if (customerLanguage == null)
            {
                //it not found, then try to get the default language for the current store (if specified)
                customerLanguage = allStoreLanguages.FirstOrDefault(language => language.Id == _storeContext.CurrentStore.DefaultLanguageId);
            }

            //if the default language for the current store not found, then try to get the first one
            if (customerLanguage == null)
                customerLanguage = allStoreLanguages.FirstOrDefault();

            //if there are no languages for the current store try to get the first one regardless of the store
            if (customerLanguage == null)
                customerLanguage = _languageService.GetAllLanguages().FirstOrDefault();

            //cache the found language
            return customerLanguage;
        }

        #endregion
    }
}
