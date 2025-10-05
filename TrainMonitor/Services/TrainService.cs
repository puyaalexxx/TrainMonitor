// Ignore Spelling: env dto

using TrainMonitor.Data.Repositories;
using TrainMonitor.DTOs;
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

    /// <summary>
    /// Checks if a train with the specified <paramref name="trainId"/> exists in the database.
    /// If it does not exist, verifies if the trainId exists in the JSON file.
    /// If found in the JSON file, saves the train data to the database.
    /// </summary>
    /// <param name="trainId">The ID of the train to retrieve or create.</param>
    /// <returns>
    /// The existing or newly created <see cref="Train"/> object, or <c>null</c> if the trainId does not exist in the JSON file.
    /// </returns>
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

    /// <summary>
    /// Adds an incident to a specific train.
    /// </summary>
    /// <param name="dto">The incident details to be added.</param>
    public async Task AddIncidentAsync(AddIncidentDto dto)
    {
        var incident = new Incident
        {
            TrainId = dto.TrainId,
            Username = dto.Username,
            Reason = dto.Reason,
            AdditionalComment = dto.Comment
        };

        await _trainRepository.AddIncidentAsync(incident);
    }

    public Task SaveChangesAsync() => _trainRepository.SaveChangesAsync();

}
