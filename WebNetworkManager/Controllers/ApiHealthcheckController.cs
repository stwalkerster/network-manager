namespace DnsWebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    
    [ApiController]
    public class ApiHealthcheckController : Controller
    {
        [Route("/api/healthcheck")]
        public IActionResult Healthcheck()
        {
            this.Response.ContentType = "text/plain";
            return Content("OK");
        }
    }
}