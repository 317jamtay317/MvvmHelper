using System;

namespace MvvmHelper.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class GenerateCommandAttribute : Attribute
{
    public string? CanExecuteMethod { get; set; }
}