using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _mockRepository;
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _mockRepository = new Mock<IReviewRepository>();
        _service = new ReviewService(_mockRepository.Object);
    }

    [Fact]
    public void IsValidRating_WhenBetweenOneAndFive_ReturnsTrue()
    {
        Assert.True(_service.IsValidRating(1));
        Assert.True(_service.IsValidRating(3));
        Assert.True(_service.IsValidRating(5));
    }

    [Fact]
    public void IsValidRating_WhenOutOfRange_ReturnsFalse()
    {
        Assert.False(_service.IsValidRating(0));
        Assert.False(_service.IsValidRating(6));
    }

    [Fact]
    public void CanSubmitReview_WhenCompleted_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Completed" };

        // Assert
        Assert.True(_service.CanSubmitReview(rental));
    }

    [Fact]
    public void CanSubmitReview_WhenNotCompleted_ReturnsFalse()
    {
        // Arrange
        var rental = new Rental { Status = "Returned" };

        // Assert
        Assert.False(_service.CanSubmitReview(rental));
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenValidInput_CallsRepository()
    {
        // Arrange
        var rental = new Rental { Id = 1, Status = "Completed" };
        var review = new Review { Id = 10, RentalId = 1, Rating = 5 };
        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Review>()))
            .ReturnsAsync(review);

        // Act
        var result = await _service.SubmitReviewAsync(rental, 5, "Great!");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenNotCompleted_ThrowsInvalidOperationException()
    {
        // Arrange
        var rental = new Rental { Id = 1, Status = "Returned" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.SubmitReviewAsync(rental, 5, "Great!"));
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenInvalidRating_ThrowsArgumentException()
    {
        // Arrange
        var rental = new Rental { Id = 1, Status = "Completed" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.SubmitReviewAsync(rental, 6, "Great!"));
    }

    [Fact]
    public async Task SubmitReviewAsync_WhenCommentTooLong_ThrowsArgumentException()
    {
        // Arrange
        var rental = new Rental { Id = 1, Status = "Completed" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.SubmitReviewAsync(rental, 5, new string('a', 501)));
    }

    [Fact]
    public async Task GetAverageRatingAsync_ReturnsValueFromRepository()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAverageRatingAsync(1)).ReturnsAsync(4.5);

        // Act
        var result = await _service.GetAverageRatingAsync(1);

        // Assert
        Assert.Equal(4.5, result);
    }
}