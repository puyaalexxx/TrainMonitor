namespace TrainMonitor.ViewModels;

public sealed record TrainViewModel
{
    public required string TrainId { get; init; }
    public required string TrainName { get; init; }
    public required string TrainNumber { get; init; }
    public int DelayTime { get; init; }
    public required string LastUpdatedTime { get; init; }
    public required string NextStation { get; init; }
    public bool HasDelay { get; init; }
}
