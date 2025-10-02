using System.Text.Json.Serialization;

namespace TrainMonitor.Helpers.Json;

public sealed class TrainJson
{
    [JsonPropertyName("name")]
    public string TrainName { get; set; } = string.Empty;

    [JsonPropertyName("returnValue")]
    public TrainReturnValue ReturnValue { get; set; } = default!;
}
