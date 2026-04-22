using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

/// <summary>
/// Defines the repository contract for review data access operations,
/// extending the generic repository with review-specific queries.
/// </summary>
public interface IReviewRepository : IRepository<Review>
{
    /// <summary>
    /// Retrieves all reviews for a specific item along with summary statistics.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <returns>A collection of reviews for the item.</returns>
    Task<IEnumerable<Review>> GetByItemAsync(int itemId);

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
}