using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace MvvmHelper.Generator;

public record ClassInfo
{
    public string? Name { get; set; } = null!;
    
    public string? Namespace { get; set; } = null!;

    public bool ImplementINotifyPropertyChanged { get; set; } = true;

    public bool ImplementIDataErrorInfo { get; set; } = false;

    public Accessibility? Accessibility { get; set; }
    
    public List<PropertyInfo> Properties { get; set; } = new();
    
    public List<CommandInfo> Commands { get; set; } = new();
}