// Ignore Spelling: env Utils Json

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;
using TrainMonitor.Helpers.Json;
using TrainMonitor.Models;

namespace TrainMonitor.Helpers;

public static class TrainUtils
{
    /// <summary>
    /// Converts the <c>LastUpdatedTime</c> of a <see cref="TrainJson"/> object to a formatted string.
    /// If <c>LastUpdatedTime"</c> is <c>null</c>, returns an empty string.
    /// </summary>
    /// <param name="train">The <see cref="TrainJson"/> object containing the LastUpdatedTime.</param>
    /// <returns>Formatted LastUpdatedTime or an empty string.</returns>
    public static string LastUpdatedTimeConversion(TrainJson train)
    {
        return train.ReturnValue.LastUpdatedTime.HasValue
                ? train.ReturnValue.LastUpdatedTime.Value
                    .ToUniversalTime()
                    .ToString("dd MMM yyyy, HH:mm:ss")
                : string.Empty;
    }

    /// <summary>
    /// Collects all error messages from the <see cref="ModelStateDictionary"/> and concatenates them into a single string.
    /// If there are no errors, returns a default error message.
    /// </summary>
    /// <param name="modelState">The <see cref="ModelStateDictionary"/> containing validation errors.</param>
    /// <returns>Concatenated error messages or a default message.</returns>
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

    /// <summary>
    /// Asynchronously loads train data from a JSON file located at "Database/Seed/trains.json" path.
    /// Returns a <see cref="Root"/> object deserialized from the JSON content, or <c>null</c> if deserialization fails.
    /// </summary>
    /// <param name="env">The <see cref="IWebHostEnvironment"/> used to locate the content root path.</param>
    /// <returns>A <see cref="Task{Root}"/> representing the asynchronous operation, containing the deserialized <see cref="Root"/> object or <c>null</c>.</returns>
    public static async Task<Root?> LoadTrainsFromJsonFileAsync(IWebHostEnvironment env)
    {
        string path = Path.Combine(env.ContentRootPath, "Data", "Seed", "trains.json");
        string json = await System.IO.File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<Root>(json);
    }

    /// <summary>
    /// Asynchronously retrieves train data for a specific train ID from the JSON file.
    /// Returns a <see cref="Train"/> object if found, or <c>null</c> if the train ID does not exist in the JSON data.
    /// </summary>
    /// <param name="trainId">The ID of the train to retrieve.</param>
    /// <param name="env">The <see cref="IWebHostEnvironment"/> used to locate the content root path.</param>
    /// <returns>A <see cref="Task{Train}"/> <see cref="Train"/> object if found, or <c>null</c>.</returns>
    public static async Task<Train?> GetTrainDataFromJsonAsync(string trainId, IWebHostEnvironment env)
    {
        var root = await LoadTrainsFromJsonFileAsync(env);
        var trainData = root?.Data.FirstOrDefault(t => t.ReturnValue?.TrainId == trainId);

        if (trainData == null) return null;

        return new Train
        {
            Id = trainData.ReturnValue.TrainId,
            TrainName = trainData.TrainName,
            TrainNumber = trainData.ReturnValue.TrainNumber,
            DelayTime = trainData.ReturnValue.DelayTime
        };
    }
}
