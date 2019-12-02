using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.EntityFrameworkCore;

    public class ZoneController : Controller
    {
        private readonly WhoisService whoisService;
        private readonly DataContext db;

        public ZoneController(WhoisService whoisService, DataContext db)
        {
            this.whoisService = whoisService;
            this.db = db;
        }

        [Route("/zones")]
        public IActionResult Index()
        {
            var z = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Registrar)
                .Include(x => x.Owner)
                .Include(x => x.ZoneRecords)
                .ToList();

            return this.View(z);
        }

        [Route("/zone/{zone:int}")]
        public IActionResult ShowZone(int zone)
        {
            return this.View(this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .FirstOrDefault(x => x.Id == zone));
        }

        [Route("/zone/new")]
        public IActionResult NewZone()
        {
            return this.View(new Zone());
        }
        
        [Route("/zone/edit/{zone:int}")]
        public IActionResult EditZone(int zone)
        {
            return this.View(this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .FirstOrDefault(x => x.Id == zone));
        }

        [HttpPost]
        [Route("/zone/togglefave")]
        public IActionResult ToggleFave(int zone, string returnto)
        {
            return this.Redirect(returnto);
        }
    }
}