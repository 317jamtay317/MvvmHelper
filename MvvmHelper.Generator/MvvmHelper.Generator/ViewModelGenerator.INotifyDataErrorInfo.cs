using System;

namespace MvvmHelper.Generator;

public partial class ViewModelGenerator
{
    private const string IDataErrorInfoInterfaceTemplate = ", INotifyDataErrorInfo";
    private const string IDataErrorInfoImplementationTemplate =
        """
            /// <summary>
            /// Validates the ViewModel. See Remarks...
            /// </summary>
            /// <remarks>
            /// If this is not set, no validation will be performed. This will be called in SetValue after the property is set.
            /// The property name will be passed as a parameter, and the function should return a list of errors for that property.
            /// </remarks>
            private Func<string, IEnumerable<string>> ValidateViewModel { get; set; }
            
            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
            
            public System.Collections.IEnumerable GetErrors(string propertyName)
            {
               var list = Errors[propertyName?? string.Empty];
                return list?.AsEnumerable()??[]; 
            }

            public bool HasErrors => Errors.Any();

            private Dictionary<string, List<string>> Errors { get; } = new();
        """;

    private const string ValidationCallTemplate =
        """
        if(ValidateViewModel is not null)
                {
                    Errors.Remove(propertyName);
                    var errors = ValidateViewModel(propertyName);
                    if(errors?.Any() ?? false)
                    {
                        Errors[propertyName] = errors?.ToList();
                    }
                }
        """;
    private static string ImplementIDataErrorInfoTemplate(ClassInfo classInfo, string currentClassText)
    {
        if (!classInfo.ImplementINotifyPropertyChanged) return currentClassText;
        if (!classInfo.ImplementIDataErrorInfo) return currentClassText;
        return currentClassText.Replace("{{IDataErrorInfo}}", IDataErrorInfoInterfaceTemplate)
            .Replace("{{IDataErrorInfoImplementation}}", IDataErrorInfoImplementationTemplate)
            .Replace("{{ValidationCall}}", ValidationCallTemplate);
    }
}