namespace TrainMonitor.ViewModels;

public record TrainsWithIncidentsViewModel
{
    public required string TrainId { get; init; }
    public required string TrainNumber { get; init; }

    public required string TrainName { get; init; }

    public required List<IncidentViewModel> Incidents { get; init; }
}
