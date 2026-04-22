using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;

namespace StarterApp.Services;

/// <summary>
/// Implements review business logic, validating eligibility and rating rules
/// before delegating data operations to IReviewRepository.
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    /// <summary>
    /// Initialises a new instance of <see cref="ReviewService"/>.
    /// </summary>
    /// <param name="reviewRepository">The repository used for review data access.</param>
    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Review>> GetReviewsForItemAsync(int itemId)
    {
        return await _reviewRepository.GetByItemAsync(itemId);
    }

    /// <inheritdoc/>
    public async Task<double> GetAverageRatingAsync(int itemId)
    {
        return await _reviewRepository.GetAverageRatingAsync(itemId);
    }

    /// <inheritdoc/>
    public async Task<int> GetTotalReviewsAsync(int itemId)
    {
        return await _reviewRepository.GetTotalReviewsAsync(itemId);
    }

    /// <inheritdoc/>
    public async Task<Review?> SubmitReviewAsync(Rental rental, int rating, string comment)
    {
        if (!CanSubmitReview(rental))
            throw new InvalidOperationException(
                "Reviews can only be submitted for completed rentals.");

        if (!IsValidRating(rating))
            throw new ArgumentException("Rating must be between 1 and 5.");

        if (comment.Length > 500)
            throw new ArgumentException("Comment must be 500 characters or fewer.");

        var review = new Review
        {
            RentalId = rental.Id,
            Rating = rating,
            Comment = comment
        };

        return await _reviewRepository.AddAsync(review);
    }

    /// <inheritdoc/>
    public bool CanSubmitReview(Rental rental) =>
        rental.Status == "Completed";

    /// <inheritdoc/>
    public bool IsValidRating(int rating) =>
        rating >= 1 && rating <= 5;
}