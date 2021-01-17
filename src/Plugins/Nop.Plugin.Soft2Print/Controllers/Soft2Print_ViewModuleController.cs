using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DefaultRoutes = Nop.Plugin.Soft2Print.RouteProvider.DefaultRoute;


namespace Nop.Plugin.Soft2Print.Controllers
{
    public class Soft2Print_ViewModuleController : BasePluginController
    {
        #region Constants
        public const string ControllerName = "Soft2Print_ViewModule";
        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly Services.Soft2PrintAPIService _soft2PrintAPIService;
        private readonly Services.PluginSettingService _pluginSettingService;
        private readonly Services.ProductAttributeService _s2p_productAttributeService;

        private readonly IProductAttributeParser _productAttributeParser;
        #endregion

        #region Ctor

        public Soft2Print_ViewModuleController(IWorkContext workContext,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IDownloadService downloadService,
            Services.Soft2PrintAPIService soft2PrintAPIService,
            Services.PluginSettingService pluginSettingService,
            Services.ProductAttributeService s2p_productAttributeService,
             IProductAttributeParser productAttributeParser)

        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._downloadService = downloadService;
            this._soft2PrintAPIService = soft2PrintAPIService;
            this._pluginSettingService = pluginSettingService;
            this._s2p_productAttributeService = s2p_productAttributeService;
            this._productAttributeParser = productAttributeParser;
        }

        #endregion

        #region ProductAttributes
        protected string ParseProductAttributes(Product product, IFormCollection form, List<string> errors)
        {
            //product attributes
            var attributesXml = GetProductAttributesXml(product, form, errors);

            //gift cards
            AddGiftCardsAttributesXml(product, form, ref attributesXml);

            return attributesXml;
        }

