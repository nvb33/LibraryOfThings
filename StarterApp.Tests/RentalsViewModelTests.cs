using Moq;
using StarterApp.Database.Models;
using StarterApp.Services;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class RentalsViewModelTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly RentalsViewModel _viewModel;

    public RentalsViewModelTests()
    {
        _mockApiService = new Mock<IApiService>();
        _viewModel = new RentalsViewModel(_mockApiService.Object);
    }

    [Fact]
    public void ShowingOutgoing_DefaultsToTrue()
    {
        // Assert
        Assert.True(_viewModel.ShowingOutgoing);
    }

    [Fact]
    public void ShowingIncoming_DefaultsToFalse()
    {
        // Assert
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
        // Arrange — switch to incoming first
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
        _mockApiService.Setup(s => s.GetOutgoingRentalsAsync()).ReturnsAsync(rentals);
        _mockApiService.Setup(s => s.GetIncomingRentalsAsync()).ReturnsAsync(new List<Rental>());

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
        _mockApiService.Setup(s => s.GetOutgoingRentalsAsync()).ReturnsAsync(new List<Rental>());
        _mockApiService.Setup(s => s.GetIncomingRentalsAsync()).ReturnsAsync(rentals);

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(_viewModel.IncomingRentals);
    }

    [Fact]
    public async Task LoadRentals_WhenApiFails_SetsErrorMessage()
    {
        // Arrange
        _mockApiService
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
        _mockApiService.Setup(s => s.GetOutgoingRentalsAsync()).ReturnsAsync(new List<Rental>());
        _mockApiService.Setup(s => s.GetIncomingRentalsAsync()).ReturnsAsync(new List<Rental>());

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}