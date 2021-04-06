using Microsoft.AspNetCore.Mvc;

namespace DnsWebApp.Controllers
{
    using System;
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.ViewModels;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;

    public class DomainController : Controller
    {
        private readonly DataContext db;
        private readonly WhoisService whoisService;

        public DomainController(DataContext db, WhoisService whoisService)
        {
            this.db = db;
            this.whoisService = whoisService;
        }
        
        [Route("/domain")]
        public IActionResult Index()
        {
            var domains = this.db.Domains
                .Include(x => x.Owner)
                .Include(x => x.Registrar)
                .Include(x => x.Zones)
                .Include(x => x.TopLevelDomain)
                .ToList();
            
            this.whoisService.UpdateExpiryAttributes(domains);
            
            return View(domains);
        }

        [HttpGet]
        [Route("/domain/new")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult New()
        {
            var domainCommand = new DomainCommand();
            this.ConfigureCommand(domainCommand);
            
            return this.View(domainCommand);
        }
        
        [HttpPost]
        [Route("/domain/new")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult New(DomainCommand command)
        {
            if (!this.ModelState.IsValid)
            {
                this.ConfigureCommand(command);
                return this.View(command);
            }

            var domain = new Domain();
            
            domain.Name = command.BaseName;
            domain.TopLevelDomainId = command.TopLevelDomain;
            domain.Placeholder = command.Placeholder;
            domain.OwnerId = command.Owner;
            domain.RegistrarId = command.Registrar;
            domain.LastUpdated = DateTime.UtcNow;
            domain.RegistrationExpiry = command.RegistrationExpiry;
            domain.WhoisLastUpdated = null;

            if (domain.Placeholder)
            {
                domain.OwnerId = null;
                domain.RegistrarId = null;
                domain.WhoisLastUpdated = null;
            }
            
            this.db.Domains.Add(domain);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }

        private void ConfigureCommand(DomainCommand command)
        {
            command.Registrars = this.db.Registrar.Select(x => new SelectListItem(x.Name, x.Id.ToString())).ToList();
            command.TopLevelDomains = this.db.TopLevelDomains.Select(x => new SelectListItem("." + x.Domain, x.Id.ToString()))
                .ToList();
            command.Owners = this.db.Users.Select(x => new SelectListItem(x.UserName, x.Id.ToString())).ToList();
        }

        [Route("/domain/{item:int}")]
        public IActionResult ShowZones(int item)
        {
            var zones = this.db.Zones
                .Include(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Owner)
                .Include(x => x.Domain)
                .ThenInclude(x => x.Registrar)
                .Include(x => x.HorizonView)
                .Include(x => x.FavouriteDomains)
                .ThenInclude(x => x.User)
                .Include(x => x.Records)
                .Where(x => x.Domain.Id == item)
                .ToList();

            var domain = this.db.Domains.Include(x => x.TopLevelDomain).FirstOrDefault(x => x.Id == item);
            this.ViewData["Domain"] = domain.Name + "." + domain.TopLevelDomain.Domain;
            return this.View(zones);
        }
        
        [HttpGet]
        [Route("/domain/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult Edit(int item)
        {
            var obj = this.db.Domains.Include(x => x.TopLevelDomain).FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            var domainCommand = new DomainCommand();
            this.ConfigureCommand(domainCommand);

            domainCommand.Name = obj.Name + "." + obj.TopLevelDomain.Domain;
            domainCommand.BaseName = obj.Name;
            domainCommand.TopLevelDomain = obj.TopLevelDomainId;
            domainCommand.RegistrationExpiry = obj.RegistrationExpiry;
            domainCommand.Owner = obj.OwnerId;
            domainCommand.Registrar = obj.RegistrarId;
            domainCommand.Placeholder = obj.Placeholder;
            domainCommand.Id = obj.Id;

            return this.View(domainCommand);
        }
        
        [HttpPost]
        [Route("/domain/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult Edit(int item, DomainCommand command)
        {
            if (!this.ModelState.IsValid)
            {
                this.ConfigureCommand(command);
                return this.View(command);
            }
            
            var domain = this.db.Domains.FirstOrDefault(x => x.Id == item);
            
            if (domain == null)
            {
                return this.RedirectToAction("Index");
            }

            domain.Placeholder = command.Placeholder;
            domain.OwnerId = command.Owner;
            domain.RegistrarId = command.Registrar;
            domain.LastUpdated = DateTime.UtcNow;
            domain.RegistrationExpiry = command.RegistrationExpiry;
            domain.WhoisLastUpdated = null;

            if (domain.Placeholder)
            {
                domain.OwnerId = null;
                domain.RegistrarId = null;
                domain.WhoisLastUpdated = null;
            }

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/domain/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult Delete(int item)
        {
            var obj = this.db.Domains
                .Include(x => x.Zones)
                .Include(x => x.TopLevelDomain)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/domain/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.DnsManager)]
        public IActionResult Delete(int item, TopLevelDomain record)
        {
            var obj = this.db.Domains
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
          
            this.db.Domains.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
    }
}