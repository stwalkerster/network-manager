namespace DnsWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.Dns;
    using DnsWebApp.Models.ViewModels;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ZoneController : Controller
    {
        private readonly WhoisService whoisService;
        private readonly DataContext db;
        private readonly UserManager<IdentityUser> userManager;

        public ZoneController(WhoisService whoisService, DataContext db, UserManager<IdentityUser> userManager)
        {
            this.whoisService = whoisService;
            this.db = db;
            this.userManager = userManager;
        }

        [Route("/zones")]
        public IActionResult Index()
        {
            var z = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Registrar)
                .Include(x => x.Owner)
                .Include(x => x.Records)
                .Include(x => x.FavouriteDomains)
                .ThenInclude(x => x.User)
                .ToList();

            this.whoisService.UpdateExpiryAttributes(z);
            
            return this.View(z);
        }

        [Route("/zone/{zone:int}")]
        public IActionResult ShowZone(int zone)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.Records)
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.ZoneGroup)
                .ThenInclude(x => x.Records)
                .FirstOrDefault(x => x.Id == zone);

            if (zoneObject == null)
            {
                return this.RedirectToAction("Index");
            }

            var display = new ZoneDisplay
            {
                Zone = zoneObject,
                Fqdn = zoneObject.Name + "." + zoneObject.TopLevelDomain.Domain
            };
            
            foreach (var zoneRecord in zoneObject.Records)
            {
                switch (zoneRecord.Type)
                {
                    case RecordType.A:
                    case RecordType.AAAA:
                    case RecordType.CNAME:
                        display.HostRecords.FromZone.Add(zoneRecord);
                        break;
                    case RecordType.CAA:
                        display.CaaRecords.FromZone.Add(zoneRecord);
                        break;
                    case RecordType.MX:
                        display.MxRecords.FromZone.Add(zoneRecord);
                        break;
                    case RecordType.NS:
                        display.NsRecords.FromZone.Add(zoneRecord);
                        break;
                    case RecordType.SRV:
                        display.SrvRecords.FromZone.Add(zoneRecord);
                        break;
                    case RecordType.SSHFP:
                        display.SshfpRecords.FromZone.Add(zoneRecord);
                        break;
                    case RecordType.TXT:
                        display.TxtRecords.FromZone.Add(zoneRecord);
                        break;
                }
            }

            foreach (var zoneGroup in zoneObject.ZoneGroupMembers.Select(x => x.ZoneGroup))
            {
                foreach (var zoneRecord in zoneGroup.Records)
                {
                    switch (zoneRecord.Type)
                    {
                        case RecordType.A:
                        case RecordType.AAAA:
                        case RecordType.CNAME:
                            display.HostRecords.FromZoneGroup.Add(zoneRecord);
                            break;
                        case RecordType.CAA:
                            display.CaaRecords.FromZoneGroup.Add(zoneRecord);
                            break;
                        case RecordType.MX:
                            display.MxRecords.FromZoneGroup.Add(zoneRecord);
                            break;
                        case RecordType.NS:
                            display.NsRecords.FromZoneGroup.Add(zoneRecord);
                            break;
                        case RecordType.SRV:
                            display.SrvRecords.FromZoneGroup.Add(zoneRecord);
                            break;
                        case RecordType.SSHFP:
                            display.SshfpRecords.FromZoneGroup.Add(zoneRecord);
                            break;
                        case RecordType.TXT:
                            display.TxtRecords.FromZoneGroup.Add(zoneRecord);
                            break;
                    }
                }
            }
            
            
            return this.View(display);
        }

        [Route("/zone/{zone:int}/zonefile")]
        public IActionResult ShowZoneFile(int zone)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.Records)
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.ZoneGroup)
                .ThenInclude(x => x.Records)
                .FirstOrDefault(x => x.Id == zone);

            if (zoneObject == null)
            {
                return this.RedirectToAction("Index");
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
            
            return this.View(display);
        }
        
        public IActionResult EditRecord(int record)
        {
            return Content("no.");
        }
        
        #region NewZone
        [HttpGet]
        [Route("/zone/new")]
        public IActionResult NewZone()
        {
            var zoneCommand = new ZoneCommand();
            zoneCommand.Registrars = this.db.Registrar.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            zoneCommand.TopLevelDomains = this.db.TopLevelDomains.Select(x => new SelectListItem("." + x.Domain, x.Id.ToString())).ToList();
            zoneCommand.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();

            zoneCommand.Refresh = zoneCommand.Retry = zoneCommand.TimeToLive = zoneCommand.DefaultTimeToLive = 300;
            zoneCommand.Expire = 7*86400;
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

            var zoneObject = new Zone();
            zoneObject.Name = editedZone.BaseName;
            zoneObject.TopLevelDomainId = editedZone.TopLevelDomain;
            
            zoneObject.PrimaryNameServer = editedZone.PrimaryNameServer;
            zoneObject.Administrator = editedZone.Administrator;
            zoneObject.Refresh = editedZone.Refresh;
            zoneObject.Retry = editedZone.Retry;
            zoneObject.Expire = editedZone.Expire;
            zoneObject.TimeToLive = editedZone.TimeToLive;
            zoneObject.DefaultTimeToLive = editedZone.DefaultTimeToLive;
            
            zoneObject.Enabled = editedZone.Enabled;
            zoneObject.RegistrarId = editedZone.Registrar;
            zoneObject.OwnerId = editedZone.Owner;
            zoneObject.RegistrationExpiry = editedZone.RegistrationExpiry;

            zoneObject.TouchSerialNumber();
            
            this.db.Zones.Add(zoneObject);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        #endregion
        #region EditZone
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
            zoneObject.DefaultTimeToLive = editedZone.DefaultTimeToLive;
            
            zoneObject.Enabled = editedZone.Enabled;
            zoneObject.RegistrarId = editedZone.Registrar;
            zoneObject.OwnerId = editedZone.Owner;
            zoneObject.RegistrationExpiry = editedZone.RegistrationExpiry;
            
            zoneObject.TouchSerialNumber();
            
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
                DefaultTimeToLive = zoneObject.DefaultTimeToLive,
                TopLevelDomain = zoneObject.TopLevelDomainId,

                Id = zoneObject.Id,
                Name = zoneObject.Name + "." + zoneObject.TopLevelDomain.Domain,
                Registrars = this.db.Registrar.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList(),
                TopLevelDomains = this.db.TopLevelDomains.Select(x => new SelectListItem(x.Domain, x.Id.ToString())).ToList(),
                Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList()
            };
            
            return this.View(cmd);
        }
        #endregion
        
        [HttpPost]
        [Route("/zone/togglefave")]
        public IActionResult ToggleFave(int zone, string returnto)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.FavouriteDomains)
                .FirstOrDefault(x => x.Id == zone);

            if (zoneObject == null)
            {
                return this.Redirect(returnto);
            }

            var userId = this.userManager.GetUserId(this.HttpContext.User);
            
            if (zoneObject.FavouriteDomains != null && zoneObject.FavouriteDomains.Any(x => x.UserId == userId))
            {
                zoneObject.FavouriteDomains.RemoveAll(x => x.UserId == userId && x.ZoneId == zoneObject.Id);
            }
            else
            {
                if (zoneObject.FavouriteDomains == null)
                {
                    zoneObject.FavouriteDomains = new List<FavouriteDomains>();
                }

                zoneObject.FavouriteDomains.Add(new FavouriteDomains
                {
                    UserId = userId,
                    Zone = zoneObject
                });
            }

            this.db.SaveChanges();
            return this.Redirect(returnto);
        }
        
        #region Records
        
        private IActionResult AddRecord(int id, RecordViewModelBase recordViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(recordViewModel);
            }

            var zone = this.db.Zones.Include(x => x.Records).FirstOrDefault(x => x.Id == id);
            if (zone == null)
            {
                return this.RedirectToAction("Index");
            }

            zone.TouchSerialNumber();
            zone.Records.Add(recordViewModel.Record);

            this.db.SaveChanges();

            return this.RedirectToAction("ShowZone", new {zone = id});
        }
       

        [HttpGet]
        [Route("/zone/{id:int}/add/ns")]
        public IActionResult AddNsRecord(int id)
        {
            return this.View(
                new NsRecordViewModel(
                    new Record
                    {
                        ZoneId = id,
                        Type = RecordType.NS,
                        Class = RecordClass.IN
                    }));
        }
        
        [HttpPost]
        [Route("/zone/{id:int}/add/ns")]
        public IActionResult AddNsRecord(int id, NsRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }

        [HttpGet]
        [Route("/zone/{id:int}/add/caa")]
        public IActionResult AddCaaRecord(int id)
        {
            return this.View(
                new CaaRecordViewModel(
                    new Record
                    {
                        ZoneId = id,
                        Type = RecordType.CAA,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zone/{id:int}/add/caa")]
        public IActionResult AddCaaRecord(int id, CaaRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/mx")]
        public IActionResult AddMxRecord(int id)
        {
            return this.View(
                new MxRecordViewModel(
                    new Record
                    {
                        ZoneId = id,
                        Type = RecordType.MX,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zone/{id:int}/add/mx")]
        public IActionResult AddMxRecord(int id, MxRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/host")]
        public IActionResult AddHostRecord(int id)
        {
            return this.View(
                new HostRecordViewModel(
                    new Record
                    {
                        ZoneId = id,
                        Class = RecordClass.IN,
                        Type = RecordType.A
                    }));        
        }
        
        [HttpPost]
        [Route("/zone/{id:int}/add/host")]
        public IActionResult AddHostRecord(int id, HostRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/txt")]
        public IActionResult AddTxtRecord(int id)
        {
            return this.View(
                new TxtRecordViewModel(
                    new Record
                    {
                        ZoneId = id,
                        Type = RecordType.TXT,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zone/{id:int}/add/txt")]
        public IActionResult AddTxtRecord(int id, TxtRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        
        [HttpGet]
        [Route("/zone/{id:int}/add/srv")]
        public IActionResult AddSrvRecord(int id)
        {
            return this.View(
                new SrvRecordViewModel(
                    new Record
                    {
                        ZoneId = id,
                        Type = RecordType.SRV,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zone/{id:int}/add/srv")]
        public IActionResult AddSrvRecord(int id, SrvRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/sshfp")]
        public IActionResult AddSshfpRecord(int id)
        {
            return this.View(
                new SshfpRecordViewModel(
                    new Record
                    {
                        ZoneId = id,
                        Type = RecordType.SSHFP,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zone/{id:int}/add/sshfp")]
        public IActionResult AddSshfpRecord(int id, SshfpRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        #endregion
    }
}