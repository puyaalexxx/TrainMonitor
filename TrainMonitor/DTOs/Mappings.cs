using TrainMonitor.ViewModels;

namespace TrainMonitor.DTOs;

internal static class Mappings
{
    /// <summary>
    /// Converts an <see cref="AddIncidentViewModel"/> to an <see cref="AddIncidentDto"/>.
    /// </summary>
    /// <param name="addIncidentViewModel">The <see cref="AddIncidentViewModel"/> to convert.</param>
    /// <returns>The converted <see cref="AddIncidentDto"/>.</returns>
    public static AddIncidentDto ToDto(this AddIncidentViewModel addIncidentViewModel)
    {
        return new AddIncidentDto
        {
            TrainId = addIncidentViewModel.TrainId,
            Username = addIncidentViewModel.Username,
            Reason = addIncidentViewModel.Reason,
            Comment = addIncidentViewModel.Comment
        };
    }
}

