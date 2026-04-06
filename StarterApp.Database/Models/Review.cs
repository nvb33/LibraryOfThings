using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

public class Review
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("rentalId")]
    public int RentalId { get; set; }

    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    [JsonPropertyName("itemTitle")]
    public string ItemTitle { get; set; } = string.Empty;

    [JsonPropertyName("reviewerId")]
    public int ReviewerId { get; set; }

    [JsonPropertyName("reviewerName")]
    public string ReviewerName { get; set; } = string.Empty;

    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    // Computed properties for the UI
    public string FormattedRating => new string('★', Rating) + new string('☆', 5 - Rating);
    public string FormattedDate => CreatedAt.HasValue
        ? CreatedAt.Value.ToString("dd MMM yyyy")
        : string.Empty;
}