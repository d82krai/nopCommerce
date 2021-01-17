using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Web.Framework.Components;
using System.Linq;
using System.Text;

namespace Nop.Plugin.Soft2Print.Components
{
    [ViewComponent(Name = OpenModuleButton.ComponentName)]
    public class OpenModuleButton : NopViewComponent
    {
        #region Constants
        public const string ComponentName = "ComponentS2POpenModuleButton";

        private string[] ConfirmedWidgetZones = {
            "productdetails_before_pictures",
            "productdetails_after_pictures",
            "productdetails_top",
            "productdetails_after_breadcrumb",
            "productdetails_overview_top",
            "productdetails_overview_bottom",
            "productdetails_inside_overview_buttons_before",
            "productdetails_inside_overview_buttons_after",
            "productdetails_before_collateral",
            "productdetails_bottom",
            "productbox_addinfo_before",
            "productbox_addinfo_middle",
            "productbox_addinfo_after"
        };

        #endregion

        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly Services.PluginSettingService _pluginSettingService;

        private readonly ILogger _logger;
        #endregion

        #region Ctor
        public OpenModuleButton(IWorkContext workContext,
            IStoreContext storeContext,
            IProductService productService,
            IProductAttributeService productAttributeService,
            IProductTemplateService productTemplateService,
            Services.PluginSettingService pluginSettingService,
            ILogger logger
            )
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._productService = productService;
            this._productAttributeService = productAttributeService;
            this._productTemplateService = productTemplateService;
            this._pluginSettingService = pluginSettingService;
            this._logger = logger;
        }
        #endregion

        #region Methods
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            if (additionalData == null)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Soft2Print is not able to use this widget zone or theme since the value is null, and this is not supported.");
                sb.AppendLine($"- ComponentName: '{ComponentName}'");
                sb.AppendLine($"- widget zone: '{widgetZone}'");
                sb.AppendLine();
                sb.AppendLine("We have hidden the widget as a work arount for this problem.");
                this._logger.Error(sb.ToString());
                return this.Content(string.Empty);
            }
            else if (ConfirmedWidgetZones.Any(i => i.ToLower().Equals(widgetZone)) && additionalData is int)
            {
                // In this case we have confirmed that the additional data is the product id,
                // We can't say with a 110% that if the webshop is a different theme that this can't change, but this would not just impack us so it is most unlikely.
                return this.Invoke(widgetZone, (int)additionalData);
            }
            else if (additionalData is Nop.Web.Models.Catalog.ProductDetailsModel productDetailsModel)
            {
                return this.Invoke(widgetZone, productDetailsModel.Id);
            }
            else if (additionalData is Nop.Web.Models.Catalog.ProductOverviewModel productOverviewModel)
            {
                return this.Invoke(widgetZone, productOverviewModel.Id);
            }
            else if (additionalData is int)
            {
                if (this._pluginSettingService.OpenModuleButton_General_GetUnknownWidgetZoneWarning(this._storeContext.CurrentStore.Id).Value)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"Soft2Print have not pre-confirmed that this widget zone '{widgetZone}' gets the correct input value.");
                    sb.AppendLine("To create a new soft2print project we need to know the product id, but in this case we have only bin able to confirm that the value we are working on is an integer (witch the product id also is).");
                    sb.AppendLine("we can't asume this is an error, the changes that this is actally ok, is very large");
                    sb.AppendLine($"- ComponentName: '{ComponentName}'");
                    sb.AppendLine($"- widget zone: '{widgetZone}'");
                    sb.AppendLine($"- value: {additionalData.ToString()}");
                    sb.AppendLine();
                    sb.AppendLine("if there is no problem, just disable this warning.");
                    sb.AppendLine("Here is a list of the pre-confirmed widget zones: (Please notice this is a teknical confirm, not the html)");
                    foreach (var confirmedWidgetZone in ConfirmedWidgetZones.OrderBy(i => i))
                        sb.AppendLine($" - {confirmedWidgetZone}");
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("To avoit getting this warning, just disaple it under the soft2print plugin configurations");

                    this._logger.Warning(sb.ToString());
                }

                return this.Invoke(widgetZone, (int)additionalData);
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Soft2Print is not able to use this widget zone since the value is not an integer.");
                sb.AppendLine($"- ComponentName: '{ComponentName}'");
                sb.AppendLine($"- widget zone: '{widgetZone}'");
                sb.AppendLine($"- value: {additionalData.ToString()}");
                sb.AppendLine();
                sb.AppendLine("Here is a list of the pre-confirmed widget zones: (Please notice this is a teknical confirm, not the html)");
                foreach (var confirmedWidgetZone in ConfirmedWidgetZones.OrderBy(i => i))
                    sb.AppendLine($" - {confirmedWidgetZone}");
                sb.AppendLine();
                sb.AppendLine("We have hidden the widget as a work arount for this problem.");
                this._logger.Error(sb.ToString());
                return this.Content(string.Empty);
            }
        }
        private IViewComponentResult Invoke(string widgetZone, int productID)
        {
            var product = this._productService.GetProductById(productID);
            var productAttributeMappings = this._productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            var templateName = this._productTemplateService.GetProductTemplateById(product.ProductTemplateId).Name;

            // Here we should be able to figure out the product template
            var model = new Model.OpenModuleButton.OpenModule() { Show = false };
            if (templateName.Equals(Plugin.Soft2PrintProductTemplateName))
                model.Show = true;

            if (model.Show)
            {
                var themeStrcutureMapping = productAttributeMappings.FirstOrDefault(i => i.ProductAttribute.Name.Equals(Services.ProductAttributeService.AttributeKeys.ThemeStrcuture));
                if (themeStrcutureMapping != null)
                {
                    var themeListingStrcutureMapping = productAttributeMappings.FirstOrDefault(i => i.ProductAttribute.Name.Equals(Services.ProductAttributeService.AttributeKeys.ThemeStrcutureOnSite));
                    if (themeListingStrcutureMapping != null)
                    {
                        var firstValue = themeListingStrcutureMapping.ProductAttributeValues.FirstOrDefault();
                        if (firstValue != null)
                            if (firstValue.IsPreSelected)
                            {
                                model.Show = false;
                            }
                    }
                }


            }

            if (this._pluginSettingService.OpenModuleButton_ProductBox_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
            {
                model.Url = Url.RouteUrl(RouteProvider.CreateProject_Product_Box, new { id = productID });
                return View(Plugin.Location + "Views/OpenModuleButton/ProductBox.cshtml", model);
            }
            else if (this._pluginSettingService.OpenModuleButton_ProductDetails_WidgetZone(this._storeContext.CurrentStore.Id).Value.ToLower().Split(',').Contains(widgetZone.ToLower()))
            {
                model.Url = Url.RouteUrl(RouteProvider.CreateProject_Product_Details, new { id = productID });
                return View(Plugin.Location + "Views/OpenModuleButton/ProductDetails.cshtml", model);
            }
            else
                throw new System.Exception("How did we get here?");
        }
        #endregion
    }
}
