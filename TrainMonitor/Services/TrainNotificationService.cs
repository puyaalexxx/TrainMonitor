using Microsoft.AspNetCore.SignalR;
using TrainMonitor.Hubs;

namespace TrainMonitor.Services;

public class TrainNotificationService : ITrainNotificationService
{
    private readonly IHubContext<TrainHub> _hubContext;

    public TrainNotificationService(IHubContext<TrainHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task BroadcastIncidentAsync(string trainID) => _hubContext.Clients.All.SendAsync("IncidentAdded", trainID);
}
