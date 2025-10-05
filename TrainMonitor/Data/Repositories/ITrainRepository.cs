using TrainMonitor.Models;

namespace TrainMonitor.Data.Repositories;

public interface ITrainRepository
{
    Task<Train?> GetByIdAsync(string trainId);
    Task AddAsync(Train train);
    Task SaveChangesAsync();
}
