﻿using System;
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
        if (classInfo.ImplementIsDirty)
        {
            var isDirtyExpression = classInfo.ImplementIChangeTracking
                ? "IsChanged"
                : "throw new NotImplementedException(\"Please Set UseChangeTracking=True in GenerateViewModel Attribute.\")";

            var isDirtyTemplate = Indent(IsDirtyTemplate.Replace("{{IsDirtyExpression}}", isDirtyExpression), 1);
            return output.Replace("{{IsDirtyImplementation}}", isDirtyTemplate);
        }

        return output.Replace("{{IsDirtyImplementation}}", string.Empty);
    }

    private void GenerateIChangeTrackingFile(SourceProductionContext context, ClassInfo classInfo)
    {
        if (!classInfo.ImplementIChangeTracking) return;

        var output = IChangeTrackingTemplate
            .Replace("{{Namespace}}", classInfo.Namespace ?? "GeneratedNamespace")
            .Replace("{{className}}", classInfo.Name ?? "GeneratedClass")
            .Replace("{{Accessability}}", GetAccessibilityModifier(classInfo.Accessibility));

        output = ImplementRejectChanges(classInfo, output);
        output = ImplementIsDirty(classInfo, output);
        output = ImplementPropertyIsChanged(classInfo, output);
        output = ImplementPropertyOriginalValues(classInfo, output);

        context.AddSource(
            $"{classInfo.Name}.IChangeTracking.g.cs",
            SourceText.From(output.TrimEnd().Replace("\r\n", "\n").Replace("\r", "\n"), Encoding.UTF8)
        );
    }

    private string ImplementPropertyOriginalValues(ClassInfo classInfo, string output)
    {
        var builder = new StringBuilder();
        foreach (var property in classInfo.Properties)
        {
            var block = PropertyOriginalValuesTemplate
                .Replace("{{PropertyName}}", property.Name)
                .Replace("{{PropertyType}}", property.Type);

            builder.AppendLine(Indent(block, 1));
        }

        return output.Replace("{{PropertyOriginalValuesImplementation}}", builder.ToString().TrimEnd());
    }

    private string ImplementPropertyIsChanged(ClassInfo classInfo, string output)
    {
        var builder = new StringBuilder();
        foreach (var property in classInfo.Properties)
        {
            var line = PropertyIsChangedTemplate
                .Replace("{{PropertyName}}", property.Name);

            builder.AppendLine(Indent(line, 1));
        }

        return output.Replace("{{PropertyIsChangedImplementation}}", builder.ToString().TrimEnd());
    }

    private string ImplementRejectChanges(ClassInfo classInfo, string output)
    {
        var builder = new StringBuilder();
        foreach (var property in classInfo.Properties)
        {
            var block = RejectPropertyChangeTemplate
                .Replace("{{PropertyName}}", property.Name)
                .Replace("{{PropertyType}}", property.Type);

            builder.AppendLine(Indent(block, 2)); // 2 levels inside method
        }

        return output.Replace("{{RejectChangesImplementation}}", builder.ToString().TrimEnd());
    }

    private string Indent(string text, int level = 1)
    {
        var pad = new string(' ', level * 4);
        return string.Join("\r\n",
            text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                .Select(line => string.IsNullOrWhiteSpace(line) ? line : pad + line));
    }
}
