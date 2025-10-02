using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
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

    public IActionResult GetTrains()
    {
        ViewBag.Title = "Trains";

        // Read JSON file
        string path = Path.Combine(_env.ContentRootPath, "Database", "Seed", "trains.json");
        string json = System.IO.File.ReadAllText(path);

        var root = JsonSerializer.Deserialize<Root>(json);

        var trains = root.Data
            .Where(t => t.ReturnValue != null)
            .Select(t => new TrainViewModel
            {
                TrainName = t.TrainName,
                TrainNumber = t.ReturnValue.TrainNumber,
                DelayTime = t.ReturnValue.DelayTime,
                NextStation = t.ReturnValue.NextStop?.Title ?? String.Empty,
                HasIncidentHistory = false
            }).ToList();

        return View(trains);
    }
}
