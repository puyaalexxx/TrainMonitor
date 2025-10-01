namespace TrainMonitor.Helpers;

public static class EnvironmentUtils
{
    public static bool IsRunningInContainer()
    {
        return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    }
}
