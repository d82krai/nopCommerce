using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Soft2Print.ViewModel
{
    public class Configure : BaseNopModel
    {
        [NopResourceDisplayName(ResourceCollection.Key.Admin.API.Url)]
        public string API_Url { get; set; }


        [NopResourceDisplayName(ResourceCollection.Key.Admin.Account.Key)]
        public string Account_Key { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.Account.Secret)]
        public string Account_Secret { get; set; }


        [NopResourceDisplayName(ResourceCollection.Key.Admin.OpenModuleButton.General.GetUnknownWidgetZoneWarning)]
        public bool OpenModuleButton_General_GetUnknownWidgetZoneWarning { get; set; }

        [NopResourceDisplayName(ResourceCollection.Key.Admin.OpenModuleButton.ProductBox.WidgetZone)]
        public string OpenModuleButton_ProductBox_WidgetZone { get; set; }

        [NopResourceDisplayName(ResourceCollection.Key.Admin.OpenModuleButton.ProductDetails.WidgetZone)]
        public string OpenModuleButton_ProductDetails_WidgetZone { get; set; }


        [NopResourceDisplayName(ResourceCollection.Key.Admin.ThemeList.GetUnknownWidgetZoneWarning)]
        public bool ThemeList_GetUnknownWidgetZoneWarning { get; set; }

        [NopResourceDisplayName(ResourceCollection.Key.Admin.ThemeList.WidgetZone)]
        public string ThemeList_WidgetZone { get; set; }



        public int ViewModule_OpenMode { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.ViewModule.OpenMode)]
        public SelectList ViewModule_OpenModeValues { get; set; }



    }
}
