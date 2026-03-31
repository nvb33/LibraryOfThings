namespace StarterApp.Services;

public class LocationService : ILocationService
{
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