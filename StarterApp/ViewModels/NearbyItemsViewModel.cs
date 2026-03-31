using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class NearbyItemsViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly ILocationService _locationService;

    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private double _radiusKm = 5.0;

    [ObservableProperty]
    private string _locationStatus = "Tap 'Search' to find items near you";

    public NearbyItemsViewModel(IApiService apiService, ILocationService locationService)
    {
        _apiService = apiService;
        _locationService = locationService;
    }

    [RelayCommand]
    private async Task SearchNearbyAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        IsEmpty = false;
        ErrorMessage = string.Empty;

        // Step 1 — confirm the command is firing
        LocationStatus = "Step 1: Command fired";
        await Task.Delay(500); // brief pause so we can see it

        try
        {
            // Step 2 — confirm location service is called
            LocationStatus = "Step 2: Getting location...";
            var location = await _locationService.GetCurrentLocationAsync();

            if (location == null)
            {
                ErrorMessage = "Location returned null";
                LocationStatus = "Location is null";
                return;
            }

            var (lat, lon) = location.Value;
            LocationStatus = $"Step 3: Got location {lat:F4},{lon:F4}. Calling API...";
            await Task.Delay(500);

            // Step 3 — call the API
            var items = await _apiService.GetNearbyItemsAsync(lat, lon, RadiusKm);
            LocationStatus = $"Step 4: API returned {items.Count} items";
            await Task.Delay(500);

            Items = new ObservableCollection<Item>(items);
            IsEmpty = !Items.Any();

            LocationStatus = IsEmpty
                ? $"No items found within {RadiusKm:F0}km"
                : $"Found {Items.Count} item(s) within {RadiusKm:F0}km";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            LocationStatus = $"Exception: {ex.GetType().Name}";
            System.Diagnostics.Debug.WriteLine($"Full error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(Item item)
    {
        await Shell.Current.GoToAsync($"itemdetail?id={item.Id}");
    }
}