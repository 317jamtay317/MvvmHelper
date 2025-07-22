namespace MvvmHelper.Interfaces;

public interface IValueTracking
{
    void TrackValue(string propertyName, object? value);
}