using TrainMonitor.Helpers;
using TrainMonitor.Helpers.Json;
using TrainMonitor.Models;

namespace TrainMonitor.ViewModels;

internal static class ViewModelMappings
{
    /// <summary>
    /// Converts a <see cref="TrainJson"/> object to a <see cref="TrainViewModel"/>,
    /// </summary>
    /// <param name="train">The <see cref="TrainJson"/> to convert.</param>
    /// <param name="incidentTrainIdsSet">A set of train IDs that have incidents.</param>
    /// <returns>The mapped <see cref="TrainViewModel"/>.</returns>
    public static TrainViewModel ToViewModel(this TrainJson train, HashSet<string> incidentTrainIdsSet)
    {
        return new TrainViewModel
        {
            TrainId = train.ReturnValue.TrainId,
            TrainName = train.TrainName,
            TrainNumber = train.ReturnValue.TrainNumber,
            DelayTime = train.ReturnValue.DelayTime,
            LastUpdatedTime = TrainUtils.LastUpdatedTimeConversion(train),
            NextStation = train.ReturnValue.NextStop?.Title ?? string.Empty,
            // check delay time to be bigger than 10 minutes
            HasDelay = train.ReturnValue.DelayTime > 10,
            // check if the train has incident saved in the database
            HasIncident = incidentTrainIdsSet.Contains(train.ReturnValue.TrainId)
        };
    }

    /// <summary>
    /// Converts an <see cref="Incident"/> to an <see cref="IncidentViewModel"/>.
    /// </summary>
    /// <param name="incident">The incident object to convert.</param>
    /// <returns>The mapped <see cref="IncidentViewModel"/>.</returns>
    public static IncidentViewModel ToViewModel(this Incident incident)
    {
        return new IncidentViewModel
        {
            Username = incident.Username,
            Reason = incident.Reason,
            Comment = incident.AdditionalComment
        };
    }
}
