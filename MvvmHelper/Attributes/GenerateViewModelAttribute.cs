using System;
using System.ComponentModel;

namespace MvvmHelper.Attributes;

/// <summary>
/// Apply this attribute to a class to generate the ViewModel boilerplate code.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class GenerateViewModelAttribute : Attribute
{
    /// <summary>
    /// If true, the generated ViewModel will implement INotifyDataErrorInfo interface.
    /// </summary>
    /// <remarks>
    /// Default is false.
    /// </remarks>
    public bool UseIDataErrorInfo { get; set; } = false;
                
    /// <summary>
    /// If true, the generated ViewModel will implement INotifyPropertyChanged interface.
    /// </summary>
    /// <remarks>
    /// Default is true.
    /// </remarks>
    public bool ImplementINotifyPropertyChanged { get; set; } = true;

    /// <summary>
    /// Implements the <see cref="IChangeTracking"/> and <see cref="IRevertibleChangeTracking"/> interfaces.
    /// </summary>
    public bool UseChangeTracking { get; set; }
    
    /// <summary>
    /// If true the generated ViewModel will implement an IsDirty property and track changes to properties.
    /// </summary>
    /// <remarks>
    /// Default is false. For this to work, the <see cref="UseChangeTracking"/> must be set to true.
    /// </remarks>
    public bool ImplementIsDirty { get; set; } = false;
}