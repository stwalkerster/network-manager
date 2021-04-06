namespace DnsWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class OwnerController : Controller
    {
        private readonly DataContext db;
        private readonly WhoisService whoisService;
        private const string NoneSpecifier = "(none)";

        public OwnerController(DataContext db, WhoisService whoisService)
        {
            this.db = db;
            this.whoisService = whoisService;
        }
        
        [Route("/owner/zones/{item}")]
        public IActionResult Item(string item)
        {
            Expression<Func<Zone,bool>> whereClause = x => x.Owner.UserName == item;
            if (item == NoneSpecifier)
            {
                whereClause = x => x.Owner == null;
            }
            
            var zones = this.db.Zones
                .Include(x=>x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x=>x.Domain)
                .ThenInclude(x => x.Owner)
                .Include(x=>x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.FavouriteDomains)
                .ThenInclude(x => x.User)
                .Include(x => x.Records)
                .Include(x => x.HorizonView)
                .Where(whereClause)
                .ToList();
    
            this.ViewData["Owner"] = item;
            return this.View(zones);
        }
        
        [Route("/owner/domains/{item}")]
        public IActionResult Domains(string item)
        {
            Expression<Func<Domain,bool>> whereClause = x => x.Owner.UserName == item;
            if (item == NoneSpecifier)
            {
                whereClause = x => x.Owner == null;
            }
            
            var domains = this.db.Domains
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.Zones)
                .Include(x => x.TopLevelDomain)
                .Where(whereClause)
                .ToList();

            this.whoisService.UpdateExpiryAttributes(domains);
    
            this.ViewData["Owner"] = item;
            return this.View(domains);
        }
        
        [Route("/owner")]
        public IActionResult Index()
        {
            var zones = this.db.Zones
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Records)
                .ToList();

            var groupedZones = zones.Aggregate(
                new Dictionary<string, GroupedDomainZoneSummary>(),
                (cur, next) =>
                {
                    var ownerUserName = next.Owner?.UserName ?? NoneSpecifier;
                    if (!cur.ContainsKey(ownerUserName))
                    {
                        cur.Add(
                            ownerUserName,
                            new GroupedDomainZoneSummary {GroupName = ownerUserName, GroupKey = ownerUserName});
                    }

                    cur[ownerUserName].DisabledZones += !next.Enabled ? 1 : 0;
                    cur[ownerUserName].EnabledZones += next.Enabled ? 1 : 0;
                    cur[ownerUserName].DisabledRecords += !next.Enabled ? next.Records.Count : 0;
                    cur[ownerUserName].EnabledRecords += next.Enabled ? next.Records.Count : 0;

                    return cur;
                });

            foreach (var summary in groupedZones)
            {
                summary.Value.Domains =
                    this.db.Domains
                        .Include(x => x.Owner)
                        .Count(x => x.Owner.UserName == summary.Key);
            }

            return this.View(groupedZones);
        }
    }
}