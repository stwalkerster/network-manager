namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class RegistrarController : Controller
    {
        private readonly DataContext db;
        private readonly WhoisService whoisService;

        public RegistrarController(DataContext db, WhoisService whoisService)
        {
            this.db = db;
            this.whoisService = whoisService;
        }

        [Route("/registrar/{item}")]
        public IActionResult Item(long item)
        {
            var registrar = this.db.Registrar
                .Include(x => x.Zones)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Zones)
                .ThenInclude(x => x.Owner)
                .Include(x => x.Zones)
                .ThenInclude(x => x.FavouriteDomains)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == item);

            if (registrar == null)
            {
                return this.RedirectToAction("Index");
            }
            
            var zoneSummaries = registrar.Zones.ToList();

            this.whoisService.UpdateExpiryAttributes(zoneSummaries);
            
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