// Ignore Spelling: env dto

using TrainMonitor.Data.Repositories;
using TrainMonitor.DTOs;
using TrainMonitor.Exceptions;
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

    public async Task<IEnumerable<string>> GetTrainIdsWithIncidentsAsync(IEnumerable<string> trainIds, CancellationToken cancellationToken = default)
    {
        if (!trainIds.Any()) return [];

        var incidentTrainIds = await _trainRepository.GetTrainIdsWithIncidentsAsync(trainIds, cancellationToken);

        return incidentTrainIds;
    }

    public async Task<Train?> GetOrCreateTrainAsync(string trainID, CancellationToken cancellationToken = default)
    {
        //check if train exists in DB
        var train = await _trainRepository.GetByIdAsync(trainID, cancellationToken);

        if (train != null) return train;

        //if not, get train data from JSON and add to DB
        var trainData = await TrainUtils.GetTrainDataFromJsonAsync(trainID, _env, cancellationToken);

        if (trainData == null) return null;

        await _trainRepository.AddAsync(trainData, cancellationToken);

        return trainData;
    }

    public async Task<List<Incident>> GetIncidentsByTrainIdAsync(string trainID, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(trainID))
        {
            throw new InvalidIdException($"Invalid Train ID: {trainID}");
        }

        // check if train exists
        var trainExists = await _trainRepository.TrainExistsAsync(trainID, cancellationToken);
        if (!trainExists)
        {
            throw new TrainNotFoundException($"Train with ID {trainID} does not exist.");
        }

        return await _trainRepository.GetIncidentsByTrainIdAsync(trainID, cancellationToken);
    }

    public async Task AddIncidentAsync(AddIncidentDto dto, CancellationToken cancellationToken = default)
    {
        var incident = new Incident
        {
            TrainId = dto.TrainId,
            Username = dto.Username,
            Reason = dto.Reason,
            AdditionalComment = dto.Comment
        };

        await _trainRepository.AddIncidentAsync(incident, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _trainRepository.SaveChangesAsync(cancellationToken);

}
