using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.ViewModels;

// QueryProperty links a URL parameter to a property
// When you navigate to "itemdetail?id=42", ItemId gets set to 42
[QueryProperty(nameof(ItemId), "id")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;

    [ObservableProperty]
    private Item? _item;

    [ObservableProperty]
    private bool _isBusy;

    // When ItemId is set (via navigation), load that item
    private int _itemId;
    public int ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            // Load the item as soon as we know the ID
            LoadItemCommand.Execute(null);
        }
    }

    public ItemDetailViewModel(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [RelayCommand]
    private async Task LoadItemAsync()
    {
        IsBusy = true;

        try
        {
            Item = await _itemRepository.GetByIdAsync(_itemId);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        // We'll implement this in Week 3
        await Shell.Current.GoToAsync($"rental/create?itemId={_itemId}");
    }
}