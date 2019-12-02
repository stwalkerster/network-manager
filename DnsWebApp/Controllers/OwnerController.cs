namespace DnsWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class OwnerController : Controller
    {
        private readonly DataContext db;
        private const string NoneSpecifier = "(none)";

        public OwnerController(DataContext db)
        {
            this.db = db;
        }
        
        [Route("/owner/{item}")]
        public IActionResult Item(string item)
        {
            Expression<Func<Zone,bool>> whereClause = x => x.Owner.UserName == item;
            if (item == NoneSpecifier)
            {
                whereClause = x => x.Owner == null;
            }
            
            var zones = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Where(whereClause)
                .ToList();
    
            this.ViewData["Owner"] = item;
            return this.View(zones);
        }
        
        [Route("/owner")]
        public IActionResult Index()
        {
            var zones = this.db.Zones
                .Include(x => x.Owner)
                .Include(x => x.TopLevelDomain)
                .Include(x => x.ZoneRecords)
                .ToList();

            var groupedZones = zones.Aggregate(
                new Dictionary<string, GroupedZoneSummary>(),
                (cur, next) =>
                {
                    var ownerUserName = next.Owner?.UserName ?? NoneSpecifier;
                    if (!cur.ContainsKey(ownerUserName))
                    {
                        cur.Add(
                            ownerUserName,
                            new GroupedZoneSummary {GroupName = ownerUserName, GroupKey = ownerUserName});
                    }

                    cur[ownerUserName].DisabledZones += !next.Enabled ? 1 : 0;
                    cur[ownerUserName].EnabledZones += next.Enabled ? 1 : 0;
                    cur[ownerUserName].DisabledRecords += !next.Enabled ? next.ZoneRecords.Count : 0;
                    cur[ownerUserName].DisabledRecords += next.Enabled ? next.ZoneRecords.Count : 0;

                    return cur;
                });

            return this.View(groupedZones);
        }
    }
}