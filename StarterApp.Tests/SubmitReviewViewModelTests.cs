using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class SubmitReviewViewModelTests
{
    private readonly Mock<IReviewRepository> _mockReviewRepository;
    private readonly SubmitReviewViewModel _viewModel;

    public SubmitReviewViewModelTests()
    {
        _mockReviewRepository = new Mock<IReviewRepository>();
        _viewModel = new SubmitReviewViewModel(_mockReviewRepository.Object);
    }

    [Fact]
    public void Rating_DefaultsToFive()
    {
        // Assert
        Assert.Equal(5, _viewModel.Rating);
    }

    [Fact]
    public void SetRating_UpdatesRating()
    {
        // Act
        _viewModel.SetRatingCommand.Execute("3");

        // Assert
        Assert.Equal(3, _viewModel.Rating);
    }

    [Fact]
    public void SetRating_UpdatesStarColours()
    {
        // Act
        _viewModel.SetRatingCommand.Execute("3");

        // Assert — stars 1-3 should be gold, 4-5 should be grey
        Assert.Equal("#FFD700", _viewModel.Star1Colour);
        Assert.Equal("#FFD700", _viewModel.Star2Colour);
        Assert.Equal("#FFD700", _viewModel.Star3Colour);
        Assert.Equal("#CCCCCC", _viewModel.Star4Colour);
        Assert.Equal("#CCCCCC", _viewModel.Star5Colour);
    }

    [Fact]
    public void SetRating_WhenRating5_AllStarsGold()
    {
        // Act
        _viewModel.SetRatingCommand.Execute("5");

        // Assert
        Assert.Equal("#FFD700", _viewModel.Star1Colour);
        Assert.Equal("#FFD700", _viewModel.Star2Colour);
        Assert.Equal("#FFD700", _viewModel.Star3Colour);
        Assert.Equal("#FFD700", _viewModel.Star4Colour);
        Assert.Equal("#FFD700", _viewModel.Star5Colour);
    }

    [Fact]
    public async Task SubmitReview_WhenCommentTooLong_SetsErrorMessage()
    {
        // Arrange
        _viewModel.Rating = 5;
        _viewModel.Comment = new string('a', 501);

        // Act
        await _viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Comment must be 500 characters or fewer.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SubmitReview_WhenValidInput_CallsRepository()
    {
        // Arrange
        _viewModel.RentalId = 1;
        _viewModel.Rating = 4;
        _viewModel.Comment = "Great item!";
        _mockReviewRepository
            .Setup(r => r.AddAsync(It.IsAny<Review>()))
            .ReturnsAsync((Review?)null);

        // Act
        await _viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        _mockReviewRepository.Verify(
            r => r.AddAsync(It.Is<Review>(review =>
                review.RentalId == 1 &&
                review.Rating == 4 &&
                review.Comment == "Great item!")),
            Times.Once);
    }

    [Fact]
    public async Task SubmitReview_WhenRepositoryReturnsNull_SetsErrorMessage()
    {
        // Arrange
        _viewModel.RentalId = 1;
        _viewModel.Rating = 4;
        _viewModel.Comment = "Great item!";
        _mockReviewRepository
            .Setup(r => r.AddAsync(It.IsAny<Review>()))
            .ReturnsAsync((Review?)null);

        // Act
        await _viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }
}