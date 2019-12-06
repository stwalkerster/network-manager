using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.ViewModels;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Mvc.Rendering;
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

            this.whoisService.UpdateExpiryAttributes(z);
            
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

        [HttpGet]
        [Route("/zone/new")]
        public IActionResult NewZone()
        {
            var zoneCommand = new ZoneCommand();
            zoneCommand.Registrars = this.db.Registrar.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            zoneCommand.TopLevelDomains = this.db.TopLevelDomains.Select(x => new SelectListItem("." + x.Domain, x.Id.ToString())).ToList();
            zoneCommand.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();

            zoneCommand.Refresh = zoneCommand.Retry = zoneCommand.TimeToLive = 300;
            zoneCommand.Expire = 86400;
            zoneCommand.PrimaryNameServer = "ns1.stwalkerster.net";
            zoneCommand.TopLevelDomain = this.db.TopLevelDomains.First(x => x.Domain == "com").Id;
            
            return this.View(zoneCommand);
        }
        
        [HttpPost]
        [Route("/zone/new")]
        public IActionResult NewZone(ZoneCommand editedZone)
        {
            if (!this.ModelState.IsValid)
            {
                editedZone.Registrars = this.db.Registrar.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
                editedZone.TopLevelDomains = this.db.TopLevelDomains.Select(x => new SelectListItem(x.Domain, x.Id.ToString())).ToList();
                editedZone.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();

                
                return this.View(editedZone);
            }

            throw new NotImplementedException();
            
            return this.RedirectToAction("Index");
        }

        
        [HttpPost]
        [Route("/zone/edit/{zone:int}")]
        public IActionResult EditZone(int zone, ZoneCommand editedZone)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .FirstOrDefault(x => x.Id == zone);

            if (zoneObject == null)
            {
                return this.RedirectToAction("Index");
            }
            
            if (!this.ModelState.IsValid)
            {
                editedZone.Id = zoneObject.Id;
                editedZone.Name = zoneObject.Name + "." + zoneObject.TopLevelDomain.Domain;
                editedZone.Registrars =
                    this.db.Registrar.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
                editedZone.TopLevelDomains = this.db.TopLevelDomains.Select(x => new SelectListItem(x.Domain, x.Id.ToString())).ToList();
                editedZone.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();
                
                return this.View(editedZone);
            }
            
            zoneObject.PrimaryNameServer = editedZone.PrimaryNameServer;
            zoneObject.Administrator = editedZone.Administrator;
            zoneObject.Refresh = editedZone.Refresh;
            zoneObject.Retry = editedZone.Retry;
            zoneObject.Expire = editedZone.Expire;
            zoneObject.TimeToLive = editedZone.TimeToLive;
            
            zoneObject.Enabled = editedZone.Enabled;
            zoneObject.RegistrarId = editedZone.Registrar;
            zoneObject.OwnerId = editedZone.Owner;
            zoneObject.RegistrationExpiry = editedZone.RegistrationExpiry;
            
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/zone/edit/{zone:int}")]
        public IActionResult EditZone(int zone)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .FirstOrDefault(x => x.Id == zone);

            if (zoneObject == null)
            {
                return this.RedirectToAction("Index");
            }
            
            var cmd = new ZoneCommand
            {
                Administrator = zoneObject.Administrator,
                BaseName = zoneObject.Name,
                Enabled = zoneObject.Enabled,
                Expire = zoneObject.Expire,
                Owner = zoneObject.OwnerId,
                PrimaryNameServer = zoneObject.PrimaryNameServer,
                Refresh = zoneObject.Refresh,
                Registrar = zoneObject.RegistrarId,
                RegistrationExpiry = zoneObject.RegistrationExpiry,
                Retry = zoneObject.Retry,
                TimeToLive = zoneObject.TimeToLive,
                TopLevelDomain = zoneObject.TopLevelDomainId,

                Id = zoneObject.Id,
                Name = zoneObject.Name + "." + zoneObject.TopLevelDomain.Domain,
                Registrars = this.db.Registrar.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList(),
                TopLevelDomains = this.db.TopLevelDomains.Select(x => new SelectListItem(x.Domain, x.Id.ToString())).ToList(),
                Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList()
            };
            
            return this.View(cmd);
        }

        [HttpPost]
        [Route("/zone/togglefave")]
        public IActionResult ToggleFave(int zone, string returnto)
        {
            return this.Redirect(returnto);
        }
    }
}