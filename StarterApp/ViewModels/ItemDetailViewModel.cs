using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Item Detail page, loading and displaying full details
/// for a single item with navigation to rental request and reviews.
/// </summary>
[QueryProperty(nameof(ItemId), "id")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    /// <summary>Gets or sets the item currently being displayed.</summary>
    [ObservableProperty]
    private Item? _item;

    /// <summary>Gets or sets a value indicating whether the item is being loaded.</summary>
    [ObservableProperty]
    private bool _isBusy;

    private int _itemId;

    /// <summary>
    /// Gets or sets the unique identifier of the item to display.
    /// Setting this property automatically triggers a load of the item data.
    /// </summary>
    public int ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            LoadItemCommand.Execute(null);
        }
    }

    /// <summary>
    /// Initialises a new instance of <see cref="ItemDetailViewModel"/>.
    /// </summary>
    /// <param name="apiService">The API service used to retrieve item details.</param>
    public ItemDetailViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Loads the full details of the current item from the API.
    /// </summary>
    [RelayCommand]
    private async Task LoadItemAsync()
    {
        IsBusy = true;
        try
        {
            Item = await _apiService.GetItemAsync(_itemId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Navigates to the Request Rental page for the current item.
    /// </summary>
    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        await Shell.Current.GoToAsync($"requestrental?itemId={_itemId}");
    }

    /// <summary>
    /// Navigates to the Item Reviews page for the current item.
    /// </summary>
    [RelayCommand]
    private async Task NavigateToReviewsAsync()
    {
        await Shell.Current.GoToAsync($"itemreviews?itemId={_itemId}");
    }
}