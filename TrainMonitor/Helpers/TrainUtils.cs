using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TrainMonitor.DataBase;
using TrainMonitor.Exceptions;
using TrainMonitor.Helpers.Json;
using TrainMonitor.Models;

namespace TrainMonitor.Helpers;

public static class TrainUtils
{
    /* 
     * Converts the LastUpdatedTime of a TrainJson object to a formatted string.
     * If LastUpdatedTime is null, returns an empty string.
     */
    public static string LastUpdatedTimeConverstion(TrainJson train)
    {
        return train.ReturnValue.LastUpdatedTime.HasValue
                ? train.ReturnValue.LastUpdatedTime.Value
                    .ToUniversalTime()
                    .ToString("dd MMM yyyy, HH:mm:ss")
                : string.Empty;
    }

    /* 
     * Collects all error messages from the ModelStateDictionary and concatenates them into a single string.
     * If there are no errors, returns a default error message.
     */
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

    /* 
     * Asynchronously loads train data from a JSON file located in the "Database/Seed/trains.json" path.
     * Returns a Root object deserialized from the JSON content, or null if deserialization fails.
     */
    public static async Task<Root?> LoadTrainsFromJsonFileAsync(IWebHostEnvironment env)
    {
        string path = Path.Combine(env.ContentRootPath, "Database", "Seed", "trains.json");
        string json = await System.IO.File.ReadAllTextAsync(path);

        return JsonSerializer.Deserialize<Root>(json);
    }

    /*
     * Asynchronously retrieves train data for a specific train ID from the JSON file.
     * Returns a Train object if found, or null if the train ID does not exist in the JSON data.
     */
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

    public static async Task<Train?> GetOrCreateTrainAsync(string trainId, ApplicationDbContext context, IWebHostEnvironment env)
    {
        //check if train exists in DB
        var train = await context.Trains.FirstOrDefaultAsync(t => t.Id == trainId);

        if (train != null) return train;

        //if not, get train data from JSON and add to DB
        var trainData = await TrainUtils.GetTrainDataFromJsonAsync(trainId, env);

        if (trainData == null) return null;

        await context.Trains.AddAsync(trainData);

        return trainData;
    }

}
