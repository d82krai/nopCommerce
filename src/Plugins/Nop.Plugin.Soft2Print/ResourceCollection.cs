using System.Collections.Generic;

namespace Nop.Plugin.Soft2Print
{
    public class ResourceCollection
    {
        private const string HintPrefix = ".Hint";

        public class Key
        {
            private const string prefix = "Plugins.Soft2Print.Fields.";

            public class ShoppingCart
            {
                private const string prefix = Key.prefix + "ShoppingCart.";

                public const string ReconfirmRequired = prefix + "ReconfirmRequired";

            }

            public class OpenModuleButton
            {
                private const string prefix = Key.prefix + "OpenModuleButton.";

                public const string Text = prefix + "Text";
            }

            public class ThemeList
            {
                private const string prefix = Key.prefix + "ThemeList.";

                public const string Header = prefix + "Header";
            }

            public class ViewModule
            {
                private const string prefix = Key.prefix + "ViewModule.";

                public class PageTitel
                {
                    private const string prefix = ViewModule.prefix + "PageTitel.";

                    public const string View = prefix + "View";
                    public const string Edit = prefix + "Edit";
                }
            }

            public class Admin
            {
                private const string prefix = Key.prefix + "Admin.";

                public class Account
                {
                    private const string prefix = Admin.prefix + "Account.";

                    public const string Header = prefix + "Header";

                    public const string Key = prefix + "Key";
                    public const string Secret = prefix + "Secret";
                }

                public class API
                {
                    private const string prefix = Admin.prefix + "API.";

                    public const string Header = prefix + "Header";

                    public const string Url = prefix + "Url";
                }

                public class ThemeList
                {
                    private const string prefix = Admin.prefix + "ThemeList.";

                    public const string Header = prefix + "Header";

                    public const string GetUnknownWidgetZoneWarning = prefix + "GetUnknownWidgetZoneWarning";

                    public const string WidgetZone = prefix + "WidgetZone";
                }

                public class OpenModuleButton
                {
                    private const string prefix = Admin.prefix + "OpenModuleButton.";

                    public class General
                    {
                        internal const string prefix = OpenModuleButton.prefix + "General.";

                        public const string Header = prefix + "Header";

                        public const string GetUnknownWidgetZoneWarning = prefix + "GetUnknownWidgetZoneWarning";
                    }
                    public class ProductDetails
                    {
                        internal const string prefix = OpenModuleButton.prefix + "ProductDetails.";

                        public const string Header = prefix + "Header";

                        public const string WidgetZone = prefix + "WidgetZone";
                    }
                    public class ProductBox
                    {
                        internal const string prefix = OpenModuleButton.prefix + "ProductBox.";

                        public const string Header = prefix + "Header";

                        public const string WidgetZone = prefix + "WidgetZone";
                    }
                }

                public class ViewModule
                {
                    private const string prefix = Admin.prefix + "ViewModule.";

                    public const string Header = prefix + "Header";

                    public const string OpenMode = prefix + "OpenMode";

                    public class OpenModes
                    {
                        internal const string prefix = ViewModule.prefix + "OpenModes.";

                        public const string FullScreen = prefix + "FullScreen";
                        public const string EaseInFullScreen = prefix + "EaseInFullScreen";
                    }
                }
            }



        }

        public static Dictionary<string, string> GetDefaultValues()
        {
            var defaultTexts = new Dictionary<string, string>();


            #region ShoppingCart
            defaultTexts.Add(Key.ShoppingCart.ReconfirmRequired, "Changes may have been made to the project. Click '{0}' to re confirm project");
            #endregion

            #region OpenModuleButton
            defaultTexts.Add(Key.ThemeList.Header, "Designs");
            #endregion

            #region OpenModuleButton
            defaultTexts.Add(Key.OpenModuleButton.Text, "Open editor");
            #endregion

            #region ViewModule
            defaultTexts.Add(Key.ViewModule.PageTitel.View, "View project");
            defaultTexts.Add(Key.ViewModule.PageTitel.Edit, "Edit project");
            #endregion


            #region Admin

            #region API
            defaultTexts.Add(Key.Admin.API.Header, "Soft2Print API");
            defaultTexts.Add(Key.Admin.API.Url, "Url");
            defaultTexts.Add(Key.Admin.API.Url + HintPrefix, "Use the default url unless, you have a special agreement with the Soft2Print team to do otherwise");
            #endregion

            #region Account
            defaultTexts.Add(Key.Admin.Account.Header, "Account");
            defaultTexts.Add(Key.Admin.Account.Key, "Key");
            defaultTexts.Add(Key.Admin.Account.Key + HintPrefix, "Enter your personal Soft2Print Key, If you don't have a soft2print key please contact the soft2print team.");
            defaultTexts.Add(Key.Admin.Account.Secret, "Secret");
            defaultTexts.Add(Key.Admin.Account.Secret + HintPrefix, "Enter your personal Soft2Print Secret, If you don't have a soft2print secret please contact the soft2print team");
            #endregion

            #region ThemeList
            defaultTexts.Add(Key.Admin.ThemeList.Header, "Theme list");
            defaultTexts.Add(Key.Admin.ThemeList.GetUnknownWidgetZoneWarning, "Get unknown widgetzone warning");
            defaultTexts.Add(Key.Admin.ThemeList.GetUnknownWidgetZoneWarning + HintPrefix, "Get a warning if the widgetzone configured to the open module buttons is un confirmed af as working widgetzone, this does not mean that it won't work. just that the soft2print team have not confirmed it to work.");
            defaultTexts.Add(Key.Admin.ThemeList.WidgetZone, "Widget zone");
            #endregion

            #region ViewModule
            defaultTexts.Add(Key.Admin.OpenModuleButton.General.Header, "Open module button - Product box");
            defaultTexts.Add(Key.Admin.OpenModuleButton.General.GetUnknownWidgetZoneWarning, "Get unknown widgetzone warning");
            defaultTexts.Add(Key.Admin.OpenModuleButton.General.GetUnknownWidgetZoneWarning + HintPrefix, "Get a warning if the widgetzone configured to the open module buttons is un confirmed af as working widgetzone, this does not mean that it won't work. just that the soft2print team have not confirmed it to work.");
            defaultTexts.Add(Key.Admin.OpenModuleButton.ProductBox.Header, "Open module button - Product box");
            defaultTexts.Add(Key.Admin.OpenModuleButton.ProductBox.WidgetZone, "Widget zone");
            defaultTexts.Add(Key.Admin.OpenModuleButton.ProductDetails.Header, "Open module button - Product details");
            defaultTexts.Add(Key.Admin.OpenModuleButton.ProductDetails.WidgetZone, "Widget zone");
            #endregion

            #region ViewModule
            defaultTexts.Add(Key.Admin.ViewModule.Header, "View module");
            defaultTexts.Add(Key.Admin.ViewModule.OpenMode, "Opening Method");

            defaultTexts.Add(Key.Admin.ViewModule.OpenModes.FullScreen, "Full screen");
            defaultTexts.Add(Key.Admin.ViewModule.OpenModes.EaseInFullScreen, "Ease in to full screen");
            #endregion
            #endregion

            return defaultTexts;
        }
    }
}
