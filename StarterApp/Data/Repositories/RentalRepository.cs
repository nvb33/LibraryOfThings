using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Data.Repositories;

/// <summary>
/// Concrete implementation of <see cref="IRentalRepository"/> that delegates
/// all data access operations to the <see cref="IApiService"/>.
/// Abstracts the API communication layer from ViewModels, allowing the
/// data source to be changed without modifying ViewModel code.
/// </summary>
public class RentalRepository : IRentalRepository
{
    private readonly IApiService _apiService;

    /// <summary>
    /// Initialises a new instance of <see cref="RentalRepository"/>.
    /// </summary>
    /// <param name="apiService">The API service used to retrieve and update rental data.</param>
    public RentalRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Rental>> GetAllAsync()
    {
        var outgoing = await _apiService.GetOutgoingRentalsAsync();
        var incoming = await _apiService.GetIncomingRentalsAsync();
        return outgoing.Concat(incoming);
    }

    /// <inheritdoc/>
    /// <remarks>Get by ID is not supported by the current API. Always returns null.</remarks>
    public Task<Rental?> GetByIdAsync(int id)
    {
        return Task.FromResult<Rental?>(null);
    }

    /// <inheritdoc/>
    public async Task<Rental?> AddAsync(Rental entity)
    {
        return await _apiService.CreateRentalAsync(
            entity.ItemId,
            entity.StartDate,
            entity.EndDate);
    }

    /// <inheritdoc/>
    /// <remarks>General update is not supported. Use <see cref="UpdateStatusAsync"/> instead.</remarks>
    public Task<bool> UpdateAsync(int id, Rental entity)
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
    public async Task<IEnumerable<Rental>> GetOutgoingAsync()
    {
        return await _apiService.GetOutgoingRentalsAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Rental>> GetIncomingAsync()
    {
        return await _apiService.GetIncomingRentalsAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateStatusAsync(int rentalId, string status)
    {
        return await _apiService.UpdateRentalStatusAsync(rentalId, status);
    }
}