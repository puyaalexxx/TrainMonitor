namespace TrainMonitor.Models;

public sealed class Train
{
    public int Id { get; set; }
    public required string TrainName { get; set; }
    public required string TrainNumber { get; set; }
    //arrivingTime
    public int DelayTime { get; set; }

    public DateTime DepartureTime { get; set; }
    public DateTime? ArrivalTime { get; set; }

    // Navigation Property
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
}
