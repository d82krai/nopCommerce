using Nop.Plugin.Soft2Print.Model.Settings;
using Nop.Plugin.Soft2Print.Services;

namespace Nop.Plugin.Soft2Print.Widget.Projects.Services
{
    public class PluginSettingService
    {
        #region Conts
        public class Keys
        {
            private const string SettingsPrefix = Soft2Print.Services.SettingService.Keys.SettingsPrefix + "plugin.projects.";

            public class Genaral
            {
                private const string SettingsPrefix = Keys.SettingsPrefix + "general.";

                public class Button
                {
                    private const string SettingsPrefix = Genaral.SettingsPrefix + "button.";

                    internal const string ShowDelete = SettingsPrefix + "showdelete";
                    internal const string ShowRename = SettingsPrefix + "showrename";
                    internal const string ShowCopy = SettingsPrefix + "showcopy";
                }

                public class Detials
                {
                    private const string SettingsPrefix = Genaral.SettingsPrefix + "details.";

                    internal const string DefaultProjectName = SettingsPrefix + "defaultprojectname";

                    internal const string ShowPreview = SettingsPrefix + "showpreview";
                    internal const string ShowCreated = SettingsPrefix + "showctreated";
                    internal const string ShowLastChanged = SettingsPrefix + "showlastchanged";
                    internal const string ShowTheme = SettingsPrefix + "showtheme";
                    internal const string ShowProduct = SettingsPrefix + "showproduct";
                    internal const string GetWarningIfS2PThemeIsShown = SettingsPrefix + "getwarningifs2pthemeisshown";
                    internal const string ShowInheritedBy = SettingsPrefix + "showinheritedby";
                }
            }

            public class ProductDetails
            {
                private const string SettingsPrefix = Keys.SettingsPrefix + "productdetails.";

                internal const string Show = SettingsPrefix + "show";
                internal const string HideIfGuest = SettingsPrefix + "hideifguest";
                internal const string WidgetZone = SettingsPrefix + "widgetzone";
                internal const string ViewMode = SettingsPrefix + "viewmode";
            }

            public class HeaderLink
            {
                private const string SettingsPrefix = Keys.SettingsPrefix + "headerlink.";

                internal const string Show = SettingsPrefix + "show";
                internal const string HideIfGuest = SettingsPrefix + "hideifguest";
                internal const string WidgetZone = SettingsPrefix + "widgetzone";
            }
            public class AccountLink
            {
                private const string SettingsPrefix = Keys.SettingsPrefix + "accountlink.";

                internal const string Show = SettingsPrefix + "show";
                internal const string WidgetZone = SettingsPrefix + "widgetzone";
            }
        }
        #endregion

        #region Fields

        private readonly SettingService _settingService;

        #endregion
        #region Ctor

        public PluginSettingService(
                SettingService settingService
            )
        {
            this._settingService = settingService;
        }

        #endregion

        public void AddSettings<T>(string key, T value, int storeId = 0) { this._settingService.AddSettings<T>(key, value, storeId); }

        #region General
        public BoolSetting Genaral_Button_ShowCopy(int storeId = 0)
        {
            var key = Keys.Genaral.Button.ShowCopy;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral_Button_ShowRename(int storeId = 0)
        {
            var key = Keys.Genaral.Button.ShowRename;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral_Button_ShowDelete(int storeId = 0)
        {
            var key = Keys.Genaral.Button.ShowDelete;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }

        public BoolSetting Genaral__Detials_ShowCreated(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.ShowCreated;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral__Detials_ShowLastChanged(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.ShowLastChanged;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral__Detials_ShowPreview(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.ShowPreview;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral__Detials_ShowTheme(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.ShowTheme;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral__Detials_ShowProduct(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.ShowProduct;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral__Detials_GetWarningIfS2PThemeIsShown(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.GetWarningIfS2PThemeIsShown;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting Genaral__Detials_ShowInheritedBy(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.ShowInheritedBy;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public StringSetting Genaral__Detials_DefaultProjectName(int storeId = 0)
        {
            var key = Keys.Genaral.Detials.DefaultProjectName;
            var value = this._settingService.GetSettings<string>(key, string.Empty, storeId);
            return new StringSetting(key, value);
        }
        #endregion


        #region HeaderLink
        public BoolSetting HeaderLink_Show(int storeId = 0)
        {
            var key = Keys.HeaderLink.Show;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting HeaderLink_HideIfGuest(int storeId = 0)
        {
            var key = Keys.HeaderLink.HideIfGuest;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public StringSetting HeaderLink_WidgetZone(int storeId = 0)
        {
            var key = Keys.HeaderLink.WidgetZone;
            var value = this._settingService.GetSettings<string>(key, string.Empty, storeId);
            return new StringSetting(key, value);
        }
        #endregion

        #region AccountLink
        public BoolSetting AccountLink_Show(int storeId = 0)
        {
            var key = Keys.AccountLink.Show;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public StringSetting AccountLink_WidgetZone(int storeId = 0)
        {
            var key = Keys.AccountLink.WidgetZone;
            var value = this._settingService.GetSettings<string>(key, string.Empty, storeId);
            return new StringSetting(key, value);
        }
        #endregion

        #region ProductDetails
        public BoolSetting ProductDetails_Show(int storeId = 0)
        {
            var key = Keys.ProductDetails.Show;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public BoolSetting ProductDetails_HideIfGuest(int storeId = 0)
        {
            var key = Keys.ProductDetails.HideIfGuest;
            var value = this._settingService.GetSettings<bool>(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public StringSetting ProductDetails_WidgetZone(int storeId = 0)
        {
            var key = Keys.ProductDetails.WidgetZone;
            var value = this._settingService.GetSettings<string>(key, string.Empty, storeId);
            return new StringSetting(key, value);
        }
        public IntSetting ProductDetails_ViewMode(int storeId = 0)
        {
            var key = Keys.ProductDetails.ViewMode;
            var value = this._settingService.GetSettings<int>(key, int.MinValue, storeId);
            return new IntSetting(key, value);
        }
        #endregion


    }
}