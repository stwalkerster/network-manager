using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;

    public class ZoneGroupController : Controller
    {
        private DataContext db;
        private UserManager<IdentityUser> userManager;

        public ZoneGroupController(DataContext db, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [Route("/zones/group/{item:int}")]
        public IActionResult Item(int item)
        {
            return Content("");
        }
        
        [Route("/zones/group/{item:int}/zones")]
        public IActionResult Zones(int item)
        {
            var zones = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.FavouriteDomains).ThenInclude(x => x.User)
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.ZoneGroup)
                .Where(x => x.ZoneGroupMembers.Select(y=>y.ZoneGroupId).Contains(item))
                .ToList();

            this.ViewData["ZoneGroup"] = zones.FirstOrDefault()
                ?.ZoneGroupMembers.FirstOrDefault(x => x.ZoneGroupId == item)
                ?.ZoneGroup.Name;
            
            return this.View(zones);
        }
        
        [Route("/zones/groups")]
        public IActionResult Index()
        {
            var zoneGroups = this.db.ZoneGroups
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.Zone)
                .ThenInclude(x => x.ZoneRecords)
                .Include(x => x.ZoneGroupRecords)
                .ToList();


            var groupedZoneSummaries = zoneGroups.Select(
                x => new ZoneGroupSummary
                {
                    DisabledZones = x.ZoneGroupMembers.Count(y => !y.Zone.Enabled),
                    EnabledZones = x.ZoneGroupMembers.Count(y => y.Zone.Enabled),
                    EnabledRecords = x.ZoneGroupMembers.Where(y => y.Zone.Enabled).Aggregate(0, (agg, cur) => agg + cur.Zone.ZoneRecords.Count),
                    DisabledRecords = x.ZoneGroupMembers.Where(y => !y.Zone.Enabled).Aggregate(0, (agg, cur) => agg + cur.Zone.ZoneRecords.Count),
                    GroupRecords = x.ZoneGroupRecords.Count,
                    GroupKey = x.Id.ToString(),
                    GroupName = x.Name
                });

            return this.View(groupedZoneSummaries.ToDictionary(x => x.GroupKey));
        }

        [HttpGet]
        [Route("/zones/group/new")]
        public IActionResult NewZoneGroup()
        {
            return this.View(new ZoneGroupCommand
            {
                ZoneGroupMembers = new List<dynamic>(),
                AllZones = this.GetAllZones()
            });
        }

        [Route("/zones/group/new")]
        [HttpPost]
        public IActionResult NewZoneGroup(ZoneGroupCommand zoneGroupCommand)
        {
            if (!this.ModelState.IsValid)
            {
                zoneGroupCommand.AllZones = this.GetAllZones();
                return this.View(zoneGroupCommand);
            }

            var zoneGroup = new ZoneGroup
            {
                Name = zoneGroupCommand.Name,
                ZoneGroupMembers = new List<ZoneGroupMember>()
            };

            if (zoneGroupCommand.NewZoneGroupMembers != null)
            {
                var zoneMembers = zoneGroupCommand.NewZoneGroupMembers
                    .Split(",")
                    .Select(
                        x => new ZoneGroupMember
                        {
                            ZoneId = long.Parse(x),
                            ZoneGroup = zoneGroup
                        })
                    .ToList();

                
                zoneGroup.ZoneGroupMembers.AddRange(zoneMembers);
            }

            this.db.ZoneGroups.Add(zoneGroup);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [Route("/zones/group/edit/{item:int}")]
        [HttpGet]
        public IActionResult EditZoneGroup(int item)
        {
            var zoneGroupObject = this.db.ZoneGroups
                .Include(x => x.ZoneGroupMembers)
                .FirstOrDefault(x => x.Id == item);

            if (zoneGroupObject == null)
            {
                return this.RedirectToAction("Index");
            }

            var zoneGroupMembers = zoneGroupObject.ZoneGroupMembers
                .Select(x => x.ZoneId.ToString(CultureInfo.InvariantCulture))
                .ToList();
            var zoneGroupCommand = new ZoneGroupCommand
            {
                Name = zoneGroupObject.Name,
                ZoneGroupMembers = zoneGroupMembers,
                NewZoneGroupMembers = string.Join(",", zoneGroupMembers),
                AllZones = this.GetAllZones()
            };
            
            return this.View(zoneGroupCommand);
        }
        
        [Route("/zones/group/edit/{item:int}")]
        [HttpPost]
        public IActionResult EditZoneGroup(int item, ZoneGroupCommand command)
        {
            var zoneGroupObject = this.db.ZoneGroups
                .Include(x => x.ZoneGroupMembers)
                .FirstOrDefault(x => x.Id == item);

            if (zoneGroupObject == null)
            {
                return this.RedirectToAction("Index");
            }
            
            if (!this.ModelState.IsValid)
            {
                command.AllZones = this.GetAllZones();
                return this.View(command);
            }

            zoneGroupObject.Name = command.Name;

            if (command.NewZoneGroupMembers != null)
            {
                var zoneIds = command.NewZoneGroupMembers.Split(",").Select(x => int.Parse(x)).ToList();

                zoneGroupObject.ZoneGroupMembers.RemoveAll(x => !zoneIds.Contains((int) x.ZoneId));
                zoneIds.RemoveAll(x => zoneGroupObject.ZoneGroupMembers.Select(y => y.ZoneId).Contains(x));
                zoneGroupObject.ZoneGroupMembers.AddRange(
                    zoneIds.Select(
                        x => new ZoneGroupMember
                        {
                            ZoneId = x,
                            ZoneGroup = zoneGroupObject
                        }));
            }
            else
            {
                zoneGroupObject.ZoneGroupMembers.RemoveAll(x => true);
            }

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        private dynamic GetAllZones()
        {
            var allZones = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Select(x => new {label = x.Name + "." + x.TopLevelDomain.Domain, value = x.Id.ToString(CultureInfo.InvariantCulture)})
                .ToList();
            return allZones;
        }
    }
}