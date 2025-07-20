using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MvvmHelper.Generator;

public partial class ViewModelGenerator
{
    private const string AttributesTemplate =
        """
        using System;

        namespace MvvmHelper.Generator
        {
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
            }
            
            [AttributeUsage(AttributeTargets.Method)]
            public class GenerateCommandAttribute : Attribute
            {
                public string? CanExecuteMethod { get; set; }
            }
        }
        """;
    private void AddAttributes(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx=>
            ctx.AddSource("ViewModelAttributes.g.cs",
                SourceText.From(AttributesTemplate, Encoding.UTF8)));
    }
}