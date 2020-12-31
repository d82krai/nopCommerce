using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Soft2Print.Widget.Projects.ViewModel
{
    public class Configure : BaseNopModel
    {
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Action.ShowCopy)]
        public bool General_Action_ShowCopy { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Action.ShowDelete)]
        public bool General_Action_ShowDelete { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Action.ShowRename)]
        public bool General_Action_ShowRename { get; set; }


        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.DefaultProjectName)]
        public string General_Details_DefaultProjectName { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.GetWarningIfS2PThemeIsShown)]
        public bool General_Details_GetWarningIfS2PThemeIsShown { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.ShowCreated)]
        public bool General_Details_ShowCreated { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.ShowInheritedBy)]
        public bool General_Details_ShowInheritedBy { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.ShowLastChanged)]
        public bool General_Details_ShowLastChanged { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.ShowPreview)]
        public bool General_Details_ShowPreview { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.ShowTheme)]
        public bool General_Details_ShowTheme { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.General.Details.ShowProduct)]
        public bool General_Details_ShowProduct { get; set; }



        [NopResourceDisplayName(ResourceCollection.Key.Admin.ProductDetails.Show)]
        public bool ProductDetails_Show { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.ProductDetails.HideIfGuest)]
        public bool ProductDetails_HideIfGuest { get; set; }

        public int ProductDetails_ViewMode { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.ProductDetails.ViewMode)]
        public SelectList ProductDetails_ViewModeValues { get; set; }

        [NopResourceDisplayName(ResourceCollection.Key.Admin.ProductDetails.WidgetZone)]
        public string ProductDetails_WidgetZone { get; set; }


        [NopResourceDisplayName(ResourceCollection.Key.Admin.HeaderLink.Show)]
        public bool HeaderLink_Show { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.HeaderLink.HideIfGuest)]
        public bool HeaderLink_HideIfGuest { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.HeaderLink.WidgetZone)]
        public string HeaderLink_WidgetZone { get; set; }

        [NopResourceDisplayName(ResourceCollection.Key.Admin.AccountLink.Show)]
        public bool AccountLink_Show { get; set; }
        [NopResourceDisplayName(ResourceCollection.Key.Admin.AccountLink.WidgetZone)]
        public string AccountLink_WidgetZone { get; set; }

    }
}
