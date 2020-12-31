namespace Nop.Plugin.Soft2Print.Model.Settings
{
    public interface ISetting<T>
    {
        string Key { get; }
        T Value { get; }
        bool hasValue { get; }
    }
}
