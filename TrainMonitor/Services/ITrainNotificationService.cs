namespace TrainMonitor.Services;

public interface ITrainNotificationService
{
    /// <summary>
    /// Broadcasts a train incident notification to all connected SignalR clients.
    /// Used to change the Incident History buttons for all clients not just the one that added the incident.
    /// </summary>
    /// <param name="trainID">The ID of the train for which the incident was added.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task BroadcastIncidentAsync(string trainID);
}