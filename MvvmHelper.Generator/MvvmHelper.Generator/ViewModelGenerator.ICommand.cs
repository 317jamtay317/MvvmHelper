using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MvvmHelper.Generator;

public partial class ViewModelGenerator
{
    private const string CommandsTemplate =
        """
        using System;
        using System.Windows.Input;
        
        namespace MvvmHelper.Generator.Sample;
        
        public abstract class DelegateCommandBase : ICommand
        {
            public abstract bool CanExecute(object parameter);
        
            public abstract void Execute(object parameter);
        
            public event EventHandler CanExecuteChanged;
            
            public void RaiseCanExecuteChanged() => 
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        
        public class DelegateCommand : DelegateCommandBase
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;
            public DelegateCommand(Action execute, Func<bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }
        
            public DelegateCommand(Action execute) : this(execute, () => true)
            {
            }
        
            public override bool CanExecute(object parameter) =>
                _canExecute();
        
            public override void Execute(object parameter)
            {
                if (!CanExecute(parameter)) return;
                _execute();
            }
        }
        
        public class DelegateCommand<T> : DelegateCommandBase
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool> _canExecute;
        
            public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
            {
                _execute = execute;
                _canExecute = canExecute;
            }
            public DelegateCommand(Action<T> execute): this(execute, _ => true) { }
        
            public override bool CanExecute(object parameter) =>
                _canExecute((T)parameter);
        
            public override void Execute(object parameter)
            {
                if (!CanExecute((T)parameter)) return;
                _execute((T)parameter);
            }
        }
        """;
    public void AddCommands(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx=>
            ctx.AddSource("DelegateCommand.g.cs",
                SourceText.From(CommandsTemplate, Encoding.UTF8)));
    }
}