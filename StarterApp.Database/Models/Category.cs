using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

/// <summary>
/// Represents an item category used to classify rental items.
/// Maps directly to the category object returned by the REST API.
/// </summary>
public class Category
{
    /// <summary>Gets or sets the unique identifier of the category.</summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }

    /// <summary>Gets or sets the display name of the category. Example: "Tools"</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL-friendly slug for the category.
    /// Example: "power-tools"
    /// </summary>
    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Returns the category name as a string, enabling this class to be used
    /// directly in a MAUI Picker without a custom display binding.
    /// </summary>
    /// <returns>The category name.</returns>
    public override string ToString() => Name;
}