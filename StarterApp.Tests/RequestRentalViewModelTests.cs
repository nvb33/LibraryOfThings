using Moq;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class RequestRentalViewModelTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly RequestRentalViewModel _viewModel;

    public RequestRentalViewModelTests()
    {
        _mockApiService = new Mock<IApiService>();
        _viewModel = new RequestRentalViewModel(_mockApiService.Object);
    }

    [Fact]
    public void StartDate_DefaultsToTomorrow()
    {
        // Arrange + Act (set in constructor)
        var expected = DateTime.Today.AddDays(1);

        // Assert
        Assert.Equal(expected, _viewModel.StartDate);
    }

    [Fact]
    public void EndDate_DefaultsToTwoDaysFromNow()
    {
        // Arrange + Act (set in constructor)
        var expected = DateTime.Today.AddDays(2);

        // Assert
        Assert.Equal(expected, _viewModel.EndDate);
    }

    [Fact]
    public void MinDate_IsTomorrow()
    {
        // Arrange + Act
        var expected = DateTime.Today.AddDays(1);

        // Assert
        Assert.Equal(expected, _viewModel.MinDate);
    }

    [Fact]
    public async Task SubmitRental_WhenEndDateBeforeStartDate_SetsErrorMessage()
    {
        // Arrange
        _viewModel.StartDate = DateTime.Today.AddDays(5);
        _viewModel.EndDate = DateTime.Today.AddDays(3);

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("End date must be after start date.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SubmitRental_WhenEndDateEqualsStartDate_SetsErrorMessage()
    {
        // Arrange
        var date = DateTime.Today.AddDays(3);
        _viewModel.StartDate = date;
        _viewModel.EndDate = date;

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("End date must be after start date.", _viewModel.ErrorMessage);
    }

    [Fact]
    public async Task SubmitRental_WhenValidDates_CallsApiService()
    {
        // Arrange
        _viewModel.ItemId = 1;
        _viewModel.StartDate = DateTime.Today.AddDays(1);
        _viewModel.EndDate = DateTime.Today.AddDays(3);
        _mockApiService
            .Setup(s => s.CreateRentalAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((StarterApp.Database.Models.Rental?)null);

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        _mockApiService.Verify(
            s => s.CreateRentalAsync(1, It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitRental_WhenApiFails_SetsErrorMessage()
    {
        // Arrange
        _viewModel.ItemId = 1;
        _viewModel.StartDate = DateTime.Today.AddDays(1);
        _viewModel.EndDate = DateTime.Today.AddDays(3);
        _mockApiService
            .Setup(s => s.CreateRentalAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((StarterApp.Database.Models.Rental?)null);

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }
}