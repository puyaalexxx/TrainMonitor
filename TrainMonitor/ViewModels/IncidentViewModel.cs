namespace TrainMonitor.ViewModels;

public sealed record IncidentViewModel
{
    public required string Username { get; init; }
    public required string Reason { get; init; }
    public string? Comment { get; init; }
}
