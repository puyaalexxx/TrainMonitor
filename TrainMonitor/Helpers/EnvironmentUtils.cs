// Ignore Spelling: Utils

namespace TrainMonitor.Helpers;

public static class EnvironmentUtils
{
    /// <summary>
    /// Checks if the application is running inside a container by looking for the
    /// "DOTNET_RUNNING_IN_CONTAINER" environment variable.
    /// </summary>
    /// <returns>
    /// <c>true</c> if running in a container; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsRunningInContainer()
    {
        return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
    }
}
