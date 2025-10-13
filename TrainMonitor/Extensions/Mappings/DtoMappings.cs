// Ignore Spelling: Dto

using TrainMonitor.DTOs;
using TrainMonitor.ViewModels;

namespace TrainMonitor.Extensions.Mappings;

internal static class DtoMappings
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

