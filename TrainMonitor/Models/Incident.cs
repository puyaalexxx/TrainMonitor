namespace TrainMonitor.Models;

public sealed class Incident
{
    public int Id { get; set; }
    public required string TrainNumber { get; set; }
    public required string Username { get; set; }
    public required string Reason { get; set; }

    public string? AdditionalComment { get; set; }


    // Foreign Key
    public int TrainId { get; set; }
    // Navigation Property
    public Train Train { get; set; } = default!;

}

