using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.ComponentModel;
using MvvmHelper.Interfaces;
namespace MvvmHelper.Generator.Sample;

public partial class AddressViewModel : INotifyPropertyChanged
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
        
        OnPropertyChanged(propertyName);
        
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
    
}