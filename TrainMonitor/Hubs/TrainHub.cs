// Ignore Spelling: env

using Microsoft.AspNetCore.SignalR;
using TrainMonitor.Helpers;
using TrainMonitor.Services;
using TrainMonitor.ViewModels;

namespace TrainMonitor.Hubs;

public class TrainHub : Hub
{
    private readonly ITrainService _trainService;
    private readonly IWebHostEnvironment _env;
    private static readonly HashSet<string> ConnectedClients = new();


    public TrainHub(ITrainService trainService, IWebHostEnvironment env)
    {
        _trainService = trainService;
        _env = env;
    }

    public override Task OnConnectedAsync()
    {
        ConnectedClients.Add(Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectedClients.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task StartStreaming()
    {
        // Only stream if the client is new
        if (!ConnectedClients.Contains(Context.ConnectionId)) return;

        var trainData = await TrainUtils.LoadTrainsDataFromJsonFileAsync(_env, Context.ConnectionAborted);

        var trainIds = TrainUtils.GetAllTrainIdsFromJson(trainData);

        // This service call fetches incident history from MariaDB
        var incidentTrainIdsSet = (await _trainService.GetTrainIdsWithIncidentsAsync(trainIds, Context.ConnectionAborted)).ToHashSet();

        foreach (var train in trainData)
        {
            var viewModel = train.ToViewModel(incidentTrainIdsSet);

            await Clients.Caller.SendAsync("ReceiveTrain", viewModel, DateTime.Now.ToString("HH:mm:ss"), Context.ConnectionAborted);

            await Task.Delay(2000, Context.ConnectionAborted); // simulate train updates over time
        }
    }
}