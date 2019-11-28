using Microsoft.AspNetCore.Mvc;

namespace LdapDnsWebApp.Controllers
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