using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class ItemReviewsViewModelTests
{
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly ItemReviewsViewModel _viewModel;

    public ItemReviewsViewModelTests()
    {
        _mockReviewService = new Mock<IReviewService>();
        _viewModel = new ItemReviewsViewModel(_mockReviewService.Object);
    }

    [Fact]
    public void Reviews_DefaultsToEmpty()
    {
        Assert.Empty(_viewModel.Reviews);
    }

    [Fact]
    public void AverageRating_DefaultsToZero()
    {
        Assert.Equal(0, _viewModel.AverageRating);
    }

    [Fact]
    public void TotalReviews_DefaultsToZero()
    {
        Assert.Equal(0, _viewModel.TotalReviews);
    }

    [Fact]
    public void FormattedAverageRating_WhenNoReviews_ReturnsNoReviewsYet()
    {
        // Arrange
        _viewModel.TotalReviews = 0;

        // Assert
        Assert.Equal("No reviews yet", _viewModel.FormattedAverageRating);
    }

    [Fact]
    public void FormattedAverageRating_WhenHasReviews_ReturnsFormattedString()
    {
        // Arrange
        _viewModel.AverageRating = 4.5;
        _viewModel.TotalReviews = 10;

        // Assert
        Assert.Equal("4.5 ★ (10 reviews)", _viewModel.FormattedAverageRating);
    }

    [Fact]
    public async Task LoadReviews_PopulatesReviewsCollection()
    {
        // Arrange
        var reviews = new List<Review>
        {
            new Review { Id = 1, Rating = 5, Comment = "Great!" },
            new Review { Id = 2, Rating = 4, Comment = "Good" }
        };
        _mockReviewService.Setup(s => s.GetReviewsForItemAsync(It.IsAny<int>())).ReturnsAsync(reviews);
        _mockReviewService.Setup(s => s.GetAverageRatingAsync(It.IsAny<int>())).ReturnsAsync(4.5);
        _mockReviewService.Setup(s => s.GetTotalReviewsAsync(It.IsAny<int>())).ReturnsAsync(2);
        _viewModel.ItemId = 1;

        // Act
        await _viewModel.LoadReviewsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, _viewModel.Reviews.Count);
    }

    [Fact]
    public async Task LoadReviews_SetsAverageRatingAndTotalReviews()
    {
        // Arrange
        _mockReviewService.Setup(s => s.GetReviewsForItemAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Review>());
        _mockReviewService.Setup(s => s.GetAverageRatingAsync(It.IsAny<int>())).ReturnsAsync(4.5);
        _mockReviewService.Setup(s => s.GetTotalReviewsAsync(It.IsAny<int>())).ReturnsAsync(8);
        _viewModel.ItemId = 1;

        // Act
        await _viewModel.LoadReviewsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(4.5, _viewModel.AverageRating);
        Assert.Equal(8, _viewModel.TotalReviews);
    }

    [Fact]
    public async Task LoadReviews_WhenServiceFails_SetsErrorMessage()
    {
        // Arrange
        _mockReviewService
            .Setup(s => s.GetReviewsForItemAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Network error"));

        // Act
        await _viewModel.LoadReviewsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }

    [Fact]
    public async Task LoadReviews_SetsIsBusyFalseAfterCompletion()
    {
        // Arrange
        _mockReviewService.Setup(s => s.GetReviewsForItemAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<Review>());
        _mockReviewService.Setup(s => s.GetAverageRatingAsync(It.IsAny<int>()))
            .ReturnsAsync(0.0);
        _mockReviewService.Setup(s => s.GetTotalReviewsAsync(It.IsAny<int>()))
            .ReturnsAsync(0);

        // Act
        await _viewModel.LoadReviewsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}