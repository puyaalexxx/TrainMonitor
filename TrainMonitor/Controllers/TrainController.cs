using Microsoft.AspNetCore.Mvc;

namespace TrainMonitor.Controllers;

[Route("trains")]
public class TrainController : Controller
{

    public IActionResult GetTrains()
    {
        return View();
    }
}
