using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

/// <summary>
/// Represents a review submitted by a borrower after completing a rental.
/// Maps directly to the review object returned by the REST API.
/// </summary>
public class Review
{
    /// <summary>Gets or sets the unique identifier of the review.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Gets or sets the unique identifier of the rental this review relates to.</summary>
    [JsonPropertyName("rentalId")]
    public int RentalId { get; set; }

    /// <summary>Gets or sets the unique identifier of the item that was reviewed.</summary>
    [JsonPropertyName("itemId")]
    public int ItemId { get; set; }

    /// <summary>Gets or sets the title of the item that was reviewed.</summary>
    [JsonPropertyName("itemTitle")]
    public string ItemTitle { get; set; } = string.Empty;

    /// <summary>Gets or sets the unique identifier of the user who submitted the review.</summary>
    [JsonPropertyName("reviewerId")]
    public int ReviewerId { get; set; }

    /// <summary>Gets or sets the full name of the user who submitted the review.</summary>
    [JsonPropertyName("reviewerName")]
    public string ReviewerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the numeric rating given by the reviewer.
    /// Must be an integer between 1 and 5 inclusive.
    /// </summary>
    [JsonPropertyName("rating")]
    public int Rating { get; set; }

    /// <summary>
    /// Gets or sets the optional written comment accompanying the rating.
    /// Maximum length is 500 characters.
    /// </summary>
    [JsonPropertyName("comment")]
    public string Comment { get; set; } = string.Empty;

    /// <summary>Gets or sets the date and time the review was submitted.</summary>
    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets a visual star representation of the rating using filled and empty star characters.
    /// Example: a rating of 3 returns ★★★☆☆
    /// </summary>
    public string FormattedRating => new string('★', Rating) + new string('☆', 5 - Rating);

    /// <summary>
    /// Gets the review submission date formatted as a human-readable string.
    /// Example: 15 Jun 2025. Returns an empty string if the date is not set.
    /// </summary>
    public string FormattedDate => CreatedAt.HasValue
        ? CreatedAt.Value.ToString("dd MMM yyyy")
        : string.Empty;
}