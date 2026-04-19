using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class RequestRentalViewModelTests
{
    private readonly Mock<IRentalRepository> _mockRentalRepository;
    private readonly RequestRentalViewModel _viewModel;

    public RequestRentalViewModelTests()
    {
        _mockRentalRepository = new Mock<IRentalRepository>();
        _viewModel = new RequestRentalViewModel(_mockRentalRepository.Object);
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
    public async Task SubmitRental_WhenValidDates_CallsRepository()
    {
        // Arrange
        _viewModel.ItemId = 1;
        _viewModel.StartDate = DateTime.Today.AddDays(1);
        _viewModel.EndDate = DateTime.Today.AddDays(3);
        _mockRentalRepository
            .Setup(r => r.AddAsync(It.IsAny<Rental>()))
            .ReturnsAsync((Rental?)null);

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        _mockRentalRepository.Verify(
            r => r.AddAsync(It.Is<Rental>(rental =>
                rental.ItemId == 1 &&
                rental.StartDate == DateTime.Today.AddDays(1).ToString("yyyy-MM-dd") &&
                rental.EndDate == DateTime.Today.AddDays(3).ToString("yyyy-MM-dd"))),
            Times.Once);
    }

    [Fact]
    public async Task SubmitRental_WhenRepositoryReturnsNull_SetsErrorMessage()
    {
        // Arrange
        _viewModel.ItemId = 1;
        _viewModel.StartDate = DateTime.Today.AddDays(1);
        _viewModel.EndDate = DateTime.Today.AddDays(3);
        _mockRentalRepository
            .Setup(r => r.AddAsync(It.IsAny<Rental>()))
            .ReturnsAsync((Rental?)null);

        // Act
        await _viewModel.SubmitRentalCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }
}