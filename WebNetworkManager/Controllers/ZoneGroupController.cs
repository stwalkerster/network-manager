using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.Dns;
    using DnsWebApp.Models.ViewModels;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class ZoneGroupController : Controller
    {
        private DataContext db;

        public ZoneGroupController(DataContext db)
        {
            this.db = db;
        }

        [Route("/zones/group/{item:int}")]
        public IActionResult ShowZoneGroup(int item)
        {
            var zoneGroupObject = this.db.ZoneGroups
                .Include(x => x.ZoneGroupMembers)
                .Include(x => x.Records)
                .FirstOrDefault(x => x.Id == item);
            
            if (zoneGroupObject == null)
            {
                return this.RedirectToAction("Index");
            }
            
            var display = new ZoneGroupDisplay();
            display.ZoneGroup = zoneGroupObject;
            
            foreach (var zoneRecord in zoneGroupObject.Records)
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
            
            return this.View(display);
        }
        
        [Route("/zones/group/{item:int}/zones")]
        public IActionResult Zones(int item)
        {
            var zones = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.HorizonView)
                .Include(x => x.FavouriteDomains).ThenInclude(x => x.User)
                .Include(x => x.ZoneGroupMembers).ThenInclude(x => x.ZoneGroup)
                .Include(x => x.Records)
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
                .ThenInclude(x => x.Records)
                .Include(x => x.Records)
                .ToList();


            var groupedZoneSummaries = zoneGroups.Select(
                x => new ZoneGroupSummary
                {
                    DisabledZones = x.ZoneGroupMembers.Count(y => !y.Zone.Enabled),
                    EnabledZones = x.ZoneGroupMembers.Count(y => y.Zone.Enabled),
                    EnabledRecords = x.ZoneGroupMembers.Where(y => y.Zone.Enabled).Aggregate(0, (agg, cur) => agg + cur.Zone.Records.Count),
                    DisabledRecords = x.ZoneGroupMembers.Where(y => !y.Zone.Enabled).Aggregate(0, (agg, cur) => agg + cur.Zone.Records.Count),
                    GroupRecords = x.Records.Count,
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
                AllZones = this.GetAllZones(),
                Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList(),
            });
        }

        [Route("/zones/group/new")]
        [HttpPost]
        public IActionResult NewZoneGroup(ZoneGroupCommand zoneGroupCommand)
        {
            if (!this.ModelState.IsValid)
            {
                zoneGroupCommand.AllZones = this.GetAllZones();
                zoneGroupCommand.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();
                return this.View(zoneGroupCommand);
            }

            var zoneGroup = new ZoneGroup
            {
                Name = zoneGroupCommand.Name,
                ZoneGroupMembers = new List<ZoneGroupMember>(),
                OwnerId = zoneGroupCommand.Owner
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
                .Include(x => x.Owner)
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
                AllZones = this.GetAllZones(),
                Owner = zoneGroupObject.OwnerId,
                Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList()
            };
            
            return this.View(zoneGroupCommand);
        }
        
        [Route("/zones/group/edit/{item:int}")]
        [HttpPost]
        public IActionResult EditZoneGroup(int item, ZoneGroupCommand command)
        {
            var zoneGroupObject = this.db.ZoneGroups
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.Zone)
                .FirstOrDefault(x => x.Id == item);

            if (zoneGroupObject == null)
            {
                return this.RedirectToAction("Index");
            }
            
            if (!this.ModelState.IsValid)
            {
                command.AllZones = this.GetAllZones();
                command.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();

                return this.View(command);
            }

            zoneGroupObject.Name = command.Name;
            zoneGroupObject.OwnerId = command.Owner;

            // needs to be done pre- and post-update of members, otherwise removed members don't get touched.
            zoneGroupObject.TouchSerialNumber();
            
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

            zoneGroupObject.TouchSerialNumber();
            
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        private dynamic GetAllZones()
        {
            var allZones = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.HorizonView)
                .Select(x => new {label = x.Name + "." + x.TopLevelDomain.Domain + (x.HorizonViewId.HasValue ? $" ({x.HorizonView.ViewTag})" : String.Empty), value = x.Id.ToString(CultureInfo.InvariantCulture)})
                .ToList();
            return allZones;
        }
        
        #region Records
        
        private IActionResult AddRecord(int id, RecordViewModelBase recordViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(recordViewModel);
            }

            var zoneGroup = this.db.ZoneGroups
                .Include(x => x.Records)
                .Include(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.Zone)
                .FirstOrDefault(x => x.Id == id);
            if (zoneGroup == null)
            {
                return this.RedirectToAction("Index");
            }

            zoneGroup.TouchSerialNumber();
            zoneGroup.Records.Add(recordViewModel.Record);

            this.db.SaveChanges();

            return this.RedirectToAction("ShowZoneGroup", new {item = id});
        }
        
        private IActionResult EditRecord(int item, RecordViewModelBase recordViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(recordViewModel);
            }
            
            var record = this.db.Record
                .Include(x => x.ZoneGroup)
                .ThenInclude(x => x.ZoneGroupMembers)
                .ThenInclude(x => x.Zone)
                .FirstOrDefault(x => x.Id == item);

            if (record == null)
            {
                return this.RedirectToAction("Index");
            }

            record.Name = recordViewModel.Record.Name;
            record.Value = recordViewModel.Record.Value;
            record.TimeToLive = recordViewModel.Record.TimeToLive;
            record.Type = recordViewModel.Type;
            
            record.ZoneGroup.TouchSerialNumber();

            this.db.SaveChanges();

            return this.RedirectToAction("ShowZoneGroup", new {item = record.ZoneGroupId});
        }
        
        private Record GetRecord(int item)
        {
            return this.db.Record
                .Include(x => x.Zone)
                .FirstOrDefault(x => x.Id == item);
        }

        [HttpGet]
        [Route("/zones/group/{id:int}/add/ns")]
        public IActionResult AddNsRecord(int id)
        {
            return this.View(
                new NsRecordViewModel(
                    new Record
                    {
                        ZoneGroupId = id,
                        Type = RecordType.NS,
                        Class = RecordClass.IN
                    }));
        }
        
        [HttpPost]
        [Route("/zones/group/{id:int}/add/ns")]
        public IActionResult AddNsRecord(int id, NsRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }

        [HttpGet]
        [Route("/zones/group/{id:int}/add/caa")]
        public IActionResult AddCaaRecord(int id)
        {
            return this.View(
                new CaaRecordViewModel(
                    new Record
                    {
                        ZoneGroupId = id,
                        Type = RecordType.CAA,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zones/group/{id:int}/add/caa")]
        public IActionResult AddCaaRecord(int id, CaaRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zones/group/{id:int}/add/mx")]
        public IActionResult AddMxRecord(int id)
        {
            return this.View(
                new MxRecordViewModel(
                    new Record
                    {
                        ZoneGroupId = id,
                        Type = RecordType.MX,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zones/group/{id:int}/add/mx")]
        public IActionResult AddMxRecord(int id, MxRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zones/group/{id:int}/add/host")]
        public IActionResult AddHostRecord(int id)
        {
            return this.View(
                new HostRecordViewModel(
                    new Record
                    {
                        ZoneGroupId = id,
                        Class = RecordClass.IN,
                        Type = RecordType.A
                    }));        
        }
        
        [HttpPost]
        [Route("/zones/group/{id:int}/add/host")]
        public IActionResult AddHostRecord(int id, HostRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zones/group/{id:int}/add/txt")]
        public IActionResult AddTxtRecord(int id)
        {
            return this.View(
                new TxtRecordViewModel(
                    new Record
                    {
                        ZoneGroupId = id,
                        Type = RecordType.TXT,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zones/group/{id:int}/add/txt")]
        public IActionResult AddTxtRecord(int id, TxtRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        
        [HttpGet]
        [Route("/zones/group/{id:int}/add/srv")]
        public IActionResult AddSrvRecord(int id)
        {
            return this.View(
                new SrvRecordViewModel(
                    new Record
                    {
                        ZoneGroupId = id,
                        Type = RecordType.SRV,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zones/group/{id:int}/add/srv")]
        public IActionResult AddSrvRecord(int id, SrvRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zones/group/{id:int}/add/sshfp")]
        public IActionResult AddSshfpRecord(int id)
        {
            return this.View(
                new SshfpRecordViewModel(
                    new Record
                    {
                        ZoneGroupId = id,
                        Type = RecordType.SSHFP,
                        Class = RecordClass.IN
                    }));        
        }
        
        [HttpPost]
        [Route("/zones/group/{id:int}/add/sshfp")]
        public IActionResult AddSshfpRecord(int id, SshfpRecordViewModel recordViewModel)
        {
            return this.AddRecord(id, recordViewModel);
        }
        
        [HttpGet]
        [Route("/zones/group/{id:int}/edit/ns/{item:int}")]
        public IActionResult EditNsRecord(int id, int item)
        {
            return this.View(new NsRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zones/group/{id:int}/edit/ns/{item:int}")]
        public IActionResult EditNsRecord(int id, int item, NsRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zones/group/{id:int}/edit/caa/{item:int}")]
        public IActionResult EditCaaRecord(int id, int item)
        {
            return this.View(new CaaRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zones/group/{id:int}/edit/caa/{item:int}")]
        public IActionResult EditCaaRecord(int id, int item, CaaRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zones/group/{id:int}/edit/mx/{item:int}")]
        public IActionResult EditMxRecord(int id, int item)
        {
            return this.View(new MxRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zones/group/{id:int}/edit/mx/{item:int}")]
        public IActionResult EditMxRecord(int id, int item, MxRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zones/group/{id:int}/edit/host/{item:int}")]
        public IActionResult EditHostRecord(int id, int item)
        {
            return this.View(new HostRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zones/group/{id:int}/edit/host/{item:int}")]
        public IActionResult EditHostRecord(int id, int item, HostRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zones/group/{id:int}/edit/txt/{item:int}")]
        public IActionResult EditTxtRecord(int id, int item)
        {
            return this.View(new TxtRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zones/group/{id:int}/edit/txt/{item:int}")]
        public IActionResult EditTxtRecord(int id, int item, TxtRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }


        [HttpGet]
        [Route("/zones/group/{id:int}/edit/srv/{item:int}")]
        public IActionResult EditSrvRecord(int id, int item)
        {
            return this.View(new SrvRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zones/group/{id:int}/edit/srv/{item:int}")]
        public IActionResult EditSrvRecord(int id, int item, SrvRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        [HttpGet]
        [Route("/zones/group/{id:int}/edit/sshfp/{item:int}")]
        public IActionResult EditSshfpRecord(int id, int item)
        {
            return this.View(new SshfpRecordViewModel(this.GetRecord(item)));
        }

        [HttpPost]
        [Route("/zones/group/{id:int}/edit/sshfp/{item:int}")]
        public IActionResult EditSshfpRecord(int id, int item, SshfpRecordViewModel recordViewModel)
        {
            return this.EditRecord(item, recordViewModel);
        }

        #endregion
    }
}