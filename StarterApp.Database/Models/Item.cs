using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

/// <summary>
/// Represents an item available for rent in the Library of Things marketplace.
/// Maps directly to the item object returned by the REST API.
/// </summary>
public class Item
{
    /// <summary>Gets or sets the unique identifier of the item.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Gets or sets the title of the item.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the detailed description of the item.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the daily rental rate in GBP.</summary>
    [JsonPropertyName("dailyRate")]
    public decimal DailyRate { get; set; }

    /// <summary>Gets or sets the unique identifier of the item's category.</summary>
    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Gets or sets the category name as returned by the API.
    /// The API returns this as a plain string rather than a nested object.
    /// Example: "Tools"
    /// </summary>
    [JsonPropertyName("category")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Gets or sets the unique identifier of the item owner.</summary>
    [JsonPropertyName("ownerId")]
    public int OwnerId { get; set; }

    /// <summary>
    /// Gets or sets the full name of the item owner as returned by the API.
    /// The API returns this as a plain string rather than a nested object.
    /// Example: "Sarah Smith"
    /// </summary>
    [JsonPropertyName("ownerName")]
    public string OwnerName { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the item is currently available to rent.</summary>
    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; } = true;

    /// <summary>Gets or sets the latitude coordinate of the item's location.</summary>
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    /// <summary>Gets or sets the longitude coordinate of the item's location.</summary>
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    /// <summary>Gets or sets the average rating of the item across all reviews, or null if unrated.</summary>
    [JsonPropertyName("averageRating")]
    public double? AverageRating { get; set; }

    /// <summary>Gets or sets the average rating of the item owner across all their items, or null if unrated.</summary>
    [JsonPropertyName("ownerRating")]
    public double? OwnerRating { get; set; }

    /// <summary>Gets or sets the date and time the item listing was created.</summary>
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the distance from the user's location to the item in kilometres.
    /// Only populated when the item is retrieved via the nearby items endpoint.
    /// </summary>
    [JsonPropertyName("distance")]
    public double? Distance { get; set; }

    /// <summary>
    /// Gets a formatted distance string for display on the nearby items page.
    /// Returns an empty string when distance is not available.
    /// Example: "2.5 km away"
    /// </summary>
    public string FormattedDistance => Distance.HasValue
        ? $"{Distance.Value:F1} km away"
        : string.Empty;

    /// <summary>
    /// Gets a human-readable availability status for display in the UI.
    /// Returns "Available" or "Unavailable" based on the IsAvailable property.
    /// </summary>
    public string AvailabilityText => IsAvailable ? "Available" : "Unavailable";

    /// <summary>
    /// Gets a formatted average rating string for display in the UI.
    /// Returns "No ratings yet" when no reviews exist.
    /// Example: "4.5 ★"
    /// </summary>
    public string FormattedRating => AverageRating.HasValue
        ? $"{AverageRating.Value:F1} ★"
        : "No ratings yet";
}