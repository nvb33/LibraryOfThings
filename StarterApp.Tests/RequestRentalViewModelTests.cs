using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class RequestRentalViewModelTests
{
    private readonly Mock<IRentalService> _mockRentalService;
    private readonly RequestRentalViewModel _viewModel;

    public RequestRentalViewModelTests()
    {
        _mockRentalService = new Mock<IRentalService>();
        _viewModel = new RequestRentalViewModel(_mockRentalService.Object);
    }

    [Fact]
    public void StartDate_DefaultsToTomorrow()
    {
        // Assert
        Assert.Equal(DateTime.Today.AddDays(1), _viewModel.StartDate);
    }

    [Fact]
    public void EndDate_DefaultsToTwoDaysFromNow()
    {
        // Assert
        Assert.Equal(DateTime.Today.AddDays(2), _viewModel.EndDate);
    }

    [Fact]
    public void MinDate_IsTomorrow()
    {
        // Assert
        Assert.Equal(DateTime.Today.AddDays(1), _viewModel.MinDate);
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
    public async Task SubmitRental_WhenValidDates_CallsRentalService()
    {
        // Arrange
        _viewModel.ItemId = 1;
        _viewModel.StartDate = DateTime.Today.AddDays(1);
        _viewModel.EndDate = DateTime.Today.AddDays(3);
        _mockRentalService
            .Setup(s => s.CreateRentalAsync(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync((Rental?)null);

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        _mockRentalService.Verify(
            s => s.CreateRentalAsync(
                1,
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()),
            Times.Once);
    }

    [Fact]
    public async Task SubmitRental_WhenServiceReturnsNull_SetsErrorMessage()
    {
        // Arrange
        _viewModel.ItemId = 1;
        _viewModel.StartDate = DateTime.Today.AddDays(1);
        _viewModel.EndDate = DateTime.Today.AddDays(3);
        _mockRentalService
            .Setup(s => s.CreateRentalAsync(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>()))
            .ReturnsAsync((Rental?)null);

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }
}