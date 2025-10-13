using TrainMonitor.Models;

namespace TrainMonitor.Data.Repositories;

public interface ITrainRepository
{
    Task<Train?> GetByIdAsync(string trainId, CancellationToken cancellationToken = default);

    Task<bool> TrainExistsAsync(string trainID, CancellationToken cancellationToken = default);

    Task<List<string>> GetTrainIdsWithIncidentsAsync(IEnumerable<string> trainIds, CancellationToken cancellationToken = default);

    Task<bool> HasIncidentAsync(string trainID, CancellationToken cancellationToken = default);

    Task AddAsync(Train train, CancellationToken cancellationToken = default);

    Task<List<Incident>> GetIncidentsByTrainIdAsync(string trainID, CancellationToken cancellationToken = default);

    Task<List<Train>> GetTrainsWithIncidentsAsync(CancellationToken cancellationToken = default);

    Task AddIncidentAsync(Incident incident, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
