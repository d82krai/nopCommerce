using Nop.Plugin.Soft2Print.Model.Settings;

namespace Nop.Plugin.Soft2Print.Services
{
    public class PluginSettingService
    {
        #region Conts
        public class Keys
        {
            private const string Prefix = "soft2print.plugin.";

            public class OpenModuleButton
            {
                private const string Prefix = Keys.Prefix + "openmodulebutton.";

                public class General
                {
                    private const string Prefix = OpenModuleButton.Prefix + "general.";

                    public const string GetUnknownWidgetZoneWarning = Prefix + "getunknownwidgetzonewarning";
                }
                public class ProductDetails
                {
                    private const string Prefix = OpenModuleButton.Prefix + "productdetails.";

                    public const string WidgetZone = Prefix + "widgetzone";
                }
                public class ProductBox
                {
                    private const string Prefix = OpenModuleButton.Prefix + "productbox.";

                    public const string WidgetZone = Prefix + "widgetzone";
                }
            }

            public class ThemeList
            {
                private const string Prefix = Keys.Prefix + "themelist.";

                public const string GetUnknownWidgetZoneWarning = Prefix + "getunknownwidgetzonewarning";
                public const string WidgetZone = Prefix + "widgetzone";
            }

            public class ViewModule
            {
                private const string Prefix = Keys.Prefix + "viewmodules.";

                internal const string OpenMode = Prefix + "openmode";
            }
        }
        #endregion

        #region Fields

        private readonly SettingService _soft2PrintSettingService;

        #endregion
        #region Ctor

        public PluginSettingService(
                SettingService soft2PrintSettingService
            )
        {
            this._soft2PrintSettingService = soft2PrintSettingService;
        }

        #endregion

        public void AddSettings<T>(string key, T value, int storeId = 0) { this._soft2PrintSettingService.AddSettings<T>(key, value, storeId); }

        #region OpenModuleButton    
        public BoolSetting OpenModuleButton_General_GetUnknownWidgetZoneWarning(int storeId = 0)
        {
            var key = Keys.OpenModuleButton.General.GetUnknownWidgetZoneWarning;
            var value = this._soft2PrintSettingService.GetSettings(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public StringSetting OpenModuleButton_ProductBox_WidgetZone(int storeId = 0)
        {
            var key = Keys.OpenModuleButton.ProductBox.WidgetZone;
            var value = this._soft2PrintSettingService.GetSettings(key, string.Empty, storeId);
            return new StringSetting(key, value);
        }
        public StringSetting OpenModuleButton_ProductDetails_WidgetZone(int storeId = 0)
        {
            var key = Keys.OpenModuleButton.ProductDetails.WidgetZone;
            var value = this._soft2PrintSettingService.GetSettings(key, string.Empty, storeId);
            return new StringSetting(key, value);
        }



        #endregion

        #region ThemeList    
        public BoolSetting ThemeList_GetUnknownWidgetZoneWarning(int storeId = 0)
        {
            var key = Keys.ThemeList.GetUnknownWidgetZoneWarning;
            var value = this._soft2PrintSettingService.GetSettings(key, false, storeId);
            return new BoolSetting(key, value);
        }
        public StringSetting ThemeList_WidgetZone(int storeId = 0)
        {
            var key = Keys.ThemeList.WidgetZone;
            var value = this._soft2PrintSettingService.GetSettings(key, string.Empty, storeId);
            return new StringSetting(key, value);
        }
        #endregion


        #region ViewModule  
        public IntSetting ViewModule_OpenMode(int storeId = 0)
        {
            var key = Keys.ViewModule.OpenMode;
            var value = this._soft2PrintSettingService.GetSettings(key, (int)Model.ViewModule.OpenMode.FullScreen, storeId);
            return new IntSetting(key, value);
        }
        #endregion



    }
}