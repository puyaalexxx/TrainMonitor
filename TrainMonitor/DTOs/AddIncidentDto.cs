namespace TrainMonitor.DTOs;

public sealed record AddIncidentDto
{
    public required string TrainId { get; init; }

    public required string Username { get; init; }

    public required string Reason { get; init; }

    public string? Comment { get; init; }
}
