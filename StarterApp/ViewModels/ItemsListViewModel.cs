using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class ItemsListViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Item> _items = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ItemsListViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

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
            // Show the actual error in the empty state label temporarily
            // so you can see what went wrong during development
            ErrorMessage = $"Failed to load items: {ex.Message}";
        }
        finally
        {
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