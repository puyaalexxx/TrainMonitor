using Microsoft.EntityFrameworkCore;
using TrainMonitor.DataBase;
using TrainMonitor.Exceptions;
using TrainMonitor.Models;

namespace TrainMonitor.Data.Repositories;

public class TrainRepository : ITrainRepository
{
    private readonly ApplicationDbContext _context;
    public TrainRepository(ApplicationDbContext context) => _context = context;

    public Task<Train?> GetByIdAsync(string trainId) => _context.Trains.FirstOrDefaultAsync(t => t.Id == trainId);

    public Task AddAsync(Train train) => _context.Trains.AddAsync(train).AsTask();

    public async Task AddIncidentAsync(Incident incident)
    {
        var train = await GetByIdAsync(incident.TrainId);

        if (train == null)
            throw new InvalidIdException($"Train not found: {incident.TrainId}");

        train.Incidents.Add(incident);
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
