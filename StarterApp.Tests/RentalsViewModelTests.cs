using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.ViewModels;

namespace StarterApp.Tests;

public class RentalsViewModelTests
{
    private readonly Mock<IRentalRepository> _mockRentalRepository;
    private readonly RentalsViewModel _viewModel;

    public RentalsViewModelTests()
    {
        _mockRentalRepository = new Mock<IRentalRepository>();
        _viewModel = new RentalsViewModel(_mockRentalRepository.Object);
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
        _mockRentalRepository.Setup(r => r.GetOutgoingAsync()).ReturnsAsync(rentals);
        _mockRentalRepository.Setup(r => r.GetIncomingAsync()).ReturnsAsync(new List<Rental>());

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
        _mockRentalRepository.Setup(r => r.GetOutgoingAsync()).ReturnsAsync(new List<Rental>());
        _mockRentalRepository.Setup(r => r.GetIncomingAsync()).ReturnsAsync(rentals);

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.Single(_viewModel.IncomingRentals);
    }

    [Fact]
    public async Task LoadRentals_WhenRepositoryFails_SetsErrorMessage()
    {
        // Arrange
        _mockRentalRepository
            .Setup(r => r.GetOutgoingAsync())
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
        _mockRentalRepository.Setup(r => r.GetOutgoingAsync()).ReturnsAsync(new List<Rental>());
        _mockRentalRepository.Setup(r => r.GetIncomingAsync()).ReturnsAsync(new List<Rental>());

        // Act
        await _viewModel.LoadRentalsCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.IsBusy);
    }
}