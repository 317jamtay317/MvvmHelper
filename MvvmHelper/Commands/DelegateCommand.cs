using System;
using System.Windows.Input;

namespace MvvmHelper.Commands;

public abstract class DelegateCommandBase : ICommand
{
    public abstract bool CanExecute(object? parameter);
        
    public abstract void Execute(object? parameter);
        
    public event EventHandler? CanExecuteChanged;
            
    public void RaiseCanExecuteChanged() => 
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
        
public class DelegateCommand(Action execute, Func<bool> canExecute) : DelegateCommandBase
{
    public DelegateCommand(Action execute) : this(execute, () => true)
    {
    }
        
    public override bool CanExecute(object? parameter) =>
        canExecute();
        
    public override void Execute(object? parameter)
    {
        if (!CanExecute(parameter)) return;
        execute();
    }
}
        
public class DelegateCommand<T>(Action<T> execute, Func<T, bool> canExecute) : DelegateCommandBase
{
    public DelegateCommand(Action<T> execute): this(execute, _ => true) { }

    public override bool CanExecute(object? parameter)
    {
        if (parameter is T t)
        {
            return canExecute(t);
        }
#pragma warning disable CS8604 // Possible null reference argument.
        return canExecute(default);
#pragma warning restore CS8604 // Possible null reference argument.
    }
        
    public override void Execute(object? parameter)
    {
        if (!CanExecute((T)parameter!)) return;
        if (parameter is T t)
        {
            execute(t);
            return;
        }
#pragma warning disable CS8604 // Possible null reference argument.
        execute(default);        
#pragma warning restore CS8604 // Possible null reference argument.
    }
}