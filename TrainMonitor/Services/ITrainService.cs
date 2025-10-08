using TrainMonitor.DTOs;
using TrainMonitor.Models;

namespace TrainMonitor.Services;

public interface ITrainService
{
    /// <summary>
    /// Retrieves the IDs of trains that have at least one saved incident from the provided list of train IDs.
    /// </summary>
    /// <param name="trainIds">A collection of train IDs to check for incidents. If null or empty, the method returns an empty collection.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>
    /// A collection of train IDs that have at least one incident saved in the database. 
    /// Returns an empty collection if no matching incidents are found or if <paramref name="trainIds"/> is null or empty.
    /// </returns>
    Task<IEnumerable<string>> GetTrainIdsWithIncidentsAsync(IEnumerable<string> trainIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a train with the specified <paramref name="trainId"/> exists in the database.
    /// If it does not exist, verifies if the trainId exists in the JSON file.
    /// If found in the JSON file, saves the train data to the database.
    /// </summary>
    /// <param name="trainId">The ID of the train to retrieve or create.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>
    /// The existing or newly created <see cref="Train"/> object, or <c>null</c> if the trainId does not exist in the JSON file.
    /// </returns>
    Task<Train?> GetOrCreateTrainAsync(string trainId, CancellationToken cancellationToken = default);

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
