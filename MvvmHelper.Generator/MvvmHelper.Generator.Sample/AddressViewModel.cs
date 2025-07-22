using MvvmHelper.Attributes;

namespace MvvmHelper.Generator.Sample;

[GenerateViewModel]
public partial class AddressViewModel
{
    public string StreetAddress
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

    public string PostalCode
    {
        get => GetValue<string>();
        set => SetValue(value);
    }

    public bool IsLoaded
    {
        get => GetValue<bool>();
        set => SetValue(value);
    }
    
    [GenerateCommand]
    private void Loaded() => IsLoaded = true;
}