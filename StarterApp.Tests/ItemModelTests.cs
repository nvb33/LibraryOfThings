using StarterApp.Database.Models;

namespace StarterApp.Tests;

public class ItemModelTests
{
    [Fact]
    public void DailyRate_StoresCorrectValue()
    {
        // Arrange
        var item = new Item { DailyRate = 10.00m };

        // Act
        var rate = item.DailyRate;

        // Assert
        Assert.Equal(10.00m, rate);
    }

    [Fact]
    public void AvailabilityText_WhenAvailable_ReturnsAvailable()
    {
        // Arrange
        var item = new Item { IsAvailable = true };

        // Act
        var text = item.AvailabilityText;

        // Assert
        Assert.Equal("Available", text);
    }

    [Fact]
    public void AvailabilityText_WhenNotAvailable_ReturnsUnavailable()
    {
        // Arrange
        var item = new Item { IsAvailable = false };

        // Act
        var text = item.AvailabilityText;

        // Assert
        Assert.Equal("Unavailable", text);
    }

    [Fact]
    public void CategoryName_ReturnsCorrectValue()
    {
        // Arrange
        var item = new Item { CategoryName = "Tools" };

        // Act
        var name = item.CategoryName;

        // Assert
        Assert.Equal("Tools", name);
    }

    [Fact]
    public void FormattedDistance_WhenNull_ReturnsEmptyString()
    {
        // Arrange
        var item = new Item { Distance = null };

        // Act
        var distance = item.FormattedDistance;

        // Assert
        Assert.Equal(string.Empty, distance);
    }

    [Fact]
    public void FormattedDistance_WhenSet_ReturnsFormattedString()
    {
        // Arrange
        var item = new Item { Distance = 2.5 };

        // Act
        var distance = item.FormattedDistance;

        // Assert
        Assert.Contains("2.5", distance);
    }
}