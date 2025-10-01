using Microsoft.AspNetCore.Mvc;

namespace TrainMonitor.Controllers;

public class HomeController : Controller
{

    /**
     * HomePage
     */
    public IActionResult Index()
    {
        return View();
    }


}
