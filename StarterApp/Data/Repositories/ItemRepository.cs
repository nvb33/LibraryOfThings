using StarterApp.Database.Data.Repositories;
using StarterApp.Database.Models;
using StarterApp.Services;

namespace StarterApp.Data.Repositories;

/// <summary>
/// Concrete implementation of <see cref="IItemRepository"/> that delegates
/// all data access operations to the <see cref="IApiService"/>.
/// Abstracts the API communication layer from ViewModels, allowing the
/// data source to be changed without modifying ViewModel code.
/// </summary>
public class ItemRepository : IItemRepository
{
    private readonly IApiService _apiService;

    /// <summary>
    /// Initialises a new instance of <see cref="ItemRepository"/>.
    /// </summary>
    /// <param name="apiService">The API service used to retrieve and persist item data.</param>
    public ItemRepository(IApiService apiService)
    {
        _apiService = apiService;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Item>> GetAllAsync()
    {
        return await _apiService.GetItemsAsync();
    }

    /// <inheritdoc/>
    public async Task<Item?> GetByIdAsync(int id)
    {
        return await _apiService.GetItemAsync(id);
    }

    /// <inheritdoc/>
    public async Task<Item?> AddAsync(Item entity)
    {
        return await _apiService.CreateItemAsync(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(int id, Item entity)
    {
        var result = await _apiService.UpdateItemAsync(id, entity);
        return result != null;
    }

    /// <inheritdoc/>
    /// <remarks>Delete is not supported by the current API. Always returns false.</remarks>
    public Task<bool> DeleteAsync(int id)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Item>> GetNearbyAsync(double lat, double lon, double radiusKm = 5)
    {
        return await _apiService.GetNearbyItemsAsync(lat, lon, radiusKm);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _apiService.GetCategoriesAsync();
    }
}