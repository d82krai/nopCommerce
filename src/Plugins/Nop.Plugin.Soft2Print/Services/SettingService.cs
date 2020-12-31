using Nop.Plugin.Soft2Print.Model.Settings;
using Nop.Services.Configuration;
using Nop.Services.Stores;
using System.Linq;

namespace Nop.Plugin.Soft2Print.Services
{
    public class SettingService
    {
        #region Conts
        public class Keys
        {
            public const string SettingsPrefix = "soft2print.";

            internal const string AccountKey = SettingsPrefix + "account.key";
            internal const string AccountSecret = SettingsPrefix + "account.secret";

            internal const string APIUrl = SettingsPrefix + "api.url";
        }
        #endregion

        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public SettingService(
                ISettingService settingService,
               IStoreService storeService
            )
        {
            this._settingService = settingService;
            this._storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Use this to get settings that is speficic for the plugin.
        /// </summary>
        public T GetSettings<T>(string key, T defaultValue, int storeId = 0, bool loadSharedValueIfNotFound = true)
        {
            return this._settingService.GetSettingByKey<T>(key, defaultValue, storeId, loadSharedValueIfNotFound);
        }

        /// <summary>
        /// Use this to add a new setting to the settings section in nop, THIS IS FOR SETTINGS THAT DOES NOT SUPPORT A SHARED SETTING
        /// </summary>
        public void AddSettingsNotAllStores<T>(string key, T value)
        {
            this.AddSettings<T>(key, value, this._storeService.GetAllStores().First().Id);
        }
        /// <summary>
        /// Use this to add a new setting to the settings section in nop
        /// </summary>
        public void AddSettings<T>(string key, T value, int storeId = 0)
        {
            this._settingService.SetSetting<T>(key, value, storeId);
        }

        public StringSetting AccountKey(int storeId = 0)
        {
            if (storeId > 0)
            {
                var value = this.GetSettings<string>(Keys.AccountKey, string.Empty, storeId, false);
                return new StringSetting(Keys.AccountKey, value);
            }
            else
                return new StringSetting(Keys.AccountKey);
        }
        public StringSetting AccountSecret(int storeId = 0)
        {
            if (storeId > 0)
            {
                var value = this.GetSettings<string>(Keys.AccountSecret, string.Empty, storeId, false);
                return new StringSetting(Keys.AccountSecret, value);
            }
            else
                return new StringSetting(Keys.AccountSecret);
        }

        public StringSetting APIUrl(int storeId = 0)
        {
            var value = this.GetSettings<string>(Keys.APIUrl, string.Empty, storeId, true);
            return new StringSetting(Keys.APIUrl, value);
        }

        #endregion

    }
}
