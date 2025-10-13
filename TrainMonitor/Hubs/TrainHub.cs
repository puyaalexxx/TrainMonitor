// Ignore Spelling: env

using Microsoft.AspNetCore.SignalR;
using TrainMonitor.Extensions.Mappings;
using TrainMonitor.Helpers;
using TrainMonitor.Helpers.Json;
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

        var random = new Random();

        // trains available to be sent to the client
        var availableTrains = trainData.ToList();
        // trains that are shown on the client
        var activeTrains = new List<TrainJson>();
        //first iteration should send trains right away
        bool isFirstIteration = true;

        while (!Context.ConnectionAborted.IsCancellationRequested)
        {
            await Task.Delay(GetNextDelay(isFirstIteration, random), Context.ConnectionAborted);

            // prevent the loop running continuously when no available trains.
            if (availableTrains.Count == 0)
            {
                await Task.Delay(2000, Context.ConnectionAborted);
                continue;
            }

            await SendRandomTrainsAsync(availableTrains, activeTrains, random, Clients.Caller, Context.ConnectionAborted);

            isFirstIteration = false;
        }
    }

    /// <summary>
    /// Selects a random subset of available trains and sends them to the client.
    /// Each train is assigned a random lifetime, after which it will be removed and eventually returned to the available list.
    /// </summary>
    /// <param name="availableTrains">The list of trains that are available to send.</param>
    /// <param name="activeTrains">The list of trains currently active (visible) for the client.</param>
    /// <param name="incidentTrainIdsSet">A set of train IDs that have incidents, used to populate the view model.</param>
    /// <param name="random">An instance of <see cref="Random"/> used to select trains and generate lifetimes.</param>
    /// <param name="client">The client proxy to send train data to.</param>
    /// <param name = "cancellationToken" > Token to cancel the operation if needed.</param>
    private async Task SendRandomTrainsAsync(List<TrainJson> availableTrains, List<TrainJson> activeTrains,
        Random random, IClientProxy client, CancellationToken cancellationToken)
    {
        // pick a random number of trains to send (1-3)
        int trainsToSend = random.Next(1, Math.Min(4, activeTrains.Count + 1));

        var batch = PickRandomTrains(availableTrains, trainsToSend, random);

        foreach (var train in batch)
        {
            // assign a random lifetime to each train
            int trainLifetime = random.Next(60000, 90000);

            bool hasIncident = await _trainService.HasIncidentAsync(train.ReturnValue.TrainId, Context.ConnectionAborted);
            var viewModel = train.ToViewModel(hasIncident);

            // send train and its lifetime(ms) to client
            await client.SendAsync("ReceiveTrain", viewModel, trainLifetime, cancellationToken);

            availableTrains.Remove(train);
            activeTrains.Add(train);

            // track when to remove the train
            _ = Task.Run(() =>
                RemoveTrainAfterLifetimeAsync(train, viewModel, availableTrains, activeTrains, trainLifetime, random),
                cancellationToken);
        }
    }

    /// <summary>
    /// Send the train that should be removed from the client.
    /// Waits for the specified <paramref name="trainLifetime"/> before notifying the client to remove the train,
    /// removes it from the active list, and then after a short random pause, returns it to the available list.
    /// </summary>
    /// <param name="train">The train being tracked.</param>
    /// <param name="viewModel">The view model of the train used for client notifications.</param>
    /// <param name="availableTrains">The list of trains that are available to send to the client.</param>
    /// <param name="activeTrains">The list of trains currently active (visible) for the client.</param>
    /// <param name="trainLifetime">The duration in milliseconds that the train should remain active before removal.</param>
    /// <param name="random">An instance of <see cref="Random"/> used to generate a random pause before returning the train to the available list.</param>
    private async Task RemoveTrainAfterLifetimeAsync(TrainJson train, TrainViewModel viewModel,
        List<TrainJson> availableTrains, List<TrainJson> activeTrains, int trainLifetime, Random random)
    {
        // wait for lifetime
        await Task.Delay(trainLifetime, Context.ConnectionAborted);

        if (!Context.ConnectionAborted.IsCancellationRequested)
        {
            // notify client to remove train
            await Clients.Caller.SendAsync("RemoveTrain", viewModel.TrainId, Context.ConnectionAborted);

            // remove from active
            activeTrains.Remove(train);

            // pause before returning to available
            int pause = random.Next(5000, 10000);
            await Task.Delay(pause, Context.ConnectionAborted);

            if (!Context.ConnectionAborted.IsCancellationRequested)
            {
                availableTrains.Add(train);
            }
        }
    }

    /// <summary>
    /// Determines the next delay in milliseconds before sending the next batch of trains.
    /// </summary>
    /// <param name="isFirstIteration">
    /// A flag indicating whether this is the first iteration. If true, returns a short initial delay.
    /// </param>
    /// <param name="random">An instance of <see cref="Random"/> used to generate a random delay.</param>
    /// <returns>
    /// The delay in milliseconds: 1000 ms for the first iteration, or a random value between 3000 and 5000 ms otherwise.
    /// </returns>
    private static int GetNextDelay(bool isFirstIteration, Random random) =>
        isFirstIteration ? 1000 : random.Next(3000, 5000);

    /// <summary>
    /// Selects a random subset of trains from the provided list.
    /// </summary>
    /// <param name="trains">The list of trains to pick from.</param>
    /// <param name="count">The number of trains to select.</param>
    /// <param name="random">An instance of <see cref="Random"/> used for shuffling.</param>
    /// <returns>
    /// Returns a new list containing a number of randomly chosen trains,
    /// no more than the number requested (count).
    /// </returns>
    private static List<TrainJson> PickRandomTrains(List<TrainJson> trains, int count, Random random) =>
        trains.OrderBy(_ => random.Next()).Take(count).ToList();

}