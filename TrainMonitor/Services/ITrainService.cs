using TrainMonitor.DTOs;
using TrainMonitor.Models;

namespace TrainMonitor.Services;

public interface ITrainService
{
    Task<Train?> GetOrCreateTrainAsync(string trainId, CancellationToken cancellationToken = default);

    Task AddIncidentAsync(AddIncidentDto dto, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
