using TrainMonitor.Data.Repositories;
using TrainMonitor.Helpers;
using TrainMonitor.Models;

namespace TrainMonitor.Services;

public sealed class TrainService : ITrainService
{
    private readonly ITrainRepository _trainRepository;
    private readonly IWebHostEnvironment _env;

    public TrainService(ITrainRepository trainRepository, IWebHostEnvironment env)
    {
        _trainRepository = trainRepository;
        _env = env;
    }

    /*
     * Check if trainId exists already in DB
     * If it does not exist, verify if trainId exists in the JSON file
     * If the trainId exists in the JSON file, save the train data to DB
     */
    public async Task<Train?> GetOrCreateTrainAsync(string trainId)
    {
        //check if train exists in DB
        var train = await _trainRepository.GetByIdAsync(trainId);

        if (train != null) return train;

        //if not, get train data from JSON and add to DB
        var trainData = await TrainUtils.GetTrainDataFromJsonAsync(trainId, _env);

        if (trainData == null) return null;

        await _trainRepository.AddAsync(trainData);

        return trainData;
    }
}
