namespace Nop.Plugin.Soft2Print.Model.Settings
{
    public class IntSetting : ISetting<int>
    {
        public IntSetting(string key, int value) : this(key)
        {
            this.Value = value;
        }
        public IntSetting(string key) : this()
        {
            this.Key = key;
        }
        public IntSetting()
        {

        }

        public string Key { get; set; }

        private int? _Value { get; set; }
        public int Value
        {
            get { return this._Value.GetValueOrDefault(int.MinValue); }
            set { this._Value = value; }
        }

        public bool hasValue => this._Value.HasValue;
    }
}
