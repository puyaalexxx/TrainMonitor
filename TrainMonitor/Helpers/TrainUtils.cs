using Microsoft.AspNetCore.Mvc.ModelBinding;
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

    public static string CollectFormErrors(ModelStateDictionary modelState)
    {
        // Collect all errors into a single list
        var allErrors = modelState.Values
            .Where(v => v?.Errors.Any() == true)
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        var errorMessage = allErrors.Any()
            ? string.Join("<br/>", allErrors)
            : "Unexpected form error. Please try again.";

        return errorMessage;
    }
}
