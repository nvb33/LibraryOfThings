using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Items List page, loading and displaying all available
/// rental items from the API with navigation to item detail and creation.
/// </summary>
public partial class ItemsListViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    /// <summary>Gets or sets the collection of items loaded from the API.</summary>
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
    /// <param name="apiService">The API service used to retrieve the items list.</param>
    public ItemsListViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Loads all available items from the API.
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
            var items = await _apiService.GetItemsAsync();
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