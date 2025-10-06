using Microsoft.EntityFrameworkCore;
using TrainMonitor.DataBase;
using TrainMonitor.Models;

namespace TrainMonitor.Data.Repositories;

public class TrainRepository : ITrainRepository
{
    private readonly ApplicationDbContext _context;
    public TrainRepository(ApplicationDbContext context) => _context = context;

    public Task<Train?> GetByIdAsync(string trainId) => _context.Trains.FirstOrDefaultAsync(t => t.Id == trainId);

    public Task AddAsync(Train train) => _context.Trains.AddAsync(train).AsTask();

    public async Task AddIncidentAsync(Incident incident) => await _context.Incidents.AddAsync(incident);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
