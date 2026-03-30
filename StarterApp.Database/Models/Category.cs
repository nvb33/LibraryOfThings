using System.Text.Json.Serialization;

namespace StarterApp.Database.Models;

public class Category
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    // Useful for displaying in a Picker
    public override string ToString() => Name;
}