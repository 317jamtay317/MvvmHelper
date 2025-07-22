using System;
using System.Collections.Generic;
using System.ComponentModel;
using MvvmHelper.Interfaces;

namespace MvvmHelper.Generator.Sample;

public partial class PersonViewModel : IChangeTracking, IRevertibleChangeTracking, IValueTracking
{
    public bool IsDirty => IsChanged;

    private readonly Dictionary<string, object> _originalValues = new();

    public bool IsChanged => _originalValues.Count > 0;

    public bool FirstNameIsChanged => _originalValues.ContainsKey(nameof(FirstName));
    public bool IsLoadedIsChanged => _originalValues.ContainsKey(nameof(IsLoaded));

    public string FirstNameOriginalValue
    {
        get
        {
            if (_originalValues.TryGetValue(nameof(FirstName), out var value))
            {
                return (string)value;
            }
            return FirstName;
        }
    }
    public bool IsLoadedOriginalValue
    {
        get
        {
            if (_originalValues.TryGetValue(nameof(IsLoaded), out var value))
            {
                return (bool)value;
            }
            return IsLoaded;
        }
    }

    public void AcceptChanges()
    {
        _originalValues.Clear();
    }

    public void RejectChanges()
    {
        if (_originalValues.ContainsKey(nameof(FirstName)))
        {
            FirstName = (string)_originalValues[nameof(FirstName)];
        }
        if (_originalValues.ContainsKey(nameof(IsLoaded)))
        {
            IsLoaded = (bool)_originalValues[nameof(IsLoaded)];
        }

        _originalValues.Clear();
    }

    public void TrackValue(string propertyName, object oldValue)
    {
        if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));

        if (!_originalValues.ContainsKey(propertyName))
        {
            _originalValues[propertyName] = oldValue;
            OnPropertyChanged($"{propertyName}OriginalValue");
        }

        OnPropertyChanged($"{propertyName}IsChanged");
    }
}