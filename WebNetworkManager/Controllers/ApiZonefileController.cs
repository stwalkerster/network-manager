using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.ViewModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services;

    [Route("/api/zonefile/[action]")]
    [ApiController]
    public class ApiZonefileController : Controller
    {
        private readonly DataContext db;

        public ApiZonefileController(DataContext db)
        {
            this.db = db;
        }
        
        public IActionResult Zones(long horizon)
        {
            var zones = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Where(x => x.Enabled && x.HorizonViewId.GetValueOrDefault() == horizon)
                .Select(x => x.Domain.Name + "." + x.Domain.TopLevelDomain.Domain + "|" +  x.Id)
                .ToList();

            this.Response.ContentType = "text/plain";
            return Content(string.Join('\n', zones));
        }
        
        public IActionResult ZoneFile(int id)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.Records)
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.ZoneGroup)
                .ThenInclude(x => x.Records)
                .FirstOrDefault(x => x.Id == id);

            if (zoneObject == null)
            {
                return this.NotFound();
            }

            var display = new ZoneFileDisplay
            {
                Zone = zoneObject,
                Fqdn = zoneObject.Domain.Name + "." + zoneObject.Domain.TopLevelDomain.Domain
            };
            
            foreach (var zoneRecord in zoneObject.Records)
            {
                display.Records.Add(zoneRecord);
            }

            foreach (var zoneGroup in zoneObject.ZoneGroupMembers.Select(x => x.ZoneGroup))
            {
                foreach (var zoneRecord in zoneGroup.Records)
                {
                    display.Records.Add(zoneRecord);
                }
            }

            this.HttpContext.Response.ContentType = "text/plain";
            return this.View(display);
        }

        [HttpPost]
        public IActionResult MergeZoneFile(int id, IFormFile existingZonefile)
        {
            if (existingZonefile == null)
            {
                return this.BadRequest();
            }
            
            var zoneObject = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.Records)
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.ZoneGroup)
                .ThenInclude(x => x.Records)
                .FirstOrDefault(x => x.Id == id);

            if (zoneObject == null)
            {
                return this.NotFound();
            }
            
            var display = new ZoneFileDisplay
            {
                Zone = zoneObject,
                Fqdn = zoneObject.Domain.Name + "." + zoneObject.Domain.TopLevelDomain.Domain
            };

            foreach (var zoneRecord in zoneObject.Records)
            {
                display.Records.Add(zoneRecord);
            }

            foreach (var zoneGroup in zoneObject.ZoneGroupMembers.Select(x => x.ZoneGroup))
            {
                foreach (var zoneRecord in zoneGroup.Records)
                {
                    display.Records.Add(zoneRecord);
                }
            }
            
            var parser = new ZoneFileParser(existingZonefile.OpenReadStream(), display.Fqdn + ".");
            var records = parser.Parse();

            var parsedSoa = records.First(x => x.Type == RecordType.SOA);
            records.Remove(parsedSoa);

            var parsedSoaSettings = parsedSoa.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            var maxSerialNumber = Math.Max(uint.Parse(parsedSoaSettings[2]), zoneObject.SerialNumber);
            zoneObject.SerialNumber = maxSerialNumber;
            zoneObject.TouchSerialNumber();
            this.db.SaveChanges();

            var dhcidNames = records.Where(x => x.Type == RecordType.DHCID)
                .Select(x => x.Name)
                .Where(x => !display.Zone.Records.Select(r => r.Name).Contains(x))
                .ToList();

            display.Records.AddRange(records.Where(x => dhcidNames.Contains(x.Name)));
            
            this.HttpContext.Response.ContentType = "text/plain";
            return this.View("Zonefile", display);

        }
    }
}