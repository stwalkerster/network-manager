using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;

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
            var z = this.db.Zones.Select(
                    x => new ZoneSummary
                    {
                        ZoneName = x.Name + "." + x.TopLevelDomain.Domain, Enabled = x.Enabled,
                        Records = x.ZoneRecords.Count, Registrar = x.Registrar.Name, NameServer = x.PrimaryNameServer
                    })
                .ToList();

            return this.View(z);
        }

        [Route("/zone/{zone}")]
        public IActionResult ShowZone(string zone)
        {
            return Content("zonefile for " + zone);
        }
    }
}