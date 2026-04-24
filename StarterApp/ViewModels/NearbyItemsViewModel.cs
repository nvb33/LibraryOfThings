using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Items Near Me page, allowing users to search for
/// available rental items within a configurable radius with category
/// filter and title search.
/// </summary>
public partial class NearbyItemsViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly ILocationService _locationService;
    private List<Item> _allNearbyItems = new();

    /// <summary>Gets or sets the filtered collection of nearby items.</summary>
    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    /// <summary>Gets or sets the list of categories available for filtering.</summary>
    [ObservableProperty]
    private ObservableCollection<string> _categoryFilters = new();

    /// <summary>Gets or sets the currently selected category filter.</summary>
    [ObservableProperty]
    private string _selectedCategoryFilter = "All categories";

    /// <summary>Gets or sets the current search query for filtering by title.</summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

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

    /// <summary>Gets or sets a value indicating whether search results are available to filter.</summary>
    [ObservableProperty]
    private bool _hasResults;

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

    /// <summary>Called automatically when SelectedCategoryFilter changes.</summary>
    partial void OnSelectedCategoryFilterChanged(string value) => ApplyFilters();

    /// <summary>Called automatically when SearchQuery changes.</summary>
    partial void OnSearchQueryChanged(string value) => ApplyFilters();

    /// <summary>
    /// Retrieves the device's current location and searches for items within
    /// the configured radius. Populates category filter from results.
    /// </summary>
    [RelayCommand]
    private async Task SearchNearbyAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        IsEmpty = false;
        ErrorMessage = string.Empty;
        LocationStatus = "Getting location...";

        try
        {
            var location = await _locationService.GetCurrentLocationAsync();

            if (location == null)
            {
                ErrorMessage = "Could not determine your location.";
                LocationStatus = "Location unavailable";
                return;
            }

            var (lat, lon) = location.Value;
            LocationStatus = "Searching nearby items...";

            var items = await _itemRepository.GetNearbyAsync(lat, lon, RadiusKm);
            _allNearbyItems = items.ToList();
            HasResults = _allNearbyItems.Count > 0;

            // Build category filter dynamically from results
            var categories = _allNearbyItems
                .Select(i => i.CategoryName)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            CategoryFilters = new ObservableCollection<string>(
                new[] { "All categories" }.Concat(categories));

            SelectedCategoryFilter = "All categories";
            SearchQuery = string.Empty;
            ApplyFilters();

            LocationStatus = _allNearbyItems.Count == 0
                ? $"No items found within {RadiusKm:F0}km"
                : $"Found {_allNearbyItems.Count} item(s) within {RadiusKm:F0}km";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
            LocationStatus = "Search failed";
            System.Diagnostics.Debug.WriteLine($"Full error: {ex}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Applies the current search query and category filter to the nearby items.
    /// </summary>
    private void ApplyFilters()
    {
        var filtered = _allNearbyItems.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchQuery))
            filtered = filtered.Where(i =>
                i.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        if (SelectedCategoryFilter != "All categories")
            filtered = filtered.Where(i => i.CategoryName == SelectedCategoryFilter);

        Items = new ObservableCollection<Item>(filtered);
        IsEmpty = !Items.Any();
        HasResults = _allNearbyItems.Count > 0;
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