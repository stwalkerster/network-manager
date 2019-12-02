using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    public class RegistrarController : Controller
    {
        private readonly DataContext db;

        public RegistrarController(DataContext db)
        {
            this.db = db;
        }

        [Route("/registrar/{item}")]
        public IActionResult Item(long item)
        {
            var registrar = this.db.Registrar
                .Include(x => x.Zones)
                .ThenInclude(x => x.TopLevelDomain)
                .FirstOrDefault(x => x.Id == item);
            
            var zoneSummaries = registrar
                .Zones.Select(
                    x => new ZoneSummary
                    {
                        Enabled = x.Enabled, Expiry = x.RegistrationExpiry, NameServer = x.PrimaryNameServer,
                        Registrar = x.Registrar.Name, ZoneName = x.Name + "." + x.TopLevelDomain.Domain
                    })
                .ToList();

            this.ViewData["Registrar"] = registrar.Name;
            
            return this.View(zoneSummaries);
        }

        [Route("/registrar")]
        public IActionResult Index()
        {
            var includableQueryable = this.db.Registrar
                .Include(x => x.Zones)
                .ThenInclude(x => x.ZoneRecords)
                .ToList();

            var groupedZoneSummaries = includableQueryable.Select(
                x => new GroupedZoneSummary
                {
                    DisabledZones = x.Zones.Count(y => !y.Enabled),
                    EnabledZones = x.Zones.Count(y => y.Enabled),
                    EnabledRecords = x.Zones.Where(y => y.Enabled).Aggregate(0, (agg, cur) => agg + cur.ZoneRecords.Count),
                    DisabledRecords = x.Zones.Where(y => !y.Enabled).Aggregate(0, (agg, cur) => agg + cur.ZoneRecords.Count),
                    GroupKey = x.Id.ToString(),
                    GroupName = x.Name
                });

            return this.View(groupedZoneSummaries.ToDictionary(x => x.GroupKey));
        }
    }
}