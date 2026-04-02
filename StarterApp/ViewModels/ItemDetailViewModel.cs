using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "id")]
public partial class ItemDetailViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private Item? _item;

    [ObservableProperty]
    private bool _isBusy;

    private int _itemId;
    public int ItemId
    {
        get => _itemId;
        set
        {
            _itemId = value;
            LoadItemCommand.Execute(null);
        }
    }

    public ItemDetailViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

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

    [RelayCommand]
    private async Task RequestRentalAsync()
    {
        await Shell.Current.GoToAsync($"requestrental?itemId={_itemId}");
    }
}