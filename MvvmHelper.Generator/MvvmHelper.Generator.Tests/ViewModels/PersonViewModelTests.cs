using System;
using System.Linq;
using MvvmHelper.Generator.Sample.ViewModels;
using Shouldly;
using Xunit;

namespace MvvmHelper.Generator.Tests.ViewModels;

public class PersonViewModelTests
{
    [Fact]
    public void LoadedCommand_ShouldSetIsLoaded_WhenCalled()
    {
        //arrange

        //act
        _sut.LoadedCommand.Execute(null);
        
        //assert
        _sut.IsLoaded.ShouldBeTrue();
    }

    [Fact]
    public void Name_ShouldSetValue_WhenSet()
    {
        //arrange

        //act
        _sut.Name = "John";
        
        //assert
        _sut.Name.ShouldBe("John");
    }

    [Fact]
    public void Name_ShouldCallOnPropertyChanged_WhenSet()
    {
        //arrange
        bool fired = false;
        _sut.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_sut.Name))
            {
                fired = true;
            }
        };
        
        //act
        _sut.Name = "John";
        
        //assert
        fired.ShouldBeTrue();
    }

    [Fact]
    public void Name_ShouldNotCallOnPropertyChanged_WhenValueDoesntChange()
    {
        //arrange
        bool fired = false;
        _sut.Name = "John";
        _sut.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_sut.Name))
            {
                fired = true;
            }
        };
        
        //act
        _sut.Name = "John";
        
        //assert
        fired.ShouldBeFalse();
    }

    [Fact]
    public void HasErrors_ShouldHaveError_WhenPropertyNameIsLongerThan10()
    {
        //arrange
        
        //act
        _sut.Name = new string('a', 11);

        //assert
        _sut.HasErrors.ShouldBeTrue();
    }

    [Fact]
    public void GetErrors_ShouldReturnCorrectErrorList_WhenInvalid()
    {
        //arrange
        _sut.Name = new string('a', 11);

        //act
        var list = _sut.GetErrors(nameof(_sut.Name));
        
        //assert
        list.ShouldNotBeNull();
        list.Cast<string>().ShouldContain("Name cannot be longer than 10 characters");
    }

    [Fact]
    public void UpdateLastUpdateTime_ShouldUpdateTheLastUpdatedTime_WhenValid()
    {
        //arrange
        var newTime = DateTime.Now;
        
        //act
        _sut.UpdateTimeCommand.Execute(newTime);
        
        //assert
        _sut.LastTimeUpdated.ShouldBe(newTime);
    }

    [Fact]
    public void UpdateTime_ShouldNotUpdateTime_WhenTimeIsLessThanAlreadyUpdated()
    {
        //arrange
        var now = DateTime.Now;
        _sut.UpdateTimeCommand.Execute(now);
        var lastTime = now.AddDays(-1);
        
        //act
        _sut.UpdateTimeCommand.Execute(lastTime);
        
        //assert
        _sut.LastTimeUpdated.ShouldBe(now);
    }
    
    private readonly PersonViewModel _sut = new();
}