using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// <summary>
/// ViewModel for the Item Reviews page, displaying all reviews for a specific item
/// along with the average rating and total review count.
/// </summary>
[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemReviewsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    /// <summary>Gets or sets the unique identifier of the item whose reviews are displayed.</summary>
    [ObservableProperty]
    private int _itemId;

    /// <summary>Gets or sets the collection of reviews for the item.</summary>
    [ObservableProperty]
    private ObservableCollection<Review> _reviews = new();

    /// <summary>Gets or sets the average rating across all reviews for the item.</summary>
    [ObservableProperty]
    private double _averageRating;

    /// <summary>Gets or sets the total number of reviews for the item.</summary>
    [ObservableProperty]
    private int _totalReviews;

    /// <summary>Gets or sets a value indicating whether an API operation is in progress.</summary>
    [ObservableProperty]
    private bool _isBusy;

    /// <summary>Gets or sets the error message to display when loading fails.</summary>
    [ObservableProperty]
    private string _errorMessage = string.Empty;

    /// <summary>
    /// Gets a formatted summary string showing the average rating and total review count.
    /// Returns "No reviews yet" when there are no reviews.
    /// Example: "4.5 ★ (12 reviews)"
    /// </summary>
    public string FormattedAverageRating => TotalReviews > 0
        ? $"{AverageRating:F1} ★ ({TotalReviews} reviews)"
        : "No reviews yet";

    /// <summary>
    /// Initialises a new instance of <see cref="ItemReviewsViewModel"/>.
    /// </summary>
    /// <param name="apiService">The API service used to retrieve item reviews.</param>
    public ItemReviewsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <summary>
    /// Called automatically when the ItemId query property is set.
    /// Triggers a review load if the ID is valid.
    /// </summary>
    /// <param name="value">The new item ID value.</param>
    partial void OnItemIdChanged(int value)
    {
        if (value > 0)
            LoadReviewsCommand.ExecuteAsync(null);
    }

    /// <summary>
    /// Loads all reviews for the current item from the API.
    /// Skips execution if a load is already in progress.
    /// </summary>
    [RelayCommand]
    private async Task LoadReviewsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _apiService.GetItemReviewsAsync(ItemId);
            var reviews = result.Reviews;
            var averageRating = result.AverageRating;
            var totalReviews = result.TotalReviews;

            Reviews = new ObservableCollection<Review>(reviews);
            AverageRating = averageRating ?? 0.0;
            TotalReviews = totalReviews;
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