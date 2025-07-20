using System;
using System.ComponentModel.Design;
using MvvmHelper.Generator.Sample;
using Shouldly;
using Xunit;

namespace MvvmHelper.Generator.Tests.ViewModels;

public class DelegateCommandTests
{
    [Fact]
    public void Execute_ShouldCallTheMethod_WhenCalled()
    {
        //arrange
        bool fired = false;
        Action action = () => fired = true;
        var command = new DelegateCommand(action);
        
        //act
        command.Execute(null);
        
        //assert
        fired.ShouldBeTrue();
    }

    [Fact]
    public void CanExecute_ShouldAllowExecute_WhenTrue()
    {
        //arrange
        bool fired = false;
        Func<bool> canExecute = () => true;
        Action action = () => fired = true;
        var command = new DelegateCommand(action, canExecute);
        
        //act
        var result =  command.CanExecute(null);  
        //assert
        result.ShouldBeTrue();
        command.Execute(null);
        fired.ShouldBeTrue();
    }

    [Fact] 
    public void CanExecute_ShouldNotAllowExecute_WhenFalse()
    {
        //arrange
        var fired = false;
        Func<bool> canExecute = () => false;
        Action action = () => fired = true;
        var command = new DelegateCommand(action, canExecute);
        
        //act
        var result =  command.CanExecute(null);
        
        //assert
        command.Execute(null);
        result.ShouldBeFalse();
        fired.ShouldBeFalse();
    }
    private record Person(string FirstName, string LastName);

    [Fact]
    public void Execute_ShouldPassParameter_WhenCalled()
    {
        //arrange
        var personA = new Person("First", "Last");
        var personB = new Person("First", "Last");
        var correctParameter = false;
        Action<Person> action = (p) => correctParameter = p.Equals(personB);
        var command = new DelegateCommand<Person>(action);
        
        //act
        command.Execute(personA);

        //assert
        correctParameter.ShouldBeTrue();
    }

    [Fact]
    public void Execute_ShouldAllowNulls_WhenCalled()
    {
        //arrange
        bool fired = false;
        Person? person = null;
        Action<Person?> action = (p) => fired = true;
        var command = new DelegateCommand<Person>(action);
        
        //act
        command.Execute(person);
        
        //assert
        fired.ShouldBeTrue();
    }

    [Fact]
    public void RaiseCanExecuteChanged_ShouldFireCanExecuteChanged_WhenCalled()
    {
        //arrange
        var canExecuteChanged = false;
        var command = new DelegateCommand(() => { }, () => true);
        command.CanExecuteChanged += (s, e) => canExecuteChanged = true;
        
        //act
        command.RaiseCanExecuteChanged();
        
        //assert
        canExecuteChanged.ShouldBeTrue();
    }
}