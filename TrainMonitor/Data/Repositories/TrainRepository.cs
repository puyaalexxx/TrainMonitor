using Microsoft.EntityFrameworkCore;
using TrainMonitor.DataBase;
using TrainMonitor.Models;

namespace TrainMonitor.Data.Repositories;

public class TrainRepository : ITrainRepository
{
    private readonly ApplicationDbContext _context;
    public TrainRepository(ApplicationDbContext context) => _context = context;

    public Task<Train?> GetByIdAsync(string trainId, CancellationToken cancellationToken = default) =>
        _context.Trains.FirstOrDefaultAsync(t => t.Id == trainId, cancellationToken);

    public Task AddAsync(Train train, CancellationToken cancellationToken = default) =>
        _context.Trains.AddAsync(train, cancellationToken).AsTask();

    public Task AddIncidentAsync(Incident incident, CancellationToken cancellationToken = default) =>
        _context.Incidents.AddAsync(incident, cancellationToken).AsTask();


    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}
