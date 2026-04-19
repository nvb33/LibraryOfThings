using Moq;
using StarterApp.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests;

/// <summary>
/// Unit tests for RentalRepository verifying correct delegation to IApiService.
/// </summary>
public class RentalRepositoryTests
{
    private readonly Mock<IApiService> _mockApiService;
    private readonly RentalRepository _repository;

    public RentalRepositoryTests()
    {
        _mockApiService = new Mock<IApiService>();
        _repository = new RentalRepository(_mockApiService.Object);
    }

    [Fact]
    public async Task GetOutgoingAsync_ReturnsOutgoingRentals()
    {
        // Arrange
        var rentals = new List<Rental>
        {
            new Rental { Id = 1, ItemTitle = "Drill", Status = "Requested" }
        };
        _mockApiService.Setup(s => s.GetOutgoingRentalsAsync()).ReturnsAsync(rentals);

        // Act
        var result = await _repository.GetOutgoingAsync();

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task GetIncomingAsync_ReturnsIncomingRentals()
    {
        // Arrange
        var rentals = new List<Rental>
        {
            new Rental { Id = 2, ItemTitle = "Ladder", Status = "Requested" },
            new Rental { Id = 3, ItemTitle = "Tent", Status = "Approved" }
        };
        _mockApiService.Setup(s => s.GetIncomingRentalsAsync()).ReturnsAsync(rentals);

        // Act
        var result = await _repository.GetIncomingAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task AddAsync_CreatesRentalViaApiService()
    {
        // Arrange
        var rental = new Rental
        {
            ItemId = 1,
            StartDate = "2026-05-01",
            EndDate = "2026-05-07"
        };
        var created = new Rental { Id = 10, ItemId = 1, Status = "Requested" };
        _mockApiService
            .Setup(s => s.CreateRentalAsync(1, "2026-05-01", "2026-05-07"))
            .ReturnsAsync(created);

        // Act
        var result = await _repository.AddAsync(rental);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
    }

    [Fact]
    public async Task UpdateStatusAsync_ReturnsTrueWhenSuccessful()
    {
        // Arrange
        _mockApiService
            .Setup(s => s.UpdateRentalStatusAsync(1, "Approved"))
            .ReturnsAsync(true);

        // Act
        var result = await _repository.UpdateStatusAsync(1, "Approved");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_ReturnsFalseWhenApiFails()
    {
        // Arrange
        _mockApiService
            .Setup(s => s.UpdateRentalStatusAsync(1, "Approved"))
            .ReturnsAsync(false);

        // Act
        var result = await _repository.UpdateStatusAsync(1, "Approved");

        // Assert
        Assert.False(result);
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
    public async Task GetByIdAsync_AlwaysReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }
}