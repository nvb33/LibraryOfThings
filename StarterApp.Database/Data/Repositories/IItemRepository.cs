using StarterApp.Database.Models;

namespace StarterApp.Database.Data.Repositories;

/// <summary>
/// Defines the repository contract for item data access operations,
/// extending the generic repository with item-specific queries.
/// </summary>
public interface IItemRepository : IRepository<Item>
{
    /// <summary>
    /// Retrieves items within a given radius of a geographic location.
    /// </summary>
    /// <param name="lat">Latitude of the search centre point.</param>
    /// <param name="lon">Longitude of the search centre point.</param>
    /// <param name="radiusKm">Search radius in kilometres. Defaults to 5km.</param>
    /// <returns>A collection of nearby items sorted by distance.</returns>
    Task<IEnumerable<Item>> GetNearbyAsync(double lat, double lon, double radiusKm = 5);

    /// <summary>
    /// Retrieves all available item categories.
    /// </summary>
    /// <returns>A collection of all categories.</returns>
    Task<IEnumerable<Category>> GetCategoriesAsync();
}