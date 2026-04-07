using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(RentalId), "rentalId")]
[QueryProperty(nameof(ItemTitle), "itemTitle")]
public partial class SubmitReviewViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private int _rentalId;

    [ObservableProperty]
    private string _itemTitle = string.Empty;

    [ObservableProperty]
    private int _rating = 5;

    [ObservableProperty]
    private string _comment = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Star display properties
    public string Star1Colour => Rating >= 1 ? "#FFD700" : "#CCCCCC";
    public string Star2Colour => Rating >= 2 ? "#FFD700" : "#CCCCCC";
    public string Star3Colour => Rating >= 3 ? "#FFD700" : "#CCCCCC";
    public string Star4Colour => Rating >= 4 ? "#FFD700" : "#CCCCCC";
    public string Star5Colour => Rating >= 5 ? "#FFD700" : "#CCCCCC";
    public SubmitReviewViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private void SetRating(string value)
    {
        if (int.TryParse(value, out int rating))
        {
            Rating = rating;
            OnPropertyChanged(nameof(Star1Colour));
            OnPropertyChanged(nameof(Star2Colour));
            OnPropertyChanged(nameof(Star3Colour));
            OnPropertyChanged(nameof(Star4Colour));
            OnPropertyChanged(nameof(Star5Colour));
        }
    }

    [RelayCommand]
    private async Task SubmitReviewAsync()
    {
        if (Rating < 1 || Rating > 5)
        {
            ErrorMessage = "Please select a rating between 1 and 5.";
            return;
        }

        if (Comment.Length > 500)
        {
            ErrorMessage = "Comment must be 500 characters or fewer.";
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var review = await _apiService.SubmitReviewAsync(RentalId, Rating, Comment);

            if (review != null)
            {
                await Shell.Current.DisplayAlert(
                    "Review Submitted",
                    "Thank you for your feedback!",
                    "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "Failed to submit review. You may have already reviewed this rental.";
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