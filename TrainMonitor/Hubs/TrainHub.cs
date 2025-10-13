// Ignore Spelling: env

using Microsoft.AspNetCore.SignalR;
using TrainMonitor.Extensions.Mappings;
using TrainMonitor.Helpers;
using TrainMonitor.Helpers.Json;
using TrainMonitor.Services;

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

    /// <summary>
    /// Starts an infinite randomized train streaming process for the connected client.
    /// </summary>
    /// <remarks>
    /// This method continuously sends random batches of train data to the client at irregular intervals,
    /// simulating real-time train movements. Each train appears for a randomly assigned lifetime (in ms) before
    /// being removed and then re-added to the pool after a short pause period (in ms).  
    /// 
    /// <c>The system ensures:</c>
    /// <list type="bullet">
    ///     <item><description>No duplicate trains are visible simultaneously.</description></item>
    ///     <item><description>Each train receives a new randomized display duration (in ms) on every reappearance.</description></item>
    ///     <item><description>Each train is removed after a random duration (in ms) assigned to it.</description></item>
    /// </list>
    /// 
    /// This method runs indefinitely until the client disconnects or the connection is aborted.
    /// </remarks>
    /// <returns></returns>
    public async Task StartStreaming()
    {
        //only stream if the client is new
        if (!ConnectedClients.Contains(Context.ConnectionId)) return;

        var trainData = await TrainUtils.LoadTrainsDataFromJsonFileAsync(_env, Context.ConnectionAborted);

        var trainIds = TrainUtils.GetAllTrainIdsFromJson(trainData);

        // train ids that have incidents
        var incidentTrainIdsSet = (await _trainService.GetTrainIdsWithIncidentsAsync(trainIds, Context.ConnectionAborted)).ToHashSet();

        var random = new Random();

        // all trains start as available to send
        var availableTrains = trainData.ToList();
        // tracks trains currently visible
        var activeTrains = new List<TrainJson>();

        bool isFirstIteration = true;

        while (!Context.ConnectionAborted.IsCancellationRequested)
        {
            int delayMs = isFirstIteration ? 1000 : random.Next(3000, 5000);
            await Task.Delay(delayMs, Context.ConnectionAborted);


            // If all trains are currently active, wait for some to finish
            if (availableTrains.Count == 0)
            {
                await Task.Delay(2000, Context.ConnectionAborted);
                continue;
            }

            // random number of trains to send (1–3)
            int trainsToSend = random.Next(1, Math.Min(4, activeTrains.Count + 1));

            // choose random trains without repetition
            var batch = availableTrains
                .OrderBy(_ => random.Next())
                .Take(trainsToSend)
                .ToList();

            foreach (var train in batch)
            {
                //how many seconds the train should be displayed
                var trainLifetime = random.Next(60000, 90000);

                var viewModel = train.ToViewModel(incidentTrainIdsSet);

                await Clients.Caller.SendAsync("ReceiveTrain", viewModel, trainLifetime, Context.ConnectionAborted);

                // remove train from available trains
                availableTrains.Remove(train);
                // move train from available to active
                activeTrains.Add(train);

                // schedule train removal after a random lifetime
                _ = Task.Run(async () =>
                {
                    // Wait for lifetime, then remove from active
                    await Task.Delay(trainLifetime, Context.ConnectionAborted);

                    // send train remove notification if the connection is still active
                    if (!Context.ConnectionAborted.IsCancellationRequested)
                    {
                        await Clients.Caller.SendAsync("RemoveTrain", viewModel.TrainId, Context.ConnectionAborted);

                        // add a pause before returning to available
                        int pause = random.Next(5000, 10000);
                        await Task.Delay(pause, Context.ConnectionAborted);

                        if (!Context.ConnectionAborted.IsCancellationRequested)
                        {
                            availableTrains.Add(train);
                        }
                    }
                }, Context.ConnectionAborted);
            }

            isFirstIteration = false;
        }
    }
}