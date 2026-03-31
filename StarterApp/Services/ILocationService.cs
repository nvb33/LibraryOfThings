namespace StarterApp.Services;

public interface ILocationService
{
    // Returns the device's current GPS coordinates
    // Returns null if location cannot be determined
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
}