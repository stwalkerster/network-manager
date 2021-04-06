using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;

    public class RecordController : Controller
    {
        private readonly DataContext db;

        public RecordController(DataContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("/record/delete/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult DeleteRecord(int item)
        {
            var record = this.db.Record
                .Include(x => x.Zone)
                .ThenInclude(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.ZoneGroup)
                .FirstOrDefault(x => x.Id == item);
            
            return View(record);
        }
        
        [HttpPost]
        [Route("/record/delete/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult DeleteRecord(int item, Record r)
        {
            var record = this.db.Record
                .Include(x => x.Zone)
                .ThenInclude(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.ZoneGroup)
                .ThenInclude(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.Zone)
                .FirstOrDefault(x => x.Id == item);

            if (record == null)
            {
                return this.NotFound();
            }

            IActionResult action;
            
            if (record.Zone != null)
            {
                record.Zone.TouchSerialNumber();
                action = this.RedirectToAction("ShowZone", "Zone", new {zone = record.ZoneId});
            }
            else
            {
                record.ZoneGroup.TouchSerialNumber();
                action = this.RedirectToAction("ShowZoneGroup", "ZoneGroup", new {item = record.ZoneGroupId});
            }
            
            this.db.Record.Remove(record);
            this.db.SaveChanges();
            
            return action;
        }
    }
}