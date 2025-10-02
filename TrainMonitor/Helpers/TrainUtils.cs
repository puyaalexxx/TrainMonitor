using TrainMonitor.Helpers.Json;

namespace TrainMonitor.Helpers;

public static class TrainUtils
{
    public static string LastUpdatedTimeConverstion(TrainJson train)
    {
        return train.ReturnValue.LastUpdatedTime.HasValue
                ? train.ReturnValue.LastUpdatedTime.Value
                    .ToUniversalTime()
                    .ToString("dd MMM yyyy, HH:mm:ss")
                : string.Empty;
    }
}
