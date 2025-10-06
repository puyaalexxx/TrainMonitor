// Ignore Spelling: env

using Microsoft.AspNetCore.Mvc;
using TrainMonitor.DTOs;
using TrainMonitor.Exceptions;
using TrainMonitor.Helpers;
using TrainMonitor.Services;
using TrainMonitor.ViewModels;

namespace TrainMonitor.Controllers;

[Route("trains")]
public class TrainController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ITrainService _trainService;

    public TrainController(ITrainService trainService, IWebHostEnvironment env)
    {
        _env = env;
        _trainService = trainService;
    }

    /// <summary>
    /// Handles GET requests to "/trains" and returns a list of trains with their details.
    /// </summary>
    /// <returns>A view displaying the list of trains.</returns>
    public async Task<IActionResult> GetTrains()
    {
        ViewBag.Title = "Trains";

        var root = await TrainUtils.LoadTrainsFromJsonFileAsync(_env);

        var trains = root?.Data
            .Where(t => t.ReturnValue != null)
            .Select((t) => new TrainViewModel
            {
                TrainId = t.ReturnValue.TrainId,
                TrainName = t.TrainName,
                TrainNumber = t.ReturnValue.TrainNumber,
                DelayTime = t.ReturnValue.DelayTime,
                LastUpdatedTime = TrainUtils.LastUpdatedTimeConversion(t),
                NextStation = t.ReturnValue.NextStop?.Title ?? String.Empty,
                HasDelay = t.ReturnValue.DelayTime > 10,
                HasIncident = false,
            })
            //.Reverse()
            .ToList() ?? [];

        return View("Trains", trains);
    }

    /// <summary>
    /// Handles GET requests to "/trains/{trainID}/incidents" and returns incidents for a specific train.
    /// </summary>
    /// <param name="trainID">The ID of the train to retrieve incidents for.</param>
    /// <returns>A view displaying the incidents for the specified train.</returns>
    [HttpGet("{trainID}/incidents")]
    public IActionResult GetTrainIncidents(int trainID)
    {
        if (!ModelState.IsValid)
        {
            throw new InvalidIdException($"Invalid Train ID: {trainID}");
        }

        ViewBag.Title = "Train Incidents";

        return View("Incidents");
    }

    /// <summary>
    /// Handles POST requests to "/trains/addIncident".
    /// Creates the train if it does not exist and adds a new incident for the train.
    /// </summary>
    /// <param name="model">The <see cref="AddIncidentViewModel"/> containing the incident details submitted from the form.</param>
    /// <returns>A JSON response indicating success or failure, along with any error messages.</returns>
    [HttpPost("addIncident")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddIncident(AddIncidentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = TrainUtils.CollectFormErrors(ModelState);

            return Json(new { success = false, errors });
        }

        // Check if train exists in DB, if not create it
        var train = await _trainService.GetOrCreateTrainAsync(model.TrainId);

        if (train == null)
            return Json(new { success = false, message = $"Invalid Train ID: {model.TrainId}" });

        //add incident to current train
        await _trainService.AddIncidentAsync(model.ToDto());

        // Commit all changes in one transaction
        await _trainService.SaveChangesAsync();

        return Ok(new { success = true, message = "Incident saved successfully!" });
    }

}
