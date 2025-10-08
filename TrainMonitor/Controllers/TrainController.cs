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
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A view displaying the list of trains.</returns>
    public async Task<IActionResult> GetTrains(CancellationToken cancellationToken)
    {
        ViewBag.Title = "Trains";

        var trainData = await TrainUtils.LoadTrainsDataFromJsonFileAsync(_env, cancellationToken);

        //get train ids
        var trainIds = TrainUtils.GetAllTrainIdsFromJson(trainData);

        // get trains that have incidents
        var incidentTrainIdsSet = (await _trainService.GetTrainIdsWithIncidentsAsync(trainIds, cancellationToken)).ToHashSet();

        var trains = _trainService.GetTrains(trainData, incidentTrainIdsSet);

        return View("Trains", trains);
    }

    /// <summary>
    /// Handles GET requests to "/trains/{trainID}/incidents" and returns incidents for a specific train.
    /// </summary>
    /// <param name="trainID">The ID of the train to retrieve incidents for.</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A view displaying the incidents for the specified train.</returns>
    [HttpGet("{trainID}/incidents")]
    public IActionResult GetTrainIncidents(string trainID, CancellationToken cancellationToken)
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
    /// <param name="cancellationToken">Token to cancel the operation if needed.</param>
    /// <returns>A JSON response indicating success or failure, along with any error messages.</returns>
    [HttpPost("addIncident")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddIncident(AddIncidentViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            var errors = TrainUtils.CollectFormErrors(ModelState);

            return Json(new { success = false, errors });
        }

        // Check if train exists in DB, if not create it
        var train = await _trainService.GetOrCreateTrainAsync(model.TrainId, cancellationToken);

        if (train == null)
            return Json(new { success = false, errors = $"Invalid Train ID: {model.TrainId}" });

        //add incident to current train
        await _trainService.AddIncidentAsync(model.ToDto(), cancellationToken);

        // Commit all changes in one transaction
        await _trainService.SaveChangesAsync(cancellationToken);

        return Ok(new { success = true, message = "Incident saved successfully!" });
    }

}
