using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainMonitor.DataBase;
using TrainMonitor.Exceptions;
using TrainMonitor.Helpers;
using TrainMonitor.Models;
using TrainMonitor.ViewModels;

namespace TrainMonitor.Controllers;

[Route("trains")]
public class TrainController : Controller
{
    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _context;

    public TrainController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _env = env;
        _context = context;
    }

    /* GET /trains
     * Returns a list of trains with their details.
     */
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
                LastUpdatedTime = TrainUtils.LastUpdatedTimeConverstion(t),
                NextStation = t.ReturnValue.NextStop?.Title ?? String.Empty,
                HasDelay = t.ReturnValue.DelayTime > 10,
                HasIncident = false,
            })
            //.Reverse()
            .ToList() ?? [];

        return View("Trains", trains);
    }

    /* GET /trains/{trainID}/incidents
     * Returns incidents for a specific train.
     */
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

    /* POST /trains/addIncident
     * Check form data for TrainId and see if it exists already in DB
     * If it does not exist, verify if TrainId exists in the JSON file
     * If the TrainId exists in the JSON file, save the train data to DB
     * and then add the train incident
     */
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
        var train = await TrainUtils.GetOrCreateTrainAsync(model.TrainId, _context, _env);

        if (train == null)
            return Json(new { success = false, message = $"Invalid Train ID: {model.TrainId}" });


        // Add train incident
        var incident = new Incident
        {
            Username = model.Username,
            Reason = model.Reason,
            AdditionalComment = model.Comment,
            Train = train
        };

        _context.Incidents.Add(incident);

        await _context.SaveChangesAsync();


        return Ok(new { success = true, message = "Incident saved successfully!" });
    }

}
