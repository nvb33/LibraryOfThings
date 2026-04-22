using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Items List page, loading and displaying all available
/// rental items from the repository with navigation to item detail and creation.
/// </summary>
public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;

    /// <summary>Gets or sets the collection of items loaded from the repository.</summary>
    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

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
    /// Loads all available items from the repository.
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
            Items = new ObservableCollection<Item>(items);
            IsEmpty = !Items.Any();
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