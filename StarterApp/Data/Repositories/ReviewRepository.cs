using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Data.Repositories;

/// <summary>
/// Concrete implementation of <see cref="IReviewRepository"/> that delegates
/// all data access operations to the <see cref="IApiService"/>.
/// Abstracts the API communication layer from ViewModels, allowing the
/// data source to be changed without modifying ViewModel code.
/// </summary>
public class ReviewRepository : IReviewRepository
{
    private readonly IApiService _apiService;

    /// <summary>
    /// Initialises a new instance of <see cref="ReviewRepository"/>.
    /// </summary>
    /// <param name="apiService">The API service used to retrieve and submit review data.</param>
    public ReviewRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <inheritdoc/>
    /// <remarks>Get all reviews is not supported by the current API. Returns an empty collection.</remarks>
    public Task<IEnumerable<Review>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Review>>(new List<Review>());
    }

    /// <inheritdoc/>
    /// <remarks>Get review by ID is not supported by the current API. Always returns null.</remarks>
    public Task<Review?> GetByIdAsync(int id)
    {
        return Task.FromResult<Review?>(null);
    }

    /// <inheritdoc/>
    public async Task<Review?> AddAsync(Review entity)
    {
        return await _apiService.SubmitReviewAsync(
            entity.RentalId,
            entity.Rating,
            entity.Comment);
    }

    /// <inheritdoc/>
    /// <remarks>Update is not supported by the current API. Always returns false.</remarks>
    public Task<bool> UpdateAsync(int id, Review entity)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    /// <remarks>Delete is not supported by the current API. Always returns false.</remarks>
    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Review>> GetByItemAsync(int itemId)
    {
        var result = await _apiService.GetItemReviewsAsync(itemId);
        return result.Reviews;
    }

    /// <inheritdoc/>
    public async Task<double> GetAverageRatingAsync(int itemId)
    {
        var result = await _apiService.GetItemReviewsAsync(itemId);
        return result.AverageRating ?? 0.0;
    }

    /// <inheritdoc/>
    public async Task<int> GetTotalReviewsAsync(int itemId)
    {
        var result = await _apiService.GetItemReviewsAsync(itemId);
        return result.TotalReviews;
    }
}