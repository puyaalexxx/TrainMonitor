using Microsoft.AspNetCore.SignalR;
using TrainMonitor.Helpers;
using TrainMonitor.Hubs;
using TrainMonitor.ViewModels;

namespace TrainMonitor.Services;

public class TrainStreamingService : ITrainStreamingService
{
    private readonly IHubContext<TrainHub> _hubContext;
    private readonly ITrainService _trainService;
    private readonly IWebHostEnvironment _env;

    public TrainStreamingService(IHubContext<TrainHub> hubContext, ITrainService trainService, IWebHostEnvironment env)
    {
        _hubContext = hubContext;
        _trainService = trainService;
        _env = env;
    }

    public async Task StreamTrainsAsync(CancellationToken cancellationToken = default)
    {
        var trainData = await TrainUtils.LoadTrainsDataFromJsonFileAsync(_env, cancellationToken);

        var trainIds = TrainUtils.GetAllTrainIdsFromJson(trainData);

        // This service call fetches incident history from MariaDB
        var incidentTrainIdsSet = (await _trainService.GetTrainIdsWithIncidentsAsync(trainIds, cancellationToken)).ToHashSet();

        foreach (var train in trainData)
        {
            var viewModel = train.ToViewModel(incidentTrainIdsSet);

            await _hubContext.Clients.All.SendAsync("ReceiveTrain", viewModel, DateTime.Now.ToString("HH:mm:ss"), cancellationToken);

            await Task.Delay(2000, cancellationToken); // simulate train updates over time
        }
    }
}
