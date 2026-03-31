using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

public class Item
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("dailyRate")]
    public decimal DailyRate { get; set; }

    [JsonPropertyName("categoryId")]
    public int CategoryId { get; set; }

    // API returns category as a plain string e.g. "Tools"
    [JsonPropertyName("category")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("ownerId")]
    public int OwnerId { get; set; }

    // API returns owner as a plain string e.g. "Sarah Smith"
    [JsonPropertyName("ownerName")]
    public string OwnerName { get; set; } = string.Empty;

    [JsonPropertyName("isAvailable")]
    public bool IsAvailable { get; set; } = true;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("averageRating")]
    public double? AverageRating { get; set; }

    [JsonPropertyName("ownerRating")]
    public double? OwnerRating { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("distance")]
    public double? Distance { get; set; }

// Only shown when distance is available (i.e. on the nearby page)
    public string FormattedDistance => Distance.HasValue
        ? $"{Distance.Value:F1} km away"
        : string.Empty;

    // Computed properties for the UI — not from the API
    public string AvailabilityText => IsAvailable ? "Available" : "Unavailable";
    public string FormattedRating => AverageRating.HasValue 
        ? $"{AverageRating.Value:F1} ★" 
        : "No ratings yet";
}