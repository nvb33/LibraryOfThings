using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class ItemsListViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;

    // ObservableCollection tells the UI when items are added/removed
    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    // When true, show a loading spinner
    [ObservableProperty]
    private bool _isBusy;

    // When true, show an empty state message
    [ObservableProperty]
    private bool _isEmpty;

    public ItemsListViewModel(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        if (IsBusy) return; // prevent double-loading

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
            // In a real app, show an error message to the user
            System.Diagnostics.Debug.WriteLine($"Error loading items: {ex.Message}");
        }
        finally
        {
            // "finally" runs whether or not an exception occurred
            // This ensures the spinner always gets hidden
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task NavigateToDetailAsync(Item item)
    {
        await Shell.Current.GoToAsync($"itemdetail?id={item.Id}");
    }

    [RelayCommand]
    private async Task NavigateToCreateAsync()
    {
        await Shell.Current.GoToAsync("createitem");
    }
}