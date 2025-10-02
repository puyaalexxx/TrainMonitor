using System.Text.Json.Serialization;

namespace TrainMonitor.Helpers.Json;

public class TrainReturnValue
{
    [JsonPropertyName("id")]
    public required string TrainId { get; set; }

    [JsonPropertyName("train")]
    public string TrainNumber { get; set; } = string.Empty;

    [JsonPropertyName("arrivingTime")]
    public int DelayTime { get; set; }

    [JsonPropertyName("updaterTimeStamp")]
    public DateTime? LastUpdatedTime { get; set; }

    [JsonPropertyName("nextStopObj")]
    public NextStop NextStop { get; set; } = default!;

    // public DateTime DepartureTime { get; set; }
    // public DateTime? ArrivalTime { get; set; }
}
