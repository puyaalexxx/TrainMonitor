namespace TrainMonitor.DTOs;

public sealed record NextStopDto(
    CoordinatesDto Coordinates,
    string StationName,
    DateTime DepartureTime,
    DateTime? ArrivalTime,
    int? GpsId,
    int? RoutesId,
    bool? HasWaitingRoom,
    bool? HasWC,
    bool? HasCoffeeMachine,
    string? StationNotes,
    string? Address,
    int PlatformViewID
);
