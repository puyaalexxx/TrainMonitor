using TrainMonitor.Models;

namespace TrainMonitor.Services;

public interface ITrainService
{
    Task<Train?> GetOrCreateTrainAsync(string trainId);
}
