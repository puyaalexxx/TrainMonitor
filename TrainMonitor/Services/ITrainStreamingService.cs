namespace TrainMonitor.Services;

public interface ITrainStreamingService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task StreamTrainsAsync(CancellationToken cancellationToken = default);
}
