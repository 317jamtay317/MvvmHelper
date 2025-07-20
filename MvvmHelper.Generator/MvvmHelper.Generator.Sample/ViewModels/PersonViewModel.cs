using System;
using System.Collections.Generic;

namespace MvvmHelper.Generator.Sample.ViewModels;

[GenerateViewModel(UseIDataErrorInfo = true)]
public partial class PersonViewModel
{
    public PersonViewModel()
    {
        ValidateViewModel = Validate;
    }

    private IEnumerable<string> Validate(string propertyName)
    {
        if(propertyName == nameof(Name) && Name.Length > 10)
        {
            yield return "Name cannot be longer than 10 characters";
        }
    }

    public DateTime LastTimeUpdated { get; private set; }
    public bool IsLoaded { get; private set; }

    public string Name
    {
        get => GetValue<string>();
        set => SetValue(value);
    }
    
    [GenerateCommand]
    private void Loaded()
    {
        IsLoaded = true;
    }

    [GenerateCommand(CanExecuteMethod=nameof(CanUpdateTime))]
    private void UpdateTime(DateTime time)
    {
        LastTimeUpdated = time;
    }
    private bool CanUpdateTime(DateTime time)
    {
        return time > LastTimeUpdated;
    }
}