        protected void ParseRentalDates(Product product, IFormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            var startControlId = $"rental_start_date_{product.Id}";
            var endControlId = $"rental_end_date_{product.Id}";
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        protected void AddGiftCardsAttributesXml(Product product, IFormCollection form, ref string attributesXml)
        {
            if (!product.IsGiftCard) return;

            var recipientName = "";
            var recipientEmail = "";
            var senderName = "";
            var senderEmail = "";
            var giftCardMessage = "";
            foreach (var formKey in form.Keys)
            {
                if (formKey.Equals($"giftcard_{product.Id}.RecipientName", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.RecipientEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    recipientEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderName", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderName = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.SenderEmail", StringComparison.InvariantCultureIgnoreCase))
                {
                    senderEmail = form[formKey];
                    continue;
                }
                if (formKey.Equals($"giftcard_{product.Id}.Message", StringComparison.InvariantCultureIgnoreCase))
                {
                    giftCardMessage = form[formKey];
                }
            }

            attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml, recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
        }

        protected string GetProductAttributesXml(Product product, IFormCollection form, List<string> errors)
        {
            var attributesXml = string.Empty;
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                var controlId = $"product_attribute_{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                {
                                    //get quantity entered by customer
                                    var quantity = 1;
                                    var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                    if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                        (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                        errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                }
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.ToString()
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                    {
                                        //get quantity entered by customer
                                        var quantity = 1;
                                        var quantityStr = form[$"product_attribute_{attribute.Id}_{item}_qty"];
                                        if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                            (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                            errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                                    }
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                //get quantity entered by customer
                                var quantity = 1;
                                var quantityStr = form[$"product_attribute_{attribute.Id}_{selectedAttributeId}_qty"];
                                if (!StringValues.IsNullOrEmpty(quantityStr) &&
                                    (!int.TryParse(quantityStr, out quantity) || quantity < 1))
                                    errors.Add(_localizationService.GetResource("ShoppingCart.QuantityShouldPositive"));

                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString(), quantity > 1 ? (int?)quantity : null);
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day));
                            }
                            catch
                            {
                            }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid.TryParse(form[controlId], out Guid downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            //validate conditional attributes (if specified)
            foreach (var attribute in productAttributes)
            {
                var conditionMet = _productAttributeParser.IsConditionMet(attribute, attributesXml);
                if (conditionMet.HasValue && !conditionMet.Value)
                {
                    attributesXml = _productAttributeParser.RemoveProductAttribute(attributesXml, attribute);
                }
            }
            return attributesXml;
        }
        #endregion


        #region Methods

        /// <summary>
        /// This is the action that is used to show the module.
        /// </summary>       
        public IActionResult ViewModule(int id)
        {
            var model = new ViewModel.ViewModule()
            {
                ViewMode = Model.ViewModule.ViewMode.View
            };
            if (TempData.ContainsKey(Model.ViewModule.ViewModuleData.TempDataPrafix + this._workContext.CurrentCustomer.Id))
            {
                var dataModel = JsonConvert.DeserializeObject<Model.ViewModule.ViewModuleData>(TempData[Model.ViewModule.ViewModuleData.TempDataPrafix + this._workContext.CurrentCustomer.Id] as string);
                if (!dataModel.ProjectID.Equals(id))
                    return RedirectToRoute(DefaultRoutes.HomePage);

                model.Url = dataModel.ModuleUrl;
            }
            else
            {
                var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                                        this._workContext.CurrentCustomer.Id,
                                                                        this._workContext.WorkingLanguage.LanguageCulture,
                                                                        this._workContext.CurrentCustomer.IsRegistered());

                if (s2pSessionID.Equals(Guid.Empty))
                    return RedirectToRoute(DefaultRoutes.HomePage);
                var moduleInstance = this._soft2PrintAPIService.OpenProject(s2pSessionID, id, Request.Headers["Referer"].ToString());
                if (moduleInstance == null)
                    return RedirectToRoute(DefaultRoutes.HomePage);
                if (moduleInstance.ProjectID == 0)
                    return RedirectToRoute(DefaultRoutes.HomePage);
                if (string.IsNullOrEmpty(moduleInstance.ModuleUrl))
                    return RedirectToRoute(DefaultRoutes.HomePage);

                model.Url = moduleInstance.ModuleUrl;
            }

            var openMode = (Model.ViewModule.OpenMode)this._pluginSettingService.ViewModule_OpenMode(this._storeContext.CurrentStore.Id).Value;
            if (openMode == Model.ViewModule.OpenMode.FullScreen)
            {
                return View(Plugin.Location + "Views/ViewModule/FullScreen.cshtml", model);
            }
            else if (openMode == Model.ViewModule.OpenMode.EaseInFullScreen)
            {
                return View(Plugin.Location + "Views/ViewModule/EaseInFullScreen.cshtml", model);
            }
            else
                return RedirectToRoute(DefaultRoutes.HomePage);

        }

        /// <summary>
        /// This is the action that is used to create a new project, but without any attributes.
        /// </summary>
        public IActionResult CreateProject_ByProductID_WithoutAttr(int id)
        {
            var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                        this._workContext.CurrentCustomer.Id,
                                                        this._workContext.WorkingLanguage.LanguageCulture,
                                                        this._workContext.CurrentCustomer.IsRegistered());

            var moduleInstance = this._soft2PrintAPIService.CreateNewProject(s2pSessionID, id, null, Request.Headers["Referer"].ToString());

            TempData[Model.ViewModule.ViewModuleData.TempDataPrafix + this._workContext.CurrentCustomer.Id] = JsonConvert.SerializeObject(moduleInstance);

            return RedirectToRoute(RouteProvider.OpenProject, new { id = moduleInstance.ProjectID });
        }

        /// <summary>
        /// This is the action that is used to create a new project, with attributes, if some is setup on the product
        /// </summary>
        [HttpPost]
        public IActionResult CreateProject_ByProductID_WithAttr(int id)
        {
            return this.CreateProject_ByProductID_WithAttr_AdnTheme(id, null);
        }

        [HttpPost]
        public IActionResult CreateProject_ByProductID_WithAttr_AdnTheme(int id, Guid? themeIdentifier)
        {
            var form = Request.Form;

            var attributeErrors = new List<string>();
            var product = this._productService.GetProductById(id);
            var productAttritures = this.ParseProductAttributes(product, form, attributeErrors);

            var s2pSessionID = this._soft2PrintAPIService.GetActiveSession(
                                                        this._workContext.CurrentCustomer.Id,
                                                        this._workContext.WorkingLanguage.LanguageCulture,
                                                        this._workContext.CurrentCustomer.IsRegistered());

            var moduleInstance = this._soft2PrintAPIService.CreateNewProject(s2pSessionID, id, themeIdentifier, Request.Headers["Referer"].ToString());

            if (string.IsNullOrEmpty(productAttritures))
                productAttritures = "<Attributes></Attributes>";

            this._s2p_productAttributeService.SaveNopAttributesToDB(moduleInstance.ProjectID, id, productAttritures);

            TempData[Model.ViewModule.ViewModuleData.TempDataPrafix + this._workContext.CurrentCustomer.Id] = JsonConvert.SerializeObject(moduleInstance);

            return RedirectToRoute(RouteProvider.OpenProject, new { id = moduleInstance.ProjectID });
        }


        #endregion
    }
}
