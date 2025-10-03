using System.ComponentModel.DataAnnotations;

namespace TrainMonitor.ViewModels;

public sealed record AddIncidentViewModel
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
    public string Username { get; init; } = string.Empty;

    [Required(ErrorMessage = "Reason is required")]
    public string Reason { get; init; } = string.Empty;

    public string? Comment { get; init; }
}
