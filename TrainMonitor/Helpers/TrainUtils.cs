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
    /// Asynchronously loads the train data from a JSON file located at "Data/Seed/trains.json".
    /// </summary>
    /// <param name="env">The <see cref="IWebHostEnvironment"/> used to locate the content root path.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>
    /// A List of <see cref="TrainJson"/> representing the asynchronous operation,
    /// containing the list of deserialized train data. If the file is empty, invalid, or deserialization fails, an empty list is returned.
    /// </returns>
    public static async Task<List<TrainJson>> LoadTrainsDataFromJsonFileAsync(IWebHostEnvironment env, CancellationToken cancellationToken = default)
    {
        string path = Path.Combine(env.ContentRootPath, "Data", "Seed", "trains.json");
        string json = await File.ReadAllTextAsync(path, cancellationToken);

        var root = JsonSerializer.Deserialize<Root>(json);

        return root?.Data ?? [];
    }

    /// <summary>
    /// Asynchronously retrieves train data for a specific train ID from the JSON file.
    /// Returns a <see cref="Train"/> object if found, or <c>null</c> if the train ID does not exist in the JSON data.
    /// </summary>
    /// <param name="trainId">The ID of the train to retrieve.</param>
    /// <param name="env">The <see cref="IWebHostEnvironment"/> used to locate the content root path.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A <see cref="Train"/> object if found, or <c>null</c> if the train ID does not exist in the JSON data.</returns>
    public static async Task<Train?> GetTrainDataFromJsonAsync(string trainId, IWebHostEnvironment env, CancellationToken cancellationToken = default)
    {
        var trains = await LoadTrainsDataFromJsonFileAsync(env, cancellationToken);

        //check if the trainId exists in the JSON data
        var trainData = trains.FirstOrDefault(t => t.ReturnValue?.TrainId == trainId);

        if (trainData == null) return null;

        return new Train
        {
            Id = trainData.ReturnValue.TrainId,
            TrainName = trainData.TrainName,
            TrainNumber = trainData.ReturnValue.TrainNumber,
            DelayTime = trainData.ReturnValue.DelayTime
        };
    }

    /// <summary>
    /// Extracts all TrainIds from the provided trainData List of objects.
    /// </summary>
    /// <param name="trainData">A List of train data objects</param>
    /// <returns>A list of TrainIds. Returns an empty list if the root or data is null.</returns>
    public static List<string> GetAllTrainIdsFromJson(List<TrainJson> trainData)
    {
        return trainData
            .Where(t => t.ReturnValue != null)
            .Select(t => t.ReturnValue.TrainId)
            .ToList();
    }
}
