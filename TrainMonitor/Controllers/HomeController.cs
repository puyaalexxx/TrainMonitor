using Microsoft.AspNetCore.Mvc;

namespace TrainMonitor.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

        }

        /**
         * HomePage
         */
        public IActionResult Index()
        {
            return View();
        }


    }
}
