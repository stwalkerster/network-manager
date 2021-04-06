using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore;

    public class TopLevelDomainController : Controller
    {
        private readonly DataContext db;
        private readonly WhoisService whoisService;

        public TopLevelDomainController(DataContext db, WhoisService whoisService)
        {
            this.db = db;
            this.whoisService = whoisService;
        }
        
        [Route("/tld")]
        public IActionResult Index()
        {
            return View(this.db.TopLevelDomains.Include(x => x.Domains).ToList());
        }

        [HttpGet]
        [Route("/tld/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New()
        {
            return this.View(new TopLevelDomain());
        }
        
        [HttpPost]
        [Route("/tld/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New(TopLevelDomain command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            this.db.TopLevelDomains.Add(command);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [Route("/tld/{item:int}")]
        public IActionResult ShowDomains(int item)
        {
            var domains = this.db.Domains
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.Zones)
                .Include(x => x.TopLevelDomain)
                .Where(x => x.TopLevelDomainId == item)
                .ToList();
            
            this.whoisService.UpdateExpiryAttributes(domains);

            this.ViewData["TLD"] = this.db.TopLevelDomains.FirstOrDefault(x => x.Id == item)?.Domain;
            return this.View(domains);
        }
        
        [HttpGet]
        [Route("/tld/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item)
        {
            var obj = this.db.TopLevelDomains.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/tld/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item, TopLevelDomain editedTld)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(editedTld);
            }
            
            var obj = this.db.TopLevelDomains.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            obj.Registry = editedTld.Registry;
            obj.RegistryUrl = editedTld.RegistryUrl;
            obj.WhoisServer = editedTld.WhoisServer;
            obj.WhoisExpiryDateFormat = editedTld.WhoisExpiryDateFormat;

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/tld/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item)
        {
            var obj = this.db.TopLevelDomains
                .Include(x => x.Domains)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/tld/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item, TopLevelDomain record)
        {
            var obj = this.db.TopLevelDomains
                .Include(x => x.Domains)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            if (obj.Domains.Any())
            {
                return this.RedirectToAction("Index");  
            }
          
            this.db.TopLevelDomains.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
    }
}