using Moq;
using StarterApp.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests;

/// <summary>
/// Unit tests for ReviewRepository verifying correct delegation to IApiService.
/// </summary>
public class ReviewRepositoryTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly ReviewRepository _repository;

    public ReviewRepositoryTests()
    {
        _mockApiService = new Mock<IApiService>();
        _repository = new ReviewRepository(_mockApiService.Object);
    }

    [Fact]
    public async Task GetByItemAsync_ReturnsReviewsFromApiService()
    {
        // Arrange
        var reviews = new List<Review>
        {
            new Review { Id = 1, Rating = 5, Comment = "Great!" },
            new Review { Id = 2, Rating = 4, Comment = "Good" }
        };
        _mockApiService
            .Setup(s => s.GetItemReviewsAsync(1))
            .ReturnsAsync(new Services.ItemReviewsResult
            {
                Reviews = reviews,
                AverageRating = 4.5,
                TotalReviews = 2
            });

        // Act
        var result = await _repository.GetByItemAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAverageRatingAsync_ReturnsCorrectAverage()
    {
        // Arrange
        _mockApiService
            .Setup(s => s.GetItemReviewsAsync(1))
            .ReturnsAsync(new Services.ItemReviewsResult
            {
                Reviews = new List<Review>(),
                AverageRating = 4.5,
                TotalReviews = 10
            });

        // Act
        var result = await _repository.GetAverageRatingAsync(1);

        // Assert
        Assert.Equal(4.5, result);
    }

    [Fact]
    public async Task GetAverageRatingAsync_WhenNoReviews_ReturnsZero()
    {
        // Arrange
        _mockApiService
            .Setup(s => s.GetItemReviewsAsync(1))
            .ReturnsAsync(new Services.ItemReviewsResult
            {
                Reviews = new List<Review>(),
                AverageRating = null,
                TotalReviews = 0
            });

        // Act
        var result = await _repository.GetAverageRatingAsync(1);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public async Task GetTotalReviewsAsync_ReturnsCorrectCount()
    {
        // Arrange
        _mockApiService
            .Setup(s => s.GetItemReviewsAsync(1))
            .ReturnsAsync(new Services.ItemReviewsResult
            {
                Reviews = new List<Review>(),
                AverageRating = 4.0,
                TotalReviews = 8
            });

        // Act
        var result = await _repository.GetTotalReviewsAsync(1);

        // Assert
        Assert.Equal(8, result);
    }

    [Fact]
    public async Task AddAsync_SubmitsReviewViaApiService()
    {
        // Arrange
        var review = new Review { RentalId = 1, Rating = 5, Comment = "Excellent!" };
        var created = new Review { Id = 10, RentalId = 1, Rating = 5 };
        _mockApiService
            .Setup(s => s.SubmitReviewAsync(1, 5, "Excellent!"))
            .ReturnsAsync(created);

        // Act
        var result = await _repository.AddAsync(review);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task DeleteAsync_AlwaysReturnsFalse()
    {
        // Act
        var result = await _repository.DeleteAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmptyCollection()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }
}