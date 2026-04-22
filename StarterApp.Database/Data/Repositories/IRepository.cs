namespace StarterApp.Database.Data.Repositories;

/// <summary>
/// Defines a generic repository interface providing standard data access
/// operations for any entity type. Implementations abstract the underlying
/// data source from the rest of the application.
/// </summary>
/// <typeparam name="T">The entity type this repository manages.</typeparam>
public interface IRepository<T>
{
    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An enumerable collection of all entities.</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Retrieves a single entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>The matching entity, or null if not found.</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Adds a new entity to the data source.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>The created entity with any server-assigned values, or null if creation failed.</returns>
    Task<T?> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity in the data source.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update.</param>
    /// <param name="entity">The updated entity data.</param>
    /// <returns>True if the update succeeded, false otherwise.</returns>
    Task<bool> UpdateAsync(int id, T entity);

    /// <summary>
    /// Deletes an entity from the data source by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns>True if deletion succeeded, false otherwise.</returns>
    Task<bool> DeleteAsync(int id);
}