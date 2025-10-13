// Ignore Spelling: dto

using TrainMonitor.DTOs;
using TrainMonitor.Models;

namespace TrainMonitor.Extensions.Mappings
{
    internal static class ModelMappings
    {
        /// <summary>
        /// Converts an <see cref="AddIncidentDto"/> to an <see cref="Incident"/> model.
        /// </summary>
        /// <param name="addIncidentDto">The <see cref="AddIncidentDto"/> to convert.</param>
        /// <returns>The converted <see cref="Incident"/> model.</returns>
        public static Incident ToModel(this AddIncidentDto addIncidentDto)
        {
            return new Incident
            {
                TrainId = addIncidentDto.TrainId,
                Username = addIncidentDto.Username,
                Reason = addIncidentDto.Reason,
                AdditionalComment = addIncidentDto.Comment,
            };
        }
    }
}
