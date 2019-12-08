using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models.Database;
    using Microsoft.EntityFrameworkCore;

    public class TopLevelDomainController : Controller
    {
        private readonly DataContext db;

        public TopLevelDomainController(DataContext db)
        {
            this.db = db;
        }
        
        [Route("/tld")]
        public IActionResult Index()
        {
            return View(this.db.TopLevelDomains.Include(x => x.Zones).ToList());
        }

        [HttpGet]
        [Route("/tld/new")]
        public IActionResult New()
        {
            return this.View(new TopLevelDomain());
        }
        
        [HttpPost]
        [Route("/tld/new")]
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
        public IActionResult ShowZones(int item)
        {
            var zones = this.db.Zones
                .Include(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.FavouriteDomains)
                .ThenInclude(x => x.User)
                .Where(x => x.TopLevelDomainId == item)
                .ToList();

            this.ViewData["TLD"] = this.db.TopLevelDomains.FirstOrDefault(x => x.Id == item)?.Domain;
            return this.View(zones);
        }
        
        [HttpGet]
        [Route("/tld/{item:int}/edit")]
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

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
    }
}