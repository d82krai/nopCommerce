using System;

namespace Nop.Plugin.Soft2Print.Model.Settings
{
    public class BoolSetting : ISetting<Boolean>
    {
        public BoolSetting(string key, bool value) : this(key)
        {
            this.Value = value;
        }
        public BoolSetting(string key) : this()
        {
            this.Key = key;
        }
        public BoolSetting()
        {

        }

        public string Key { get; set; }

        private bool? _Value { get; set; }
        public bool Value
        {
            get { return this._Value.GetValueOrDefault(false); }
            set { this._Value = value; }
        }

        public bool hasValue => this._Value.HasValue;
    }
}
