namespace DnsWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.Dns;
    using DnsWebApp.Models.ViewModels;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ZoneController : Controller
    {
        private readonly DataContext db;
        private readonly UserManager<IdentityUser> userManager;

        public ZoneController(DataContext db, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [Route("/zones")]
        public IActionResult Index()
        {
            var z = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.Owner)
                .Include(x => x.Records)
                .Include(x => x.HorizonView)
                .Include(x => x.FavouriteDomains)
                .ThenInclude(x => x.User)
                .ToList();
            
            return this.View(z);
        }

        [Route("/zone/{zone:int}")]
        public IActionResult ShowZone(int zone)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.Records)
                .Include(x => x.HorizonView)
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
                Fqdn = zoneObject.Domain.Name + "." + zoneObject.Domain.TopLevelDomain.Domain
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
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.Records)
                .Include(x => x.HorizonView)
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
            
            return this.View(display);
        }
        
        #region NewZone
        [HttpGet]
        [Route("/zone/new")]
        public IActionResult NewZone()
        {
            var zoneCommand = new ZoneCommand();
            zoneCommand.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();
            zoneCommand.HorizonViews = this.db.HorizonViews.Select(x => new SelectListItem(x.ViewName, x.Id.ToString())).ToList();
            zoneCommand.Domains = this.db.Domains
                .Include(x => x.TopLevelDomain)
                .Select(x => new SelectListItem(x.Name + "." + x.TopLevelDomain.Domain, x.Id.ToString()))
                .ToList();

            zoneCommand.Refresh = zoneCommand.Retry = zoneCommand.TimeToLive = zoneCommand.DefaultTimeToLive = 300;
            zoneCommand.Expire = 7*86400;
            zoneCommand.PrimaryNameServer = "ns1.stwalkerster.net";
            
            return this.View(zoneCommand);
        }
        
        [HttpPost]
        [Route("/zone/new")]
        public IActionResult NewZone(ZoneCommand editedZone)
        {
            if (!this.ModelState.IsValid)
            {
                editedZone.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();
                editedZone.HorizonViews = this.db.HorizonViews.Select(x => new SelectListItem(x.ViewName, x.Id.ToString())).ToList();
                editedZone.Domains = this.db.Domains
                    .Include(x => x.TopLevelDomain)
                    .Select(x => new SelectListItem(x.Name + "." + x.TopLevelDomain.Domain, x.Id.ToString()))
                    .ToList();
                return this.View(editedZone);
            }

            var zoneObject = new Zone();
            zoneObject.DomainId = editedZone.Domain;
            
            zoneObject.PrimaryNameServer = editedZone.PrimaryNameServer;
            zoneObject.Administrator = editedZone.Administrator;
            zoneObject.Refresh = editedZone.Refresh;
            zoneObject.Retry = editedZone.Retry;
            zoneObject.Expire = editedZone.Expire;
            zoneObject.TimeToLive = editedZone.TimeToLive;
            zoneObject.DefaultTimeToLive = editedZone.DefaultTimeToLive;
            
            zoneObject.Enabled = editedZone.Enabled;
            
            zoneObject.OwnerId = editedZone.Owner;
            zoneObject.HorizonViewId = editedZone.HorizonView;

            zoneObject.TouchSerialNumber();
            
            this.db.Zones.Add(zoneObject);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        #endregion
        #region EditZone
        [HttpPost]
        [Route("/zone/edit/{zone:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditZone(int zone, ZoneCommand editedZone)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.HorizonView)
                .FirstOrDefault(x => x.Id == zone);

            if (zoneObject == null)
            {
                return this.RedirectToAction("Index");
            }
            
            if (!this.ModelState.IsValid)
            {
                editedZone.Id = zoneObject.Id;
                editedZone.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();
                editedZone.HorizonViews = this.db.HorizonViews.Select(x => new SelectListItem(x.ViewName, x.Id.ToString())).ToList();

                
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
            zoneObject.OwnerId = editedZone.Owner;
            zoneObject.HorizonViewId = editedZone.HorizonView;
            
            zoneObject.TouchSerialNumber();
            
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/zone/edit/{zone:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditZone(int zone)
        {
            var zoneObject = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .FirstOrDefault(x => x.Id == zone);

            if (zoneObject == null)
            {
                return this.RedirectToAction("Index");
            }
            
            var cmd = new ZoneCommand
            {
                Administrator = zoneObject.Administrator,
                Domain = zoneObject.DomainId,
                Enabled = zoneObject.Enabled,
                Expire = zoneObject.Expire,
                Owner = zoneObject.OwnerId,
                PrimaryNameServer = zoneObject.PrimaryNameServer,
                Refresh = zoneObject.Refresh,
                Retry = zoneObject.Retry,
                TimeToLive = zoneObject.TimeToLive,
                DefaultTimeToLive = zoneObject.DefaultTimeToLive,
                HorizonView = zoneObject.HorizonViewId,

                Id = zoneObject.Id,
                Name = zoneObject.Domain.Name + "." + zoneObject.Domain.TopLevelDomain.Domain,
                Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList(),
                HorizonViews = this.db.HorizonViews.Select(x => new SelectListItem(x.ViewName, x.Id.ToString())).ToList()

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

        private IActionResult EditRecord(int item, RecordViewModelBase recordViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(recordViewModel);
            }
            
            var record = this.db.Record
                .Include(x => x.Zone)
                .FirstOrDefault(x => x.Id == item);

            if (record == null)
            {
                return this.RedirectToAction("Index");
            }

            record.Name = recordViewModel.Record.Name;
            record.Value = recordViewModel.Record.Value;
            record.TimeToLive = recordViewModel.Record.TimeToLive;
            record.Type = recordViewModel.Type;
            
            record.Zone.TouchSerialNumber();

            this.db.SaveChanges();

            return this.RedirectToAction("ShowZone", new {zone = record.ZoneId});
        }

        private Record GetRecord(int item)
        {
            return this.db.Record
                .Include(x => x.Zone)
                .FirstOrDefault(x => x.Id == item);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/ns")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
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
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult AddNsRecord(int id, NsRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }

        [HttpGet]
        [Route("/zone/{id:int}/add/caa")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
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
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult AddCaaRecord(int id, CaaRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/mx")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
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
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult AddMxRecord(int id, MxRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/host")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
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
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult AddHostRecord(int id, HostRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/txt")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
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
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult AddTxtRecord(int id, TxtRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/srv")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
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
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult AddSrvRecord(int id, SrvRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/add/sshfp")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
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
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult AddSshfpRecord(int id, SshfpRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zone/{id:int}/edit/ns/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditNsRecord(int id, int item)
        {
            return this.View(new NsRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zone/{id:int}/edit/ns/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditNsRecord(int id, int item, NsRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zone/{id:int}/edit/caa/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditCaaRecord(int id, int item)
        {
            return this.View(new CaaRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zone/{id:int}/edit/caa/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditCaaRecord(int id, int item, CaaRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zone/{id:int}/edit/mx/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditMxRecord(int id, int item)
        {
            return this.View(new MxRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zone/{id:int}/edit/mx/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditMxRecord(int id, int item, MxRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zone/{id:int}/edit/host/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditHostRecord(int id, int item)
        {
            return this.View(new HostRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zone/{id:int}/edit/host/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditHostRecord(int id, int item, HostRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zone/{id:int}/edit/txt/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditTxtRecord(int id, int item)
        {
            return this.View(new TxtRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zone/{id:int}/edit/txt/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditTxtRecord(int id, int item, TxtRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }


        [HttpGet]
        [Route("/zone/{id:int}/edit/srv/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditSrvRecord(int id, int item)
        {
            return this.View(new SrvRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zone/{id:int}/edit/srv/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditSrvRecord(int id, int item, SrvRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zone/{id:int}/edit/sshfp/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditSshfpRecord(int id, int item)
        {
            return this.View(new SshfpRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zone/{id:int}/edit/sshfp/{item:int}")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult EditSshfpRecord(int id, int item, SshfpRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        
        #endregion

        [HttpGet]
        [Route("/zone/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult Delete(long item)
        {
            var obj = this.db.Zones
                .Include(x => x.Records)
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.ZoneGroup)
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.HorizonView)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/zone/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult Delete(int item, TopLevelDomain record)
        {
            var obj = this.db.Zones
                .Include(x => x.Records)
                .FirstOrDefault(x => x.Id == item);

            var zgm = this.db.ZoneGroups
                .Include(x => x.ZoneGroupMembers)
                .Where(x => x.ZoneGroupMembers.Any(y => y.ZoneId == obj.Id));
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            if (obj.Records.Any())
            {
                return this.RedirectToAction("Index");  
            }
            
            if (zgm.Any())
            {
                return this.RedirectToAction("Index");  
            }
          
            this.db.Zones.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }

    }
}