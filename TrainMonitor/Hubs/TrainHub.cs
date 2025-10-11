
using Microsoft.AspNetCore.SignalR;
using TrainMonitor.Services;

namespace TrainMonitor.Hubs;

public class TrainHub : Hub
{
    private readonly ITrainStreamingService _trainStreamingService;

    public TrainHub(ITrainStreamingService trainUpdateService)
    {
        _trainStreamingService = trainUpdateService;
    }

    public async Task StartStreaming()
    {
        await _trainStreamingService.StreamTrainsAsync(Context.ConnectionAborted);
    }
}