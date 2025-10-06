using Microsoft.AspNetCore.Mvc;

namespace TrainMonitor.Controllers;

public class HomeController : Controller
{

    /// <summary>
    /// Homepage of the application.
    /// </summary>
    /// <returns>A view displaying the homepage content.</returns>
    public IActionResult Index()
    {
        return View();
    }


}
