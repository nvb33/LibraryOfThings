using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class RentalsViewModelTests
{
    private readonly Mock<IRentalService> _mockRentalService;
    private readonly RentalsViewModel _viewModel;

    public RentalsViewModelTests()
    {
        _mockRentalService = new Mock<IRentalService>();
        _viewModel = new RentalsViewModel(_mockRentalService.Object);
    }

    [Fact]
    public void ShowingOutgoing_DefaultsToTrue()
    {
        Assert.True(_viewModel.ShowingOutgoing);
    }

    [Fact]
    public void ShowingIncoming_DefaultsToFalse()
    {
        Assert.False(_viewModel.ShowingIncoming);
    }

    [Fact]
    public void ShowIncoming_SetsShowingIncomingTrue()
    {
        // Act
        _viewModel.ShowIncomingCommand.Execute(null);

        // Assert
        Assert.True(_viewModel.ShowingIncoming);
        Assert.False(_viewModel.ShowingOutgoing);
    }

    [Fact]
    public void ShowOutgoing_SetsShowingOutgoingTrue()
    {
        // Arrange
        _viewModel.ShowIncomingCommand.Execute(null);

        // Act
        _viewModel.ShowOutgoingCommand.Execute(null);

        // Assert
        Assert.True(_viewModel.ShowingOutgoing);
        Assert.False(_viewModel.ShowingIncoming);
    }

    [Fact]
    public async Task LoadRentals_PopulatesOutgoingRentals()
    {
        // Arrange
        var rentals = new List<Rental>
        {
            new Rental { Id = 1, ItemTitle = "Drill", Status = "Requested" },
            new Rental { Id = 2, ItemTitle = "Ladder", Status = "Approved" }
        };
        _mockRentalService.Setup(s => s.GetOutgoingRentalsAsync()).ReturnsAsync(rentals);
        _mockRentalService.Setup(s => s.GetIncomingRentalsAsync()).ReturnsAsync(new List<Rental>());

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal(2, _viewModel.OutgoingRentals.Count);
    }

    [Fact]
    public async Task LoadRentals_PopulatesIncomingRentals()
    {
        // Arrange
        var rentals = new List<Rental>
        {
            new Rental { Id = 3, ItemTitle = "Tent", Status = "Requested" }
        };
        _mockRentalService.Setup(s => s.GetOutgoingRentalsAsync()).ReturnsAsync(new List<Rental>());
        _mockRentalService.Setup(s => s.GetIncomingRentalsAsync()).ReturnsAsync(rentals);

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(_viewModel.IncomingRentals);
    }

    [Fact]
    public async Task LoadRentals_WhenServiceFails_SetsErrorMessage()
    {
        // Arrange
        _mockRentalService
            .Setup(s => s.GetOutgoingRentalsAsync())
            .ThrowsAsync(new Exception("Network error"));

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(string.IsNullOrEmpty(_viewModel.ErrorMessage));
    }

    [Fact]
    public async Task LoadRentals_SetsIsBusyFalseAfterCompletion()
    {
        // Arrange
        _mockRentalService.Setup(s => s.GetOutgoingRentalsAsync()).ReturnsAsync(new List<Rental>());
        _mockRentalService.Setup(s => s.GetIncomingRentalsAsync()).ReturnsAsync(new List<Rental>());

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}