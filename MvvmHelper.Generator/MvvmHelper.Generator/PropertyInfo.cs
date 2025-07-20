using Microsoft.CodeAnalysis;

namespace MvvmHelper.Generator;

public record PropertyInfo
{
    public string Name { get; set; } = null!;
    
    public string Type { get; set; } = null!;

    public Accessibility Accessibility { get; set; } = Accessibility.Public;
}