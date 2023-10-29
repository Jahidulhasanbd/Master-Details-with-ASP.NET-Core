using Microsoft.AspNetCore.Mvc;

namespace prctc_masterDetails.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
