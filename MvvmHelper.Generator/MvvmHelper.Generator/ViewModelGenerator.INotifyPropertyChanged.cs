using System.Linq;
using Microsoft.CodeAnalysis;

namespace MvvmHelper.Generator;

public partial class ViewModelGenerator
{
    private const string INotifyPropertyChangedInterfaceTemplate = ": INotifyPropertyChanged";

    private const string INotifyPropertyChangedImplementationTemplate =
        """
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private T GetValue<T>([CallerMemberName] string propertyName = null)
            {
                if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));
                if (_propertiesValues.TryGetValue(propertyName, out var value))
                {
                    return (T)value;
                }
                return default!;
            }
         
            private bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
            {
                if (propertyName is null) throw new ArgumentNullException(nameof(propertyName));
                if (_propertiesValues.TryGetValue(propertyName, out var oldValue))
                {
                    if (Equals(oldValue, value)) return false;
                    _propertiesValues[propertyName] = value!;
                }
                else
                {
                    _propertiesValues.Add(propertyName, value!);
                }
                {{ValidationCall}}
                OnPropertyChanged(propertyName);
                {{ImplementINotifyDataErrorInfoRaiseErrorsChanged}}
                return true;
            }
            private Dictionary<string, object> _propertiesValues = new();
        """;


    public static string ImplementINotifyPropertyChangedTemplate(ClassInfo classInfo, string currentClassText)
    {
        if (!classInfo.ImplementINotifyPropertyChanged)
        {
            return currentClassText.Replace("{{INotifyPropertyChanged}}", string.Empty)
                .Replace("{{INotifyPropertyChangedImplementation}}", string.Empty);
        }
        return currentClassText.Replace("{{INotifyPropertyChanged}}", INotifyPropertyChangedInterfaceTemplate)
            .Replace("{{INotifyPropertyChangedImplementation}}", INotifyPropertyChangedImplementationTemplate);
    }
    
    private static bool GetImplementINotifyPropertyChanged(ISymbol? classSymbol)
    {
        var attributeData = classSymbol?.GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "MvvmHelper.Generator.GenerateViewModelAttribute");
        // Default value
        bool implementINotify = true;

        if (attributeData != null)
        {
            foreach (var namedArg in attributeData.NamedArguments)
            {
                if (namedArg.Key == "ImplementINotifyPropertyChanged" && namedArg.Value.Value is bool boolValue)
                {
                    implementINotify = boolValue;
                    break;
                }
            }
        }

        return implementINotify;
    }

}