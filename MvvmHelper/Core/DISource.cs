using System.Windows.Markup;

namespace MvvmHelper.Core;

public class DiSource(Type type) : MarkupExtension
{
    public static IServiceProvider? ServiceProvider { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(ServiceProvider);
        return ServiceProvider.GetService(type);
    }
}