namespace TrainMonitor.Models;

public sealed class Train
{
    public string Id { get; set; }
    public required string TrainName { get; set; }
    public required string TrainNumber { get; set; }
    //arrivingTime
    public int DelayTime { get; set; }

    // Navigation Property
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
}
