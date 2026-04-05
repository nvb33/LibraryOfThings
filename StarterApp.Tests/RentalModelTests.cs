using StarterApp.Database.Models;

namespace StarterApp.Tests;

public class RentalModelTests
{
    [Fact]
    public void StatusColour_WhenRequested_ReturnsOrange()
    {
        // Arrange
        var rental = new Rental { Status = "Requested" };

        // Act
        var colour = rental.StatusColour;

        // Assert
        Assert.Equal("#FF9800", colour);
    }

    [Fact]
    public void StatusColour_WhenApproved_ReturnsGreen()
    {
        // Arrange
        var rental = new Rental { Status = "Approved" };

        // Act
        var colour = rental.StatusColour;

        // Assert
        Assert.Equal("#4CAF50", colour);
    }

    [Fact]
    public void StatusColour_WhenRejected_ReturnsRed()
    {
        // Arrange
        var rental = new Rental { Status = "Rejected" };

        // Act
        var colour = rental.StatusColour;

        // Assert
        Assert.Equal("#F44336", colour);
    }

    [Fact]
    public void StatusColour_WhenOutForRent_ReturnsBlue()
    {
        // Arrange
        var rental = new Rental { Status = "Out for Rent" };

        // Act
        var colour = rental.StatusColour;

        // Assert
        Assert.Equal("#2196F3", colour);
    }

    [Fact]
    public void StatusColour_WhenCompleted_ReturnsGrey()
    {
        // Arrange
        var rental = new Rental { Status = "Completed" };

        // Act
        var colour = rental.StatusColour;

        // Assert
        Assert.Equal("#607D8B", colour);
    }

    [Fact]
    public void StatusColour_WhenUnknownStatus_ReturnsGrey()
    {
        // Arrange
        var rental = new Rental { Status = "Unknown" };

        // Act
        var colour = rental.StatusColour;

        // Assert
        Assert.Equal("#607D8B", colour);
    }

    [Fact]
    public void FormattedPrice_ReturnsPoundSign()
    {
        // Arrange
        var rental = new Rental { TotalPrice = 25.50m };

        // Act
        var price = rental.FormattedPrice;

        // Assert
        Assert.Equal("£25.50", price);
    }

    [Fact]
    public void FormattedDates_CombinesStartAndEndDate()
    {
        // Arrange
        var rental = new Rental { StartDate = "2025-06-01", EndDate = "2025-06-07" };

        // Act
        var dates = rental.FormattedDates;

        // Assert
        Assert.Equal("2025-06-01 → 2025-06-07", dates);
    }
}