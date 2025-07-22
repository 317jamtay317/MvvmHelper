using System.ComponentModel;
using MvvmHelper.Attributes;

namespace MvvmHelper.Generator.Sample;
[GenerateViewModel(UseIDataErrorInfo = true, UseChangeTracking = true, ImplementIsDirty = true)]
public partial class PersonViewModel
{
    public PersonViewModel()
    {
        ValidateViewModel = Validate;
    }

    private IEnumerable<string> Validate(string propertyName)
    {
        if(propertyName == nameof(FirstName) && string.IsNullOrWhiteSpace(FirstName))
            yield return "First name cannot be empty.";
        if(propertyName == nameof(FirstName) && FirstName?.Length > 50)
            yield return "First name cannot be longer than 50 characters.";
    }

    public string FirstName
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
    private void Loaded()
    {
        IsLoaded = true;
    }
}