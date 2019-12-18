using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Linq;
    using System.Text.Json;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.ViewModels;
    using Microsoft.EntityFrameworkCore;

    [Route("/api/zonefile/[action]")]
    [ApiController]
    public class ApiZonefileController : Controller
    {
        private readonly DataContext db;

        public ApiZonefileController(DataContext db)
        {
            this.db = db;
        }
        
        public IActionResult Zones()
        {
            var zones = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Where(x => x.Enabled)
                .Select(x => new {id = x.Id, name = x.Name + "." + x.TopLevelDomain.Domain})
                .ToList();
            
            return Content(JsonSerializer.Serialize(zones, new JsonSerializerOptions()));
        }
        
        public IActionResult ZoneFile(int id)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
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
                Fqdn = zoneObject.Name + "." + zoneObject.TopLevelDomain.Domain
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
    }
}