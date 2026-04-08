using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Create Item page, handling input validation and
/// submission of a new item listing to the API.
/// </summary>
public partial class CreateItemViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    /// <summary>Gets or sets the title of the new item.</summary>
    [ObservableProperty]
    private string _title = string.Empty;

    /// <summary>Gets or sets the description of the new item.</summary>
    [ObservableProperty]
    private string _description = string.Empty;

    /// <summary>Gets or sets the daily rental rate in GBP.</summary>
    [ObservableProperty]
    private decimal _dailyRate;

    /// <summary>Gets or sets a value indicating whether a create operation is in progress.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets the error message to display when validation or creation fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>Gets or sets the list of categories loaded from the API for the category picker.</summary>
    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    /// <summary>Gets or sets the category selected by the user in the picker.</summary>
    [ObservableProperty]
    private Category? _selectedCategory;

    /// <summary>
    /// Initialises a new instance of <see cref="CreateItemViewModel"/>.
    /// </summary>
    /// <param name="apiService">The API service used to load categories and create items.</param>
    public CreateItemViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Loads the available categories from the API for display in the category picker.
    /// Called when the page appears.
    /// </summary>
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

    /// <summary>
    /// Validates the form inputs and creates a new item listing via the API.
    /// Navigates back on success, or sets an error message on failure.
    /// </summary>
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

    /// <summary>
    /// Navigates back to the previous page without creating an item.
    /// </summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}