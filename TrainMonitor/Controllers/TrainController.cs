using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TrainMonitor.Exceptions;
using TrainMonitor.Helpers;
using TrainMonitor.Helpers.Json;
using TrainMonitor.ViewModels;

namespace TrainMonitor.Controllers;

[Route("trains")]
public class TrainController : Controller
{
    private readonly IWebHostEnvironment _env;

    public TrainController(IWebHostEnvironment env)
    {
        _env = env;
    }

    /* GET /trains
     * Returns a list of trains with their details.
     */
    public IActionResult GetTrains()
    {
        ViewBag.Title = "Trains";

        // Read JSON file
        string path = Path.Combine(_env.ContentRootPath, "Database", "Seed", "trains.json");
        string json = System.IO.File.ReadAllText(path);

        var root = JsonSerializer.Deserialize<Root>(json);

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
                HasDelay = t.ReturnValue.DelayTime > 10
            })
            //.Reverse()
            .ToList() ?? new List<TrainViewModel>();

        return View("Trains", trains);
    }

    /* GET /trains/{trainID}/incidents
     * Returns incidents for a specific train.
     */
    [Route("{trainID}/incidents")]
    public IActionResult GetTrainIncidents(int trainID)
    {
        if (!ModelState.IsValid)
        {
            throw new InvalidIdException($"Invalid Train ID: {trainID}");
        }

        ViewBag.Title = "Train Incidents";

        return View("Incidents");
    }

}
