namespace MvvmHelper.Generator.Sample.ViewModels;

[GenerateViewModel]
public partial class AddressViewModel
{
    public string Street
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string City
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string State
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public string Zip
    {
        get => GetValue<string>();
        set => SetValue(value);
    }
}