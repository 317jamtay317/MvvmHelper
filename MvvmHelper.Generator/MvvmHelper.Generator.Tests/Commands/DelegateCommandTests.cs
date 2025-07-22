using System;
using System.Windows.Input;
using MvvmHelper.Commands;
using Xunit;

namespace MvvmHelper.Generator.Tests.Commands;

public class DelegateCommandTests
{
    [Fact]
    public void DelegateCommand_Execute_CallsAction()
    {
        bool executed = false;
        var command = new DelegateCommand(() => executed = true);

        command.Execute(null);

        Assert.True(executed);
    }

    [Fact]
    public void DelegateCommand_CanExecute_ReturnsFalse_WhenCanExecuteIsFalse()
    {
        var command = new DelegateCommand(() => { }, () => false);

        Assert.False(command.CanExecute(null));
    }

    [Fact]
    public void DelegateCommand_Execute_DoesNotCallAction_WhenCanExecuteIsFalse()
    {
        bool executed = false;
        var command = new DelegateCommand(() => executed = true, () => false);

        command.Execute(null);

        Assert.False(executed);
    }

    [Fact]
    public void DelegateCommand_RaiseCanExecuteChanged_RaisesEvent()
    {
        var command = new DelegateCommand(() => { });
        bool eventRaised = false;
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        command.RaiseCanExecuteChanged();

        Assert.True(eventRaised);
    }

    [Fact]
    public void DelegateCommandT_Execute_CallsAction()
    {
        int result = 0;
        var command = new DelegateCommand<int>(x => result = x);

        command.Execute(42);

        Assert.Equal(42, result);
    }

    [Fact]
    public void DelegateCommandT_CanExecute_ReturnsFalse_WhenCanExecuteIsFalse()
    {
        var command = new DelegateCommand<int>(x => { }, x => false);

        Assert.False(command.CanExecute(1));
    }

    [Fact]
    public void DelegateCommandT_Execute_DoesNotCallAction_WhenCanExecuteIsFalse()
    {
        bool executed = false;
        var command = new DelegateCommand<int>(x => executed = true, x => false);

        command.Execute(1);

        Assert.False(executed);
    }

    [Fact]
    public void DelegateCommandT_RaiseCanExecuteChanged_RaisesEvent()
    {
        var command = new DelegateCommand<int>(x => { });
        bool eventRaised = false;
        command.CanExecuteChanged += (s, e) => eventRaised = true;

        command.RaiseCanExecuteChanged();

        Assert.True(eventRaised);
    }
}