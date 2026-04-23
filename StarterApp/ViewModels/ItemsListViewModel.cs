using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Items List page, loading and displaying all available
/// rental items from the repository with optional owner filtering.
/// </summary>
public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private List<Item> _allItems = new();

    /// <summary>Gets or sets the filtered collection of items displayed in the UI.</summary>
    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    /// <summary>Gets or sets the list of owner names available for filtering.</summary>
    [ObservableProperty]
    private ObservableCollection<string> _ownerFilters = new();

    /// <summary>Gets or sets the currently selected owner filter.</summary>
    [ObservableProperty]
    private string _selectedOwnerFilter = "All owners";

    /// <summary>Gets or sets a value indicating whether items are being loaded.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets a value indicating whether the items list is empty.</summary>
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

    /// <summary>
    /// Called automatically when SelectedOwnerFilter changes.
    /// Applies the selected filter to the items collection.
    /// </summary>
    partial void OnSelectedOwnerFilterChanged(string value)
    {
        ApplyFilter(value);
    }

    /// <summary>
    /// Loads all available items from the repository and populates the owner filter.
    /// Skips execution if a load is already in progress.
    /// </summary>
    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
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

            // Reset filter and apply
            SelectedOwnerFilter = "All owners";
            ApplyFilter("All owners");
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
        }
    }

    /// <summary>
    /// Applies the owner filter to the items collection.
    /// Shows all items when "All owners" is selected.
    /// </summary>
    /// <param name="ownerName">The owner name to filter by, or "All owners" for no filter.</param>
    private void ApplyFilter(string ownerName)
    {
        var filtered = ownerName == "All owners"
            ? _allItems
            : _allItems.Where(i => i.OwnerName == ownerName).ToList();

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