using System.Text.Json.Serialization;

namespace TrainMonitor.Helpers.Json;

public class NextStop
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}
