using System.Text.Json.Serialization;

namespace TrainMonitor.Helpers.Json;

public sealed class Root
{
    [JsonPropertyName("data")]
    public List<TrainJson> Data { get; set; } = new List<TrainJson>();
}
