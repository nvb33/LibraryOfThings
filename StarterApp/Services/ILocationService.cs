namespace StarterApp.Services;

/// <summary>
/// Defines the contract for retrieving the device's current geographic location.
/// </summary>
public interface ILocationService
{
    /// <summary>
    /// Retrieves the device's current GPS coordinates.
    /// </summary>
    /// <returns>
    /// A tuple containing Latitude and Longitude if location is available,
    /// or null if the location cannot be determined due to permissions,
    /// hardware unavailability, or timeout.
    /// </returns>
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
}