using MvvmHelper.Generator.Sample;
using Shouldly;
using Xunit;

namespace MvvmHelper.Generator.Tests.ViewModels;

public class PersonViewModelTests
{
    [Fact]
    public void LoadedCommand_ShouldSetIsLoaded_ToTrue()
    {
        //arrange
        
        //act
        _sut.LoadedCommand.Execute(null);
        
        //assert
        _sut.IsLoaded.ShouldBeTrue();
    }

    [Fact]
    public void IsValid_ShouldBeFalse_WhenNameIsToLong()
    {
        //arrange

        //act
        _sut.FirstName = new string('a', 51);
        
        //assert
        _sut.IsValid.ShouldBeFalse();
    }

    [Fact]
    public void IsChanged_ShouldBeSetToTrue_WhenNameChanges()
    {
        //arrange
        
        //act
        _sut.FirstName = "Test Name";
        
        //assert
        _sut.IsChanged.ShouldBeTrue();
    }

    [Fact]
    public void IsChanged_ShouldBeSetToTrue_WhenIsLoadedChanges()
    {
        //arrange

        //act
        _sut.IsLoaded = true;
        
        //assert
        _sut.IsChanged.ShouldBeTrue();
    }

    [Fact]
    public void FirstNameOriginalValue_ShouldBeCorrect_WhenValueChanges()
    {
        //arrange
        const string originalValue = "Original Name";;
        _sut.FirstName = originalValue;
        _sut.AcceptChanges();
        
        //act
        _sut.FirstName = "New Name";
        
        //assert
        _sut.FirstNameOriginalValue.ShouldBe(originalValue);
    }

    [Fact]
    public void AcceptChanges_ShouldSetIsChangedToFalse_WhenCalled()
    {
        //arrange
        _sut.FirstName = "Test Name";
        _sut.IsChanged.ShouldBeTrue();
        
        //act
        _sut.AcceptChanges();
        
        //assert
        _sut.IsChanged.ShouldBeFalse();
    }

    [Fact]
    public void RejectChanges_ShouldSetTheChangesBackToOriginalValue_WhenHasChanges()
    {
        //arrange
        const string originalValue = "Original Name";
        _sut.FirstName = originalValue;
        _sut.AcceptChanges();
        
        //act
        _sut.FirstName = "New Name";
        _sut.RejectChanges();
        
        //assert
        _sut.FirstName.ShouldBe(originalValue);
        _sut.IsChanged.ShouldBeFalse();
    }

    [Fact]
    public void FirstNameIsChanged_ShouldBeTrue_WhenFirstNameChanges()
    {
        //arrange
        
        //act
        _sut.FirstName = "Test Name";
        
        //assert
        _sut.FirstNameIsChanged.ShouldBeTrue();
    }

    [Fact]
    public void FirstNameIsChanged_ShouldCallPropertyChanged_WhenFirstNameChangeTheFirstTime()
    {
        //arrange
        const string originalValue = "Original Name";
        _sut.FirstName = originalValue;
        _sut.AcceptChanges();
        var correctlyCalled = false;
        _sut.PropertyChanged += (_, e)=> correctlyCalled = (e.PropertyName == nameof(PersonViewModel.FirstNameIsChanged) || correctlyCalled);
        
        //act
        _sut.FirstName = "New Name";
        
        //assert
        correctlyCalled.ShouldBeTrue();
    }

    [Fact]
    public void FirstNameOriginalValue_ShouldCallPropertyChanged_WhenFirstNameChanges()
    {
        //arrange
        const string originalValue = "Original Name";
        _sut.FirstName = originalValue;
        _sut.AcceptChanges();
        var correctlyCalled = false;
        _sut.PropertyChanged += (_, e)=> correctlyCalled = (e.PropertyName == nameof(PersonViewModel.FirstNameOriginalValue) || correctlyCalled);

        //act
        _sut.FirstName = "New Name";
        
        //assert
        correctlyCalled.ShouldBeTrue();
    }

    [Fact]
    public void IsDirty_ShouldBeTrue_WhenValuesChanged()
    {
        //arrange

        //act
        _sut.FirstName = "Test Name";
        
        //assert
        _sut.IsDirty.ShouldBeTrue();
    }

    [Fact]
    public void IsDirty_ShouldBeFalse_WhenValuesHaveNotChanged()
    {
        //arrange

        //act
        
        //assert
        _sut.IsDirty.ShouldBeFalse();
    }

    private readonly PersonViewModel _sut = new();
}