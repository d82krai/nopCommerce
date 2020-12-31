using System;

namespace Nop.Plugin.Soft2Print.Model.Settings
{
    public class StringSetting : ISetting<String>
    {
        public StringSetting(string key, string value) : this(key)
        {
            this.Value = value;
        }
        public StringSetting(string key) : this()
        {
            this.Key = key;
        }
        public StringSetting()
        {

        }

        public String Key { get; set; }
        public string Value { get; set; }
        public bool hasValue => !string.IsNullOrEmpty(this.Value);
    }
}
