using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using Microsoft.AspNetCore.Authorization;

    public class HomeController : Controller
    {
        // GET
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}