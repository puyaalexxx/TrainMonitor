using TrainMonitor.DTOs;
using TrainMonitor.Helpers.Json;
using TrainMonitor.Models;
using TrainMonitor.ViewModels;

namespace TrainMonitor.Services;

public interface ITrainService
{
    /// <summary>
    /// Builds a list of TrainViewModel trains from JSON data and incident IDs.
    /// </summary>
    /// <param name="trainData">A List of train data objects</param>
    /// <param name="incidentTrainIdsSet">A set of TrainIds that have incidents saved into DB.</param>
    /// <returns>List of TrainViewModel trains.</returns>
    List<TrainViewModel> GetTrains(List<TrainJson> trainData, HashSet<string> incidentTrainIdsSet);

    /// <summary>
    /// Retrieves the IDs of trains that have at least one saved incident from the provided list of train IDs.
    /// </summary>
    /// <param name="trainIds">A collection of train IDs to check for incidents. If null or empty, the method returns an empty collection.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>
    /// A collection of train IDs that have at least one incident saved in the database. 
    /// Returns an empty collection if no matching incidents are found or if <paramref name="trainIds"/> is empty.
    /// </returns>
    Task<IEnumerable<string>> GetTrainIdsWithIncidentsAsync(IEnumerable<string> trainIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a train with the specified <paramref name="trainId"/> exists in the database.
    /// If it does not exist, verifies if the trainID exists in the JSON file.
    /// If found in the JSON file, saves the train data to the database.
    /// </summary>
    /// <param name="trainID">The ID of the train to retrieve or create.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>
    /// The existing or newly created <see cref="Train"/> object, or <c>null</c> if the trainID does not exist in the JSON file.
    /// </returns>
    Task<Train?> GetOrCreateTrainAsync(string trainID, CancellationToken cancellationToken = default);


    /// <summary>
    /// Retrieves all incidents associated with the specified train ID.
    /// </summary>
    /// <param name="trainID">The unique identifier of the train for which incidents are requested.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A <see cref="List{Incident}"/> containing all incidents for the specified train. Returns an empty list if there are no incidents.</returns>
    /// <exception cref="InvalidIdException">Thrown when <paramref name="trainID"/> is null, empty, or whitespace.</exception>

    Task<List<Incident>> GetIncidentsByTrainIdAsync(string trainID, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an incident to a specific train.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <param name="dto">The incident details to be added.</param>
    Task AddIncidentAsync(AddIncidentDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits all pending changes made via the service to the database.
    /// This acts as a Unit of Work, ensuring multiple operations are saved in a single transaction.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns></returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
