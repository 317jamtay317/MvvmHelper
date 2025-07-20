using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MvvmHelper.Generator;

public partial class ViewModelGenerator
{
    const string ViewModelCommandTemplate =
        """
        using System;
        using System.Windows.Input;
        namespace {{Namespace}};
        
        public partial class {{className}}
        {
            {{commands}}
        }
        """;

    private const string CommandTemplate =
         """
             private DelegateCommand{{generic}} _{{commandName}} = null;
             public ICommand {{commandName}}
             {
                get
                {
                    if(_{{commandName}} is null)
                    {
                        _{{commandName}} = new DelegateCommand{{generic}}({{executeMethod}}{{canExecuteMethod}});
                    }
                    return _{{commandName}};
                }
             }
         """;
    
    public static void GenerateCommandsFile(SourceProductionContext context, ClassInfo? classInfo)
    {
        if (classInfo is null || classInfo.Commands.Count == 0) return;
        var output = ViewModelCommandTemplate
            .Replace("{{Namespace}}", classInfo.Namespace ?? "GeneratedNamespace")
            .Replace("{{className}}", classInfo.Name ?? "GeneratedClass")
            .Replace("{{commands}}", string.Join("\n\n", classInfo.Commands.Select(GenerateCommand)));
        context.AddSource($"{classInfo.Name}.Commands.g.cs", SourceText.From(output, System.Text.Encoding.UTF8));
    }

    private static string GenerateCommand(CommandInfo commandInfo)
    {
        var generic = commandInfo.IsGeneric ? $"<{commandInfo.ParameterType}>" : string.Empty;
        var executeMethod = commandInfo.IsGeneric ? $"(param) => {commandInfo.Name?.Replace("Command", "")}(param)" : $"{commandInfo.Name?.Replace("Command", "")}";
        var canExecuteMethod = commandInfo.CanExecuteMethod is not null
            ? commandInfo.IsGeneric
                ? $", (param) => {commandInfo.CanExecuteMethod}(param)"
                : $", () => {commandInfo.CanExecuteMethod}()"
            : "";
        return CommandTemplate
            .Replace("{{commandName}}", commandInfo.Name ?? "UnknownCommand")
            .Replace("{{generic}}", generic)
            .Replace("{{executeMethod}}", executeMethod)
            .Replace("{{canExecuteMethod}}", canExecuteMethod);
    }

    private static List<CommandInfo> GetCommands(ISymbol? classSymbol)
    {
        List<CommandInfo> commands = [];
        if (classSymbol is not INamedTypeSymbol namedTypeSymbol) return commands;
        var methods = namedTypeSymbol.GetMembers().OfType<IMethodSymbol>().OrderBy(x=>x.Name);
        foreach (var method in methods)
        {
            foreach (var attribute in method.GetAttributes())
            {
                if(attribute.AttributeClass?.Name != "GenerateCommandAttribute") continue;
                var commandInfo = new CommandInfo
                {
                    Name = method.Name + "Command",
                    IsGeneric = method.Parameters.Length == 1,
                    ParameterType = method.Parameters.Length == 1 ? method.Parameters[0].Type.ToDisplayString() : null,
                    CanExecuteMethod = attribute.NamedArguments.FirstOrDefault(kv => kv.Key == "CanExecuteMethod").Value.Value as string
                };
                commands.Add(commandInfo);
            }
        }
        return commands;
    }
}

public record CommandInfo
{
    public string? Name { get; set; }
    public string? CanExecuteMethod { get; set; }
    public bool IsGeneric { get; set; }
    public string? ParameterType { get; set; }
}