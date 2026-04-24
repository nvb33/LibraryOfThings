using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Items List page, loading and displaying all available
/// rental items with search, owner filter and sort options.
/// All filtering and sorting is performed client-side on the loaded items.
/// </summary>
public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private List<Item> _allItems = new();

    /// <summary>Gets or sets the filtered and sorted collection of items displayed in the UI.</summary>
    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    /// <summary>Gets or sets the list of owner names available for filtering.</summary>
    [ObservableProperty]
    private ObservableCollection<string> _ownerFilters = new();

    /// <summary>Gets or sets the currently selected owner filter.</summary>
    [ObservableProperty]
    private string _selectedOwnerFilter = "All owners";

    /// <summary>Gets or sets the list of sort options available.</summary>
    [ObservableProperty]
    private ObservableCollection<string> _sortOptions = new()
    {
        "Default",
        "Price: Low to High",
        "Price: High to Low",
        "Rating: High to Low",
        "Newest first"
    };

    /// <summary>Gets or sets the currently selected sort option.</summary>
    [ObservableProperty]
    private string _selectedSortOption = "Default";

    /// <summary>Gets or sets the current search query for filtering by title.</summary>
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    /// <summary>Gets or sets a value indicating whether items are being loaded.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets a value indicating whether a pull-to-refresh is in progress.</summary>
    [ObservableProperty]
    private bool _isRefreshing;

    /// <summary>Gets or sets a value indicating whether the filtered list is empty.</summary>
    [ObservableProperty]
    private bool _isEmpty;

    /// <summary>Gets or sets the error message to display when loading fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Initialises a new instance of <see cref="ItemsListViewModel"/>.
    /// </summary>
    /// <param name="itemRepository">The repository used to retrieve items.</param>
    public ItemsListViewModel(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    /// <summary>Called automatically when SelectedOwnerFilter changes.</summary>
    partial void OnSelectedOwnerFilterChanged(string value) => ApplyFiltersAndSort();

    /// <summary>Called automatically when SelectedSortOption changes.</summary>
    partial void OnSelectedSortOptionChanged(string value) => ApplyFiltersAndSort();

    /// <summary>Called automatically when SearchQuery changes.</summary>
    partial void OnSearchQueryChanged(string value) => ApplyFiltersAndSort();

    /// <summary>
    /// Loads all available items from the repository and populates filter options.
    /// Skips execution if a load is already in progress.
    /// </summary>
    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        IsRefreshing = true;
        IsEmpty = false;

        try
        {
            var items = await _itemRepository.GetAllAsync();
            _allItems = items.ToList();

            // Build owner filter list dynamically from loaded items
            var owners = _allItems
                .Select(i => i.OwnerName)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .OrderBy(n => n)
                .ToList();

            OwnerFilters = new ObservableCollection<string>(
                new[] { "All owners" }.Concat(owners));

            // Reset filters and apply
            SelectedOwnerFilter = "All owners";
            SelectedSortOption = "Default";
            SearchQuery = string.Empty;
            ApplyFiltersAndSort();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading items: {ex.Message}");
            IsEmpty = true;
            ErrorMessage = $"Failed to load items: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            IsRefreshing = false;
        }
    }

    /// <summary>
    /// Applies the current search query, owner filter and sort option
    /// to produce the filtered and sorted items collection.
    /// </summary>
    private void ApplyFiltersAndSort()
    {
        var filtered = _allItems.AsEnumerable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(SearchQuery))
            filtered = filtered.Where(i =>
                i.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        // Apply owner filter
        if (SelectedOwnerFilter != "All owners")
            filtered = filtered.Where(i => i.OwnerName == SelectedOwnerFilter);

        // Apply sort
        filtered = SelectedSortOption switch
        {
            "Price: Low to High"  => filtered.OrderBy(i => i.DailyRate),
            "Price: High to Low"  => filtered.OrderByDescending(i => i.DailyRate),
            "Rating: High to Low" => filtered.OrderByDescending(i => i.AverageRating ?? 0),
            "Newest first"        => filtered.OrderByDescending(i => i.CreatedAt),
            _                     => filtered
        };

        Items = new ObservableCollection<Item>(filtered);
        IsEmpty = !Items.Any();
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

    /// <summary>
    /// Navigates to the Create Item page.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToCreateAsync()
    {
        await Shell.Current.GoToAsync("createitem");
    }
}