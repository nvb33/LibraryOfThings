using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class SubmitReviewViewModelTests
{
    private readonly Mock<IReviewService> _mockReviewService;
    private readonly SubmitReviewViewModel _viewModel;

    public SubmitReviewViewModelTests()
    {
        _mockReviewService = new Mock<IReviewService>();
        _mockReviewService.Setup(s => s.IsValidRating(It.IsAny<int>()))
            .Returns((int r) => r >= 1 && r <= 5);
        _viewModel = new SubmitReviewViewModel(_mockReviewService.Object);
    }

    [Fact]
    public void Rating_DefaultsToFive()
    {
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

        // Assert
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
    public async Task SubmitReview_WhenValidInput_CallsReviewService()
    {
        // Arrange
        _viewModel.RentalId = 1;
        _viewModel.Rating = 4;
        _viewModel.Comment = "Great item!";
        _mockReviewService
            .Setup(s => s.SubmitReviewAsync(
                It.IsAny<Rental>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .ReturnsAsync((Review?)null);

        // Act
        await _viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        _mockReviewService.Verify(
            s => s.SubmitReviewAsync(
                It.Is<Rental>(r => r.Id == 1),
                4,
                "Great item!"),
            Times.Once);
    }

    [Fact]
    public async Task SubmitReview_WhenServiceReturnsNull_SetsErrorMessage()
    {
        // Arrange
        _viewModel.RentalId = 1;
        _viewModel.Rating = 4;
        _viewModel.Comment = "Great item!";
        _mockReviewService
            .Setup(s => s.SubmitReviewAsync(
                It.IsAny<Rental>(),
                It.IsAny<int>(),
                It.IsAny<string>()))
            .ReturnsAsync((Review?)null);

        // Act
        await _viewModel.SubmitReviewCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }
}