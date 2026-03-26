using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.ViewModels;

public partial class CreateItemViewModel : ObservableObject
{
    private readonly IItemRepository _itemRepository;
    private readonly IApiService _apiService;

    // These bind to the form fields
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _dailyRate;

    [ObservableProperty]
    private int _selectedCategoryId;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public CreateItemViewModel(IItemRepository itemRepository, IApiService apiService)
    {
        _itemRepository = itemRepository;
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task CreateItemAsync()
    {
        // Basic validation before hitting the API
        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "Title is required.";
            return;
        }

        if (DailyRate <= 0)
        {
            ErrorMessage = "Daily rate must be greater than zero.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var newItem = new Item
            {
                Title = Title,
                Description = Description,
                DailyRate = DailyRate,
                CategoryId = SelectedCategoryId,
                // Location
                Latitude = 55.9533,
                Longitude = -3.1883
            };

            var created = await _apiService.CreateItemAsync(newItem);

            if (created != null)
            {
                // Navigate back to the list after successful creation
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Failed to create item. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}