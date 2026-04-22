using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Defines the business logic contract for review operations.
/// Validates review eligibility and delegates data access to the repository.
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Retrieves all reviews for a specific item.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <returns>A collection of reviews for the item.</returns>
    Task<IEnumerable<Review>> GetReviewsForItemAsync(int itemId);

    /// <summary>
    /// Retrieves the average rating for a specific item.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <returns>The average rating, or 0.0 if no reviews exist.</returns>
    Task<double> GetAverageRatingAsync(int itemId);

    /// <summary>
    /// Retrieves the total number of reviews for a specific item.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <returns>The total review count.</returns>
    Task<int> GetTotalReviewsAsync(int itemId);

    /// <summary>
    /// Submits a review for a completed rental after validating eligibility.
    /// </summary>
    /// <param name="rental">The completed rental being reviewed.</param>
    /// <param name="rating">The rating value between 1 and 5.</param>
    /// <param name="comment">An optional comment up to 500 characters.</param>
    /// <returns>The created review, or null if submission failed.</returns>
    Task<Review?> SubmitReviewAsync(Rental rental, int rating, string comment);

    /// <summary>
    /// Determines whether a review can be submitted for the given rental.
    /// A review is only valid for completed rentals.
    /// </summary>
    /// <param name="rental">The rental to check.</param>
    /// <returns>True if the rental is eligible for review.</returns>
    bool CanSubmitReview(Rental rental);

    /// <summary>
    /// Validates that a rating value is within the acceptable range of 1 to 5.
    /// </summary>
    /// <param name="rating">The rating value to validate.</param>
    /// <returns>True if the rating is valid.</returns>
    bool IsValidRating(int rating);
}