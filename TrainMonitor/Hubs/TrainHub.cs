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

        var random = new Random();
        var remainingTrains = trainData.ToList();

        bool isFirstIteration = true;

        while (!Context.ConnectionAborted.IsCancellationRequested)
        {
            int delayMs = isFirstIteration ? 1000 : random.Next(3000, 5000);
            await Task.Delay(delayMs, Context.ConnectionAborted);

            // random number of trains to send (1–3)
            int trainsToSend = random.Next(1, Math.Min(4, remainingTrains.Count + 1));

            // choose random trains without repetition
            var batch = remainingTrains
                .OrderBy(_ => random.Next())
                .Take(trainsToSend)
                .ToList();

            foreach (var train in batch)
            {
                //how many seconds the train should be displayed
                var trainLifetime = random.Next(40000, 60000);

                var viewModel = train.ToViewModel(incidentTrainIdsSet);

                await Clients.Caller.SendAsync("ReceiveTrain", viewModel, trainLifetime, Context.ConnectionAborted);

                //remove train after being sent
                remainingTrains.Remove(train);

                // schedule train removal after a random lifetime
                _ = Task.Run(async () =>
                {
                    await Task.Delay(trainLifetime, Context.ConnectionAborted);

                    // send train remove notification if the connection is still active
                    if (!Context.ConnectionAborted.IsCancellationRequested)
                    {
                        await Clients.Caller.SendAsync("RemoveTrain", viewModel.TrainId, Context.ConnectionAborted);
                    }
                }, Context.ConnectionAborted);
            }

            isFirstIteration = false;
        }
    }
}