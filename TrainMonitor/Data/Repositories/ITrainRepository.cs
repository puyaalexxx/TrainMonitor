using TrainMonitor.Models;

namespace TrainMonitor.Data.Repositories;

public interface ITrainRepository
{
    Task<Train?> GetByIdAsync(string trainId, CancellationToken cancellationToken = default);
    Task AddAsync(Train train, CancellationToken cancellationToken = default);
    Task AddIncidentAsync(Incident incident, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
