using Microsoft.AspNetCore.Mvc;

namespace TrainMonitor.Controllers;

[Route("trains")]
public class TrainController : Controller
{

    public IActionResult GetTrains()
    {
        ViewBag.Title = "Trains";

        // Read JSON file
        //string json = System.IO.File.ReadAllText("trains.json");

        // Deserialize only the relevant part
        //var root = JsonSerializer.Deserialize<Root>(json); // Root contains "data": List<TrainJson>

        /*var trains = _context.Trains
            .Select(t => new TrainViewModel
            {
                Id = t.Id,
                TrainName = t.TrainName,
                TrainNumber = t.TrainNumber,
                DelayTime = t.DelayTime,
                NextStation = t.DepartureTime.AddMinutes(t.DelayTime).ToString("HH:mm"),
                HasIncidentHistory = t.Incidents.Any()
            }).ToList();*/

        return View();
    }
}
