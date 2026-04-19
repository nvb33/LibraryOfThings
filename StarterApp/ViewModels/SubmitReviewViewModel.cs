using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Submit Review page, allowing a borrower to rate
/// and comment on a completed rental.
/// </summary>
[QueryProperty(nameof(RentalId), "rentalId")]
[QueryProperty(nameof(ItemTitle), "itemTitle")]
public partial class SubmitReviewViewModel : ObservableObject
{
    private readonly IReviewRepository _reviewRepository;

    /// <summary>Gets or sets the unique identifier of the completed rental being reviewed.</summary>
    [ObservableProperty]
    private int _rentalId;

    /// <summary>Gets or sets the title of the item being reviewed.</summary>
    [ObservableProperty]
    private string _itemTitle = string.Empty;

    /// <summary>Gets or sets the selected star rating. Defaults to 5.</summary>
    [ObservableProperty]
    private int _rating = 5;

    /// <summary>Gets or sets the optional written comment. Maximum 500 characters.</summary>
    [ObservableProperty]
    private string _comment = string.Empty;

    /// <summary>Gets or sets a value indicating whether an API operation is in progress.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets the error message to display when validation or submission fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>Gets the hex colour for star 1, gold if selected, grey otherwise.</summary>
    public string Star1Colour => Rating >= 1 ? "#FFD700" : "#CCCCCC";

    /// <summary>Gets the hex colour for star 2, gold if selected, grey otherwise.</summary>
    public string Star2Colour => Rating >= 2 ? "#FFD700" : "#CCCCCC";

    /// <summary>Gets the hex colour for star 3, gold if selected, grey otherwise.</summary>
    public string Star3Colour => Rating >= 3 ? "#FFD700" : "#CCCCCC";

    /// <summary>Gets the hex colour for star 4, gold if selected, grey otherwise.</summary>
    public string Star4Colour => Rating >= 4 ? "#FFD700" : "#CCCCCC";

    /// <summary>Gets the hex colour for star 5, gold if selected, grey otherwise.</summary>
    public string Star5Colour => Rating >= 5 ? "#FFD700" : "#CCCCCC";

    /// <summary>
    /// Initialises a new instance of <see cref="SubmitReviewViewModel"/>.
    /// </summary>
    /// <param name="reviewRepository">The repository used to submit reviews.</param>
    public SubmitReviewViewModel(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    /// <summary>Sets the current star rating and updates star colours.</summary>
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

    /// <summary>
    /// Validates and submits the review via the repository.
    /// </summary>
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
            var review = new Review
            {
                RentalId = RentalId,
                Rating = Rating,
                Comment = Comment
            };

            var created = await _reviewRepository.AddAsync(review);

            if (created != null)
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

    /// <summary>Navigates back without submitting a review.</summary>
    [RelayCommand]
    private async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}