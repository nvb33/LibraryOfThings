using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Item Reviews page, displaying all reviews for a specific
/// item along with the average rating and total review count.
/// </summary>
[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemReviewsViewModel : ObservableObject
{
    private readonly IReviewRepository _reviewRepository;

    /// <summary>Gets or sets the unique identifier of the item whose reviews are displayed.</summary>
    [ObservableProperty]
    private int _itemId;

    /// <summary>Gets or sets the collection of reviews for the item.</summary>
    [ObservableProperty]
    private ObservableCollection<Review> _reviews = new();

    /// <summary>Gets or sets the average rating across all reviews.</summary>
    [ObservableProperty]
    private double _averageRating;

    /// <summary>Gets or sets the total number of reviews.</summary>
    [ObservableProperty]
    private int _totalReviews;

    /// <summary>Gets or sets a value indicating whether reviews are being loaded.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets the error message to display when loading fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Gets a formatted summary string showing average rating and total count.
    /// </summary>
    public string FormattedAverageRating => TotalReviews > 0
        ? $"{AverageRating:F1} ★ ({TotalReviews} reviews)"
        : "No reviews yet";

    /// <summary>
    /// Initialises a new instance of <see cref="ItemReviewsViewModel"/>.
    /// </summary>
    /// <param name="reviewRepository">The repository used to retrieve item reviews.</param>
    public ItemReviewsViewModel(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    /// <summary>
    /// Called automatically when ItemId is set. Triggers a review load if valid.
    /// </summary>
    partial void OnItemIdChanged(int value)
    {
        if (value > 0)
            LoadReviewsCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// Loads all reviews for the current item from the repository.
    /// </summary>
    [RelayCommand]
    private async Task LoadReviewsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var reviews = await _reviewRepository.GetByItemAsync(ItemId);
            Reviews = new ObservableCollection<Review>(reviews);
            AverageRating = await _reviewRepository.GetAverageRatingAsync(ItemId);
            TotalReviews = await _reviewRepository.GetTotalReviewsAsync(ItemId);
            OnPropertyChanged(nameof(FormattedAverageRating));
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load reviews: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}