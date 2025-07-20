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
    public class GenerateCommandAttribute : Attribute{}
            
    [AttributeUsage(AttributeTargets.Field)]
    public class GeneratePropertyAttribute : Attribute{}
}