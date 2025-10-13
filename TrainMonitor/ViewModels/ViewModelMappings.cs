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
        var delayTime = train.ReturnValue.DelayTime;

        return new TrainViewModel
        {
            TrainId = train.ReturnValue.TrainId,
            TrainName = train.TrainName,
            TrainNumber = train.ReturnValue.TrainNumber,
            DelayTime = delayTime == 1 ? "1 min" : $"{delayTime} mins",
            LastUpdatedTime = DateTime.Now.ToString("HH:mm:ss"),
            NextStation = train.ReturnValue.NextStop?.Title ?? string.Empty,
            // check delay time to be bigger than 10 minutes
            HasDelay = train.ReturnValue.DelayTime > 10,
            // check if the train has incident saved in the database
            HasIncident = incidentTrainIdsSet.Contains(train.ReturnValue.TrainId),
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

    /// <summary>
    /// Converts a <see cref="Train"/> to a <see cref="TrainsWithIncidentsViewModel"/>.
    /// </summary>
    /// <param name="train">The train object to convert.</param>
    /// <returns>The mapped <see cref="TrainsWithIncidentsViewModel"/>.</returns>
    public static TrainsWithIncidentsViewModel ToViewModel(this Train train)
    {
        return new TrainsWithIncidentsViewModel
        {
            TrainId = train.Id,
            TrainNumber = train.TrainNumber,
            TrainName = train.TrainName,
            Incidents = train.Incidents.Select(i => i.ToViewModel()).ToList()
        };
    }
}