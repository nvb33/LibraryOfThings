using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class CreateItemViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private decimal _dailyRate;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Holds the list of categories loaded from the API
    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    // The category the user selected in the picker
    [ObservableProperty]
    private Category? _selectedCategory;

    public CreateItemViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    // Called when the page appears — loads categories from API
    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _apiService.GetCategoriesAsync();
            Categories = new ObservableCollection<Category>(categories);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task CreateItemAsync()
    {
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

        if (SelectedCategory == null)
        {
            ErrorMessage = "Please select a category.";
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
                CategoryId = SelectedCategory.Id,
                Latitude = 55.9533,
                Longitude = -3.1883
            };

            var created = await _apiService.CreateItemAsync(newItem);

            if (created != null)
            {
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