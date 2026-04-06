using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Database.Models;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemReviewsViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private int _itemId;

    [ObservableProperty]
    private ObservableCollection<Review> _reviews = new();

    [ObservableProperty]
    private double _averageRating;

    [ObservableProperty]
    private int _totalReviews;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public string FormattedAverageRating => TotalReviews > 0
        ? $"{AverageRating:F1} ★ ({TotalReviews} reviews)"
        : "No reviews yet";

    public ItemReviewsViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnItemIdChanged(int value)
    {
        if (value > 0)
            LoadReviewsCommand.ExecuteAsync(null);
    }

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