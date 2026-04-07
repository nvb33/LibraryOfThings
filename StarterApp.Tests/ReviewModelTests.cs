using StarterApp.Database.Models;

namespace StarterApp.Tests;

public class ReviewModelTests
{
    [Fact]
    public void StarDisplay_WhenRating5_ShowsFiveFilledStars()
    {
        // Arrange
        var review = new Review { Rating = 5 };

        // Act
        var stars = review.FormattedRating;

        // Assert
        Assert.Equal("★★★★★", stars);
    }

    [Fact]
    public void StarDisplay_WhenRating3_ShowsThreeFilledTwoEmpty()
    {
        // Arrange
        var review = new Review { Rating = 3 };

        // Act
        var stars = review.FormattedRating;

        // Assert
        Assert.Equal("★★★☆☆", stars);
    }

    [Fact]
    public void StarDisplay_WhenRating1_ShowsOneFilledFourEmpty()
    {
        // Arrange
        var review = new Review { Rating = 1 };

        // Act
        var stars = review.FormattedRating;

        // Assert
        Assert.Equal("★☆☆☆☆", stars);
    }

    [Fact]
    public void FormattedDate_WhenCreatedAtSet_ReturnsFormattedString()
    {
        // Arrange
        var review = new Review { CreatedAt = new DateTime(2025, 6, 15) };

        // Act
        var date = review.FormattedDate;

        // Assert
        Assert.Equal("15 Jun 2025", date);
    }

    [Fact]
    public void FormattedDate_WhenCreatedAtNull_ReturnsEmptyString()
    {
        // Arrange
        var review = new Review { CreatedAt = null };

        // Act
        var date = review.FormattedDate;

        // Assert
        Assert.Equal(string.Empty, date);
    }
}