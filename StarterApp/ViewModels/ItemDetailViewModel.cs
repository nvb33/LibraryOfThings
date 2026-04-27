using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Item Detail page, loading and displaying full details
/// for a single item with navigation to rental request and reviews.
/// Hides the rental request button for items owned by the current user.
/// </summary>
[QueryProperty(nameof(ItemId), "id")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IAuthenticationService _authService;

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
    /// Gets a value indicating whether the current user can request a rental.
    /// Returns false if the item is owned by the current user.
    /// </summary>
    public bool CanRequestRental =>
        Item != null &&
        Item.OwnerId != (_authService.CurrentUser?.Id ?? -1);

    /// <summary>
    /// Initialises a new instance of <see cref="ItemDetailViewModel"/>.
    /// </summary>
    /// <param name="itemRepository">The repository used to retrieve item details.</param>
    /// <param name="authService">The authentication service used to identify the current user.</param>
    public ItemDetailViewModel(IItemRepository itemRepository,
        IAuthenticationService authService)
    {
        _itemRepository = itemRepository;
        _authService = authService;
    }

    /// <summary>
    /// Loads the full details of the current item from the repository.
    /// </summary>
    [RelayCommand]
    private async Task LoadItemAsync()
    {
        IsBusy = true;
        try
        {
            Item = await _itemRepository.GetByIdAsync(_itemId);
            OnPropertyChanged(nameof(CanRequestRental));
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