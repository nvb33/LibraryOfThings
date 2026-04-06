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
    public bool Star1Selected => Rating >= 1;
    public bool Star2Selected => Rating >= 2;
    public bool Star3Selected => Rating >= 3;
    public bool Star4Selected => Rating >= 4;
    public bool Star5Selected => Rating >= 5;

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
            OnPropertyChanged(nameof(Star1Selected));
            OnPropertyChanged(nameof(Star2Selected));
            OnPropertyChanged(nameof(Star3Selected));
            OnPropertyChanged(nameof(Star4Selected));
            OnPropertyChanged(nameof(Star5Selected));
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