using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MvvmHelper.Generator;

public partial class ViewModelGenerator
{
    private const string IsDirtyTemplate =
        """
            public bool IsDirty => {{IsDirtyExpression}};
        """;

    private const string IChangeTrackingTemplate =
        """
        using System;
        using System.Collections.Generic;
        using System.ComponentModel;
        using MvvmHelper.Interfaces;
        
        namespace {{Namespace}};
        
        {{Accessability}} partial class {{className}} : IChangeTracking, IRevertibleChangeTracking, IValueTracking
        {
            {{IsDirtyImplementation}}
            private readonly Dictionary<string, object> _originalValues = new();
            

            public bool IsChanged => _originalValues.Count > 0;
            
            {{PropertyIsChangedImplementation}}
            {{PropertyOriginalValuesImplementation}}

            public void AcceptChanges()
            {
                _originalValues.Clear();
            }

            public void RejectChanges()
            {
                {{RejectChangesImplementation}}
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
        """;
    private const string RejectPropertyChangeTemplate =
        """
            if (_originalValues.ContainsKey(nameof({{PropertyName}})))
            {
                {{PropertyName}} = ({{PropertyType}})_originalValues[nameof({{PropertyName}})];
            }
        """;
    public const string PropertyIsChangedTemplate =
        """
            public bool {{PropertyName}}IsChanged => _originalValues.ContainsKey(nameof({{PropertyName}}));
        """;
    public const string PropertyOriginalValuesTemplate =
        """
            public {{PropertyType}} {{PropertyName}}OriginalValue
            {
                get
                {
                    if (_originalValues.TryGetValue(nameof({{PropertyName}}), out var value))
                    {
                        return ({{PropertyType}})value;
                    }
                    return {{PropertyName}};
                }
            }
        """;
    
    
    private static bool GetImplementIChangeTracking(ISymbol? classSymbol)
    {
        var attributeData = classSymbol?.GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "MvvmHelper.Attributes.GenerateViewModelAttribute");
        // Default value
        bool useChangeTracking = false;

        if (attributeData != null)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                if (namedArg.Key == "UseChangeTracking" && namedArg.Value.Value is bool boolValue)
                {
                    useChangeTracking = boolValue;
                    break;
                }
            }
        }

        return useChangeTracking;
    }
    private static bool GetImplementIsDirty(ISymbol? classSymbol)
    {
        var attributeData = classSymbol?.GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "MvvmHelper.Attributes.GenerateViewModelAttribute");
        // Default value
        bool implementIsDirty = false;

        if (attributeData != null)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                if (namedArg.Key == "ImplementIsDirty" && namedArg.Value.Value is bool boolValue)
                {
                    implementIsDirty = boolValue;
                    break;
                }
            }
        }

        return implementIsDirty;
    }
    private string ImplementIsDirty(ClassInfo classInfo, string output)
    {
        string newOutput;
        if (classInfo.ImplementIsDirty)
        {
            var isDirtyExpression = classInfo.ImplementIChangeTracking ? "IsChanged" : "throw new NotImplementedException(\"Please Set UseChangeTracking=True in GenerateViewModel Attribute.\")";
            var isDirtyTemplate = IsDirtyTemplate.Replace("{{IsDirtyExpression}}", isDirtyExpression);
            newOutput = output.Replace("{{IsDirtyImplementation}}", isDirtyTemplate);
        }
        else
        {
            newOutput = output.Replace("{{IsDirtyImplementation}}", string.Empty);
        }
        return newOutput;
    }
    private void GenerateIChangeTrackingFile(SourceProductionContext context, ClassInfo classInfo)
    {
        if(!classInfo.ImplementIChangeTracking) return;
        var output = IChangeTrackingTemplate
            .Replace("{{Namespace}}", classInfo.Namespace ?? "GeneratedNamespace")
            .Replace("{{className}}", classInfo.Name ?? "GeneratedClass")
            .Replace("{{Accessability}}", GetAccessibilityModifier(classInfo.Accessibility));
        output = ImplementRejectChanges(classInfo, output);
        output = ImplementIsDirty(classInfo, output);
        output = ImplementPropertyIsChanged(classInfo, output);
        output = ImplementPropertyOriginalValues(classInfo, output);
        context.AddSource($"{classInfo.Name}.IChangeTracking.g.cs", SourceText.From(output, System.Text.Encoding.UTF8));
    }
    private string ImplementPropertyOriginalValues(ClassInfo classInfo, string output)
    {
        var builder = new StringBuilder();
        foreach (var propertyInfo in classInfo.Properties)
        {
            var value = PropertyOriginalValuesTemplate
                .Replace("{{PropertyName}}", propertyInfo.Name)
                .Replace("{{PropertyType}}", propertyInfo.Type);
            builder.AppendLine(value);
        }
        var originalValueTemplates = builder.ToString();
        return output.Replace("{{PropertyOriginalValuesImplementation}}", originalValueTemplates);
    }
    private string ImplementPropertyIsChanged(ClassInfo classInfo, string output)
    {
        var builder = new StringBuilder();
        foreach (var propertyInfo in classInfo.Properties)
        {
            var value = PropertyIsChangedTemplate.Replace("{{PropertyName}}", propertyInfo.Name);
            builder.AppendLine(value);
        }
        var propertyIsChangedImplementation = builder.ToString();
        return output.Replace("{{PropertyIsChangedImplementation}}", propertyIsChangedImplementation);
    }
    private string ImplementRejectChanges(ClassInfo classInfo, string output)
    {
        var builder = new StringBuilder();
        foreach (var propertyInfo in classInfo.Properties)
        {
            var value = RejectPropertyChangeTemplate.Replace("{{PropertyName}}", propertyInfo.Name)
                .Replace("{{PropertyType}}", propertyInfo.Type);
            builder.AppendLine(value);
        }
        var rejectChangesImplementation = builder.ToString();
        return output.Replace("{{RejectChangesImplementation}}", rejectChangesImplementation);
    }
}