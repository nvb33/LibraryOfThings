using Moq;
using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Tests;

public class RentalServiceTests
{
    private readonly Mock<IRentalRepository> _mockRepository;
    private readonly RentalService _service;

    public RentalServiceTests()
    {
        _mockRepository = new Mock<IRentalRepository>();
        _service = new RentalService(_mockRepository.Object);
    }

    [Fact]
    public void CanApprove_WhenRequested_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Requested" };

        // Assert
        Assert.True(_service.CanApprove(rental));
    }

    [Fact]
    public void CanApprove_WhenNotRequested_ReturnsFalse()
    {
        // Arrange
        var rental = new Rental { Status = "Approved" };

        // Assert
        Assert.False(_service.CanApprove(rental));
    }

    [Fact]
    public void CanReject_WhenRequested_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Requested" };

        // Assert
        Assert.True(_service.CanReject(rental));
    }

    [Fact]
    public void CanMarkOutForRent_WhenApproved_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Approved" };

        // Assert
        Assert.True(_service.CanMarkOutForRent(rental));
    }

    [Fact]
    public void CanMarkOutForRent_WhenNotApproved_ReturnsFalse()
    {
        // Arrange
        var rental = new Rental { Status = "Requested" };

        // Assert
        Assert.False(_service.CanMarkOutForRent(rental));
    }

    [Fact]
    public void CanMarkReturned_WhenOutForRent_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Out for Rent" };

        // Assert
        Assert.True(_service.CanMarkReturned(rental));
    }

    [Fact]
    public void CanMarkReturned_WhenOverdue_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Overdue" };

        // Assert
        Assert.True(_service.CanMarkReturned(rental));
    }

    [Fact]
    public void CanComplete_WhenReturned_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Returned" };

        // Assert
        Assert.True(_service.CanComplete(rental));
    }

    [Fact]
    public void CanReview_WhenCompleted_ReturnsTrue()
    {
        // Arrange
        var rental = new Rental { Status = "Completed" };

        // Assert
        Assert.True(_service.CanReview(rental));
    }

    [Fact]
    public void CanReview_WhenNotCompleted_ReturnsFalse()
    {
        // Arrange
        var rental = new Rental { Status = "Returned" };

        // Assert
        Assert.False(_service.CanReview(rental));
    }

    [Fact]
    public void CalculateTotalPrice_ReturnsCorrectAmount()
    {
        // Arrange
        var startDate = new DateTime(2026, 5, 1);
        var endDate = new DateTime(2026, 5, 8);

        // Act
        var total = _service.CalculateTotalPrice(10.00m, startDate, endDate);

        // Assert
        Assert.Equal(70.00m, total);
    }

    [Fact]
    public async Task ApproveRentalAsync_WhenCanApprove_CallsRepository()
    {
        // Arrange
        var rental = new Rental { Id = 1, Status = "Requested" };
        _mockRepository.Setup(r => r.UpdateStatusAsync(1, "Approved")).ReturnsAsync(true);

        // Act
        var result = await _service.ApproveRentalAsync(rental);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.UpdateStatusAsync(1, "Approved"), Times.Once);
    }

    [Fact]
    public async Task ApproveRentalAsync_WhenCannotApprove_ReturnsFalse()
    {
        // Arrange
        var rental = new Rental { Id = 1, Status = "Approved" };

        // Act
        var result = await _service.ApproveRentalAsync(rental);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.UpdateStatusAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateRentalAsync_WhenEndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var start = DateTime.Today.AddDays(5);
        var end = DateTime.Today.AddDays(3);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.CreateRentalAsync(1, start, end));
    }
}