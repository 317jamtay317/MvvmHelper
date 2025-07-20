using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MvvmHelper.Generator;

[Generator]
public partial class ViewModelGenerator : IIncrementalGenerator
{
    private const string ViewModelClassTemplate =
        """
        using System;
        using System.Linq;
        using System.Runtime.CompilerServices;
        using System.Collections.Generic;
        using System.ComponentModel;
        namespace {{Namespace}};

        {{Accessibility}} partial class {{className}} {{INotifyPropertyChanged}}{{IDataErrorInfo}}
        {
            {{INotifyPropertyChangedImplementation}}
            {{IDataErrorInfoImplementation}}
        }
        """;
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        AddAttributes(context);
        AddCommands(context);
        var provider = context.SyntaxProvider.CreateSyntaxProvider((node, _) => 
                ViewModelPredicate(node),
                TransformViewModel)
            .Where(x=> x is not null);
        context.RegisterSourceOutput(provider, GenerateViewModel);
    }

    private void GenerateViewModel(SourceProductionContext context, ClassInfo? classInfo)
    {
        if(classInfo is null) return;
        var output = ViewModelClassTemplate
            .Replace("{{Namespace}}", classInfo.Namespace ?? "GeneratedNamespace")
            .Replace("{{className}}", classInfo.Name ?? "GeneratedClass")
            .Replace("{{Accessibility}}", GetAccessibilityModifier(classInfo.Accessibility));
        output = ImplementINotifyPropertyChangedTemplate(classInfo, output);
        output = ImplementIDataErrorInfoTemplate(classInfo, output);
        context.AddSource($"{classInfo.Name}.g.cs", output);
        GenerateCommandsFile(context, classInfo);
    }

    private static ClassInfo? TransformViewModel(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax) syntaxContext.Node;
        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if(cancellationToken.IsCancellationRequested) return null;
                
                var attributeName = attributeSyntax.Name.ToString();
                if(attributeName != "GenerateViewModel" && attributeName != "GenerateViewModelAttribute") continue;
                
                var attributeSymbol = syntaxContext.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken);
                if(attributeSymbol.Symbol is not IMethodSymbol methodSymbol) continue;
                
                var attributeClass = methodSymbol.ContainingType;
                if(attributeClass.ToDisplayString() != "MvvmHelper.Generator.GenerateViewModelAttribute")continue;
                
                var classSymbol = syntaxContext.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken);
                var implementINotify = GetImplementINotifyPropertyChanged(classSymbol);
                var implementIDataErrorInfo = GetImplementIDataErrorInfo(classSymbol);
                var classInfo =  new ClassInfo
                {
                    Namespace = classSymbol?.ContainingNamespace.ToDisplayString(),
                    Name = classSymbol?.Name,
                    Accessibility = classSymbol?.DeclaredAccessibility,
                    Properties = GetProperties(classSymbol),
                    ImplementINotifyPropertyChanged = implementINotify,
                    ImplementIDataErrorInfo = implementIDataErrorInfo,
                    Commands = GetCommands(classSymbol)
                };
                return classInfo;
            }
        }

        return null;
    }

    private static bool GetImplementIDataErrorInfo(ISymbol? classSymbol)
    {
        var attributeData = classSymbol?.GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "MvvmHelper.Generator.GenerateViewModelAttribute");
        // Default value
        bool implementIDataErrorInfo = false;

        if (attributeData != null)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                if (namedArg.Key == "UseIDataErrorInfo" && namedArg.Value.Value is bool boolValue)
                {
                    implementIDataErrorInfo = boolValue;
                    break;
                }
            }
        }

        return implementIDataErrorInfo;
    }

    private static List<PropertyInfo> GetProperties(ISymbol? classSymbol)
    {
        List<PropertyInfo> properties = [];
        if (classSymbol is not INamedTypeSymbol namedTypeSymbol) return properties;
        foreach (var member in namedTypeSymbol.GetMembers().OrderBy(x=>x.Name))
        {
            if (member is not IPropertySymbol propertySymbol) continue;
            var prop = new PropertyInfo
            {
                Name = propertySymbol.Name,
                Type = propertySymbol.Type.ToDisplayString(),
                Accessibility = propertySymbol.DeclaredAccessibility
            };
            properties.Add(prop);
        }
        return properties;
    }

    private static bool ViewModelPredicate(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax{AttributeLists.Count: > 0};
    }
    private static string GetAccessibilityModifier(Accessibility? accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.ProtectedOrInternal => "private protected",
            _ => "public"
        };
    }
}