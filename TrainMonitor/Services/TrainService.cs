// Ignore Spelling: env dto

using TrainMonitor.Data.Repositories;
using TrainMonitor.DTOs;
using TrainMonitor.Exceptions;
using TrainMonitor.Helpers;
using TrainMonitor.Helpers.Json;
using TrainMonitor.Models;
using TrainMonitor.ViewModels;

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

    public List<TrainViewModel> GetTrains(List<TrainJson> trainData, HashSet<string> incidentTrainIdsSet)
    {
        return trainData
            .Where(t => t.ReturnValue != null)
            .Select(t => new TrainViewModel
            {
                TrainId = t.ReturnValue.TrainId,
                TrainName = t.TrainName,
                TrainNumber = t.ReturnValue.TrainNumber,
                DelayTime = t.ReturnValue.DelayTime,
                LastUpdatedTime = TrainUtils.LastUpdatedTimeConversion(t),
                NextStation = t.ReturnValue.NextStop?.Title ?? string.Empty,
                HasDelay = t.ReturnValue.DelayTime > 10, // check delay time to be bigger than 10 minutes
                HasIncident = incidentTrainIdsSet.Contains(t.ReturnValue.TrainId) // check if the train has incident saved in the database
            })
            .ToList();
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
