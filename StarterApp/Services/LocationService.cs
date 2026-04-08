namespace StarterApp.Services;

/// <summary>
/// Provides the device's current geographic location using the MAUI Geolocation API.
/// Currently returns hardcoded Edinburgh coordinates for testing purposes.
/// </summary>
public class LocationService : ILocationService
{
    /// <summary>
    /// Retrieves the device's current GPS coordinates.
    /// </summary>
    /// <remarks>
    /// Currently returns hardcoded Edinburgh coordinates (55.9533, -3.1883) for testing.
    /// The full GPS implementation using Geolocation.GetLocationAsync is available
    /// in the commented-out code below and should be enabled before production use.
    /// </remarks>
    /// <returns>
    /// A tuple containing Latitude and Longitude, or null if location is unavailable.
    /// </returns>
    public async Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync()
    {
        // Temporary: hardcoded Edinburgh coordinates for emulator testing
        // TODO: Remove this and uncomment the GPS code below before submission
        await Task.CompletedTask;
        return (55.9533, -3.1883);

        // GPS implementation
        /*
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location == null)
                return null;

            return (location.Latitude, location.Longitude);
        }
        catch (FeatureNotSupportedException)
        {
            System.Diagnostics.Debug.WriteLine("GPS not supported on this device");
            return null;
        }
        catch (PermissionException)
        {
            System.Diagnostics.Debug.WriteLine("Location permission denied");
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Location error: {ex.Message}");
            return null;
        }
        */
    }
}