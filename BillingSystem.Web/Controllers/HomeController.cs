using Microsoft.AspNetCore.Mvc;

namespace BillingSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}