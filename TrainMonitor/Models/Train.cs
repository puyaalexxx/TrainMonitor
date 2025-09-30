namespace TrainMonitor.Models
{
    public class Train
    {
        public int Id { get; set; }
        public required string TrainName { get; set; }
        public required string TrainNumber { get; set; }
        public int DelayMinutes { get; set; }
        public required string NextStop { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
