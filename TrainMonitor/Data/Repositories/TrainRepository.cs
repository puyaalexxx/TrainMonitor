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

    public Task<bool> TrainExistsAsync(string trainID, CancellationToken cancellationToken = default) =>
        _context.Trains.AnyAsync(t => t.Id == trainID, cancellationToken);

    public Task<List<string>> GetTrainIdsWithIncidentsAsync(IEnumerable<string> trainIds, CancellationToken cancellationToken = default) =>
        _context.Incidents
            .AsNoTracking()
            .Where(i => trainIds.Contains(i.TrainId))
            .Select(i => i.TrainId)
            .Distinct()
            .ToListAsync(cancellationToken);

    public Task<bool> HasIncidentAsync(string trainID, CancellationToken cancellationToken = default) =>
        _context.Incidents.AsNoTracking().AnyAsync(i => i.TrainId == trainID, cancellationToken);

    public Task AddAsync(Train train, CancellationToken cancellationToken = default) =>
        _context.Trains.AddAsync(train, cancellationToken).AsTask();

    public Task<List<Incident>> GetIncidentsByTrainIdAsync(string trainID, CancellationToken cancellationToken = default) =>
        _context.Incidents.AsNoTracking().Where(i => i.TrainId == trainID).ToListAsync(cancellationToken);

    public Task<List<Train>> GetTrainsWithIncidentsAsync(CancellationToken cancellationToken = default) =>
        _context.Trains.Include(t => t.Incidents).Where(t => t.Incidents.Any()).ToListAsync(cancellationToken);


    public Task AddIncidentAsync(Incident incident, CancellationToken cancellationToken = default) =>
        _context.Incidents.AddAsync(incident, cancellationToken).AsTask();


    public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
}
