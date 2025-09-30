namespace TrainMonitor.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public required string TrainNumber { get; set; }
        public required string Username { get; set; }
        public required string Reason { get; set; }
        public required string Comment { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
