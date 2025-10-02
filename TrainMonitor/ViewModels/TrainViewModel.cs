namespace TrainMonitor.ViewModels;

public class TrainViewModel
{
    public int Id { get; set; }
    public string TrainName { get; set; }
    public string TrainNumber { get; set; }
    public int DelayTime { get; set; }
    public string NextStation { get; set; }
    public bool HasIncidentHistory { get; set; }
}
