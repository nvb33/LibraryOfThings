using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Items Near Me page, allowing users to search for
/// available rental items within a configurable radius of their current location.
/// </summary>
public partial class NearbyItemsViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly ILocationService _locationService;

    /// <summary>Gets or sets the collection of nearby items returned by the search.</summary>
    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    /// <summary>Gets or sets a value indicating whether a search is in progress.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets a value indicating whether the search returned no results.</summary>
    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>Gets or sets the error message to display when a search fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>Gets or sets the search radius in kilometres. Defaults to 5km.</summary>
    [ObservableProperty]
    private double _radiusKm = 5.0;

    /// <summary>Gets or sets the status message displayed during and after a search.</summary>
    [ObservableProperty]
    private string _locationStatus = "Tap 'Search' to find items near you";

    /// <summary>
    /// Initialises a new instance of <see cref="NearbyItemsViewModel"/>.
    /// </summary>
    /// <param name="itemRepository">The repository used to retrieve nearby items.</param>
    /// <param name="locationService">The location service used to obtain device coordinates.</param>
    public NearbyItemsViewModel(IItemRepository itemRepository, ILocationService locationService)
    {
        _itemRepository = itemRepository;
        _locationService = locationService;
    }

    /// <summary>
    /// Retrieves the device's current location and searches for items within
    /// the configured radius. Updates the Items collection with results.
    /// </summary>
    [RelayCommand]
    private async Task SearchNearbyAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        IsEmpty = false;
        ErrorMessage = string.Empty;

        LocationStatus = "Step 1: Command fired";
        await Task.Delay(500);

        try
        {
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

            var items = await _itemRepository.GetNearbyAsync(lat, lon, RadiusKm);
            LocationStatus = $"Step 4: Repository returned {items.Count()} items";
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

    /// <summary>
    /// Navigates to the Item Detail page for the selected item.
    /// </summary>
    /// <param name="item">The item to view in detail.</param>
    [RelayCommand]
    private async Task NavigateToDetailAsync(Item item)
    {
        await Shell.Current.GoToAsync($"itemdetail?id={item.Id}");
    }
}