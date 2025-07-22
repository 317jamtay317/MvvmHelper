using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.ComponentModel;
using MvvmHelper.Interfaces;
namespace MvvmHelper.Generator.Sample;

public partial class PersonViewModel : INotifyPropertyChanged, INotifyDataErrorInfo
{
        public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private T GetValue<T>([CallerMemberName] string propertyName = null)
    {
        if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));
        if (_propertiesValues.TryGetValue(propertyName, out var value))
        {
            return (T)value;
        }
        return default!;
    }
 
    private bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
    {
        if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));
        if (_propertiesValues.TryGetValue(propertyName, out var oldValue))
        {
            if (Equals(oldValue, value)) return false;
            _propertiesValues[propertyName] = value!;
        }
        else
        {
            _propertiesValues.Add(propertyName, value!);
        }
        if(ValidateViewModel is not null)
        {
            Errors.Remove(propertyName);
            var errors = ValidateViewModel(propertyName);
            if(errors?.Any() ?? false)
            {
                Errors[propertyName] = errors?.ToList();
            }
        }
        OnPropertyChanged(propertyName);
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        if(this is IValueTracking valueTracking)
        {
            valueTracking.TrackValue(propertyName, oldValue);
        }
        return true;
    }
    
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        return SetValue(value, propertyName);
    }
    
    private Dictionary<string, object> _propertiesValues = new();
        /// <summary>
    /// Validates the ViewModel. See Remarks...
    /// </summary>
    /// <remarks>
    /// If this is not set, no validation will be performed. This will be called in SetValue after the property is set.
    /// The property name will be passed as a parameter, and the function should return a list of errors for that property.
    /// </remarks>
    private Func<string, IEnumerable<string>> ValidateViewModel { get; set; }
    
    #pragma warning disable CS0067
    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
    #pragma warning disable CS0067
    
    public System.Collections.IEnumerable GetErrors(string propertyName)
    {
       var list = Errors[propertyName?? string.Empty];
        return list?.AsEnumerable()??[]; 
    }

    public bool HasErrors => Errors.Any();
    
    public bool IsValid => !HasErrors;

    private Dictionary<string, List<string>> Errors { get; } = new();
}