namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class RegistrarController : Controller
    {
        private readonly DataContext db;
        private readonly WhoisService whoisService;

        public RegistrarController(DataContext db, WhoisService whoisService)
        {
            this.db = db;
            this.whoisService = whoisService;
        }

        [Route("/registrar/{item}")]
        public IActionResult Item(long item)
        {
            var registrar = this.db.Registrar
                .Include(x => x.Zones)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Zones)
                .ThenInclude(x => x.Owner)
                .Include(x => x.Zones)
                .ThenInclude(x => x.FavouriteDomains)
                .ThenInclude(x => x.User)
                .Include(x => x.Zones)
                .ThenInclude(x => x.Records)
                .FirstOrDefault(x => x.Id == item);

            if (registrar == null)
            {
                return this.RedirectToAction("Index");
            }
            
            var zoneSummaries = registrar.Zones.ToList();

            this.whoisService.UpdateExpiryAttributes(zoneSummaries);
            
            this.ViewData["Registrar"] = registrar.Name;
            this.ViewData["RegistrarId"] = registrar.Id;
            
            return this.View(zoneSummaries);
        }

        [Route("/registrar")]
        public IActionResult Index()
        {
            var includableQueryable = this.db.Registrar
                .Include(x => x.Zones)
                .ThenInclude(x => x.Records)
                .ToList();

            var groupedZoneSummaries = includableQueryable.Select(
                x => new GroupedZoneSummary
                {
                    DisabledZones = x.Zones.Count(y => !y.Enabled),
                    EnabledZones = x.Zones.Count(y => y.Enabled),
                    EnabledRecords = x.Zones.Where(y => y.Enabled).Aggregate(0, (agg, cur) => agg + cur.Records.Count),
                    DisabledRecords = x.Zones.Where(y => !y.Enabled).Aggregate(0, (agg, cur) => agg + cur.Records.Count),
                    GroupKey = x.Id.ToString(),
                    GroupName = x.Name
                });

            return this.View(groupedZoneSummaries.ToDictionary(x => x.GroupKey));
        }
        
        [HttpGet]
        [Route("/registrar/new")]
        public IActionResult New()
        {
            return this.View(new Registrar());
        }
        
        [HttpPost]
        [Route("/registrar/new")]
        public IActionResult New(Registrar command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            this.db.Registrar.Add(command);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/edit")]
        public IActionResult Edit(int item)
        {
            var obj = this.db.Registrar.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/edit")]
        public IActionResult Edit(int item, Registrar editedRegistrar)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(editedRegistrar);
            }
            
            var obj = this.db.Registrar.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            obj.Name = editedRegistrar.Name;

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/delete")]
        public IActionResult Delete(int item)
        {
            var obj = this.db.Registrar
                .Include(x => x.Zones)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/delete")]
        public IActionResult Delete(int item, TopLevelDomain record)
        {
            var obj = this.db.Registrar
                .Include(x => x.Zones)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            if (obj.Zones.Any())
            {
                return this.RedirectToAction("Index");  
            }
          
            this.db.Registrar.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }

        #region TLD support
        [HttpGet]
        [Route("/registrar/{item:int}/tlds")]
        public IActionResult TldList(int item)
        {
            var registrar = this.db.Registrar
                .Include(x => x.RegistrarTldSupports)
                .ThenInclude(x => x.TopLevelDomain)
                .ThenInclude(x => x.Zones)
                .FirstOrDefault(x => x.Id == item);

            if (registrar == null)
            {
                return this.RedirectToAction("Index");
            }
            
            this.ViewData["Registrar"] = registrar.Name;
            this.ViewData["RegistrarId"] = registrar.Id;
            
            return this.View(registrar.RegistrarTldSupports);
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/tlds/new")]
        public IActionResult TldAdd(int item)
        {
            var supportedTlds = this.db.RegistrarTldSupport.Where(x => x.RegistrarId == item)
                .Select(x => x.TopLevelDomainId)
                .ToList();
            
            this.ViewBag.TopLevelDomains = this.db.TopLevelDomains
                .Where(x => !supportedTlds.Contains(x.Id))
                .Select(x => new SelectListItem("." + x.Domain, x.Id.ToString()))
                .ToList();

            return this.View(new RegistrarTldSupport{ RegistrarId = item});
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/tlds/new")]
        public IActionResult TldAdd(int item, RegistrarTldSupport command)
        {
            if (!this.ModelState.IsValid)
            {
                var supportedTlds = this.db.RegistrarTldSupport.Where(x => x.RegistrarId == item)
                    .Select(x => x.TopLevelDomainId)
                    .ToList();
            
                this.ViewBag.TopLevelDomains = this.db.TopLevelDomains
                    .Where(x => !supportedTlds.Contains(x.Id))
                    .Select(x => new SelectListItem("." + x.Domain, x.Id.ToString()))
                    .ToList();
                
                return this.View(command);
            }

            this.db.RegistrarTldSupport.Add(command);
            this.db.SaveChanges();
            
            return this.RedirectToAction("TldList", new{item = item});
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/tlds/{tld:int}")]
        public IActionResult TldEdit(int item, int tld)
        {
            var obj = this.db.RegistrarTldSupport.FirstOrDefault(x => x.Id == tld);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/tlds/{tld:int}")]
        public IActionResult TldEdit(int item, int tld, RegistrarTldSupport command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            var obj = this.db.RegistrarTldSupport.FirstOrDefault(x => x.Id == tld);
            
            if (obj == null)
            {
                return this.RedirectToAction("TldList", new{item = item});
            }

            obj.RenewalPrice = command.RenewalPrice;
            
            this.db.SaveChanges();
            
            return this.RedirectToAction("TldList", new{item = item});
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/tld/{tld:int}/delete")]
        public IActionResult TldDelete(int item, int tld)
        {
            var obj = this.db.RegistrarTldSupport
                .Include(x => x.Registrar)
                .Include(x => x.TopLevelDomain)
                .FirstOrDefault(x => x.Id == tld);
            
            if (obj == null)
            {
                return this.RedirectToAction("TldList", new {item = item});
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/tld/{tld:int}/delete")]
        public IActionResult Delete(int item, int tld, RegistrarTldSupport record)
        {
            var obj = this.db.RegistrarTldSupport
                .FirstOrDefault(x => x.Id == tld);
            
            if (obj == null)
            {
                return this.RedirectToAction("TldList", new {item = item});
            }
          
            this.db.RegistrarTldSupport.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("TldList", new {item = item});
        }
        
        #endregion
    }
}