using System.Text.Json.Serialization;

namespace Stories.API.Models;

public class Story
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}
