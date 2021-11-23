namespace DnsWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Models.ViewModels;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public class RegistrarController : Controller
    {
        private readonly DataContext db;
        private readonly WhoisService whoisService;
        private readonly IConfiguration configuration;

        public RegistrarController(DataContext db, WhoisService whoisService, IConfiguration configuration)
        {
            this.db = db;
            this.whoisService = whoisService;
            this.configuration = configuration;
        }

        [Route("/registrar/{item}")]
        public IActionResult Item(long item)
        {
            var registrar = this.db.Registrar
                .Include(x => x.Domains).ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Domains).ThenInclude(x => x.Owner)
                .Include(x => x.Domains).ThenInclude(x => x.Registrar)
                .Include(x => x.Domains).ThenInclude(x => x.Zones)
                .FirstOrDefault(x => x.Id == item);

            if (registrar == null)
            {
                return this.RedirectToAction("Index");
            }

            this.whoisService.UpdateExpiryAttributes(registrar.Domains);
            
            this.ViewData["Registrar"] = registrar.Name;
            this.ViewData["RegistrarId"] = registrar.Id;
            
            return this.View(registrar.Domains);
        }

        [Route("/registrar")]
        public IActionResult Index()
        {
            var baseCurrency = this.db.Currencies.FirstOrDefault(x => x.Code == this.configuration.GetValue<string>("BaseCurrency"));
            
            var includableQueryable = this.db.Registrar
                .Include(x => x.Domains)
                .ThenInclude(x => x.Zones)
                .ThenInclude(x => x.Records)
                .Include(x => x.RegistrarTldSupports)
                .Include(x => x.Currency)
                .Select(x => new RegistrarDisplay(x, baseCurrency))
                .ToList();

            return this.View(includableQueryable);
        }
        
        #region registrar crud
        
        [HttpGet]
        [Route("/registrar/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New()
        {
            this.ViewBag.Currencies = this.db.Currencies.Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString())).ToList();

            return this.View(new Registrar());
        }
        
        [HttpPost]
        [Route("/registrar/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New(Registrar command)
        {
            if (!this.ModelState.IsValid)
            {            
                this.ViewBag.Currencies = this.db.Currencies.Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString())).ToList();
                return this.View(command);
            }

            this.db.Registrar.Add(command);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item)
        {
            var obj = this.db.Registrar.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            this.ViewBag.Currencies = this.db.Currencies.Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString())).ToList();
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item, Registrar editedRegistrar)
        {
            if (!this.ModelState.IsValid)
            {
                this.ViewBag.Currencies = this.db.Currencies.Select(x => new SelectListItem($"{x.Name} ({x.Code})", x.Id.ToString())).ToList();
                return this.View(editedRegistrar);
            }
            
            var obj = this.db.Registrar.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            obj.Name = editedRegistrar.Name;
            obj.PricesIncludeVat = editedRegistrar.PricesIncludeVat;
            obj.CurrencyId = editedRegistrar.CurrencyId;
            obj.AllowRenewals = editedRegistrar.AllowRenewals;
            obj.AllowTransfers = editedRegistrar.AllowTransfers;
            obj.TransferOutFee = editedRegistrar.TransferOutFee;
            obj.PrivacyFee = editedRegistrar.PrivacyFee;

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item)
        {
            var obj = this.db.Registrar
                .Include(x => x.Domains)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item, Registrar record)
        {
            var obj = this.db.Registrar
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
          
            this.db.Registrar.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        #endregion
        
        #region TLD support
        [HttpGet]
        [Route("/registrar/{item:int}/tlds")]
        public IActionResult TldList(int item)
        {
            var registrar = this.db.Registrar.FirstOrDefault(x => x.Id == item);
            
            if (registrar == null)
            {
                return this.RedirectToAction("Index");
            }

            var supports = this.db.RegistrarTldSupport
                .Include(x => x.TopLevelDomain)
                .ThenInclude(x => x.Domains)
                .ThenInclude(x => x.Zones)
                .Include(x => x.Registrar)
                .ThenInclude(x => x.Currency)
                .Where(x => x.RegistrarId == item)
                .ToList();

            var targetCurrency = this.db.Currencies.FirstOrDefault(x => x.Code == this.configuration.GetValue<string>("BaseCurrency"));
            
            this.ViewData["Registrar"] = registrar.Name;
            this.ViewData["RegistrarId"] = registrar.Id;
            
            return this.View(supports.Select(x => new TldSupportDisplay(x, targetCurrency, this.configuration.GetValue<decimal>("Vat"))).ToList());
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/tlds/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult TldAdd(int item)
        {
            var supportedTlds = this.db.RegistrarTldSupport.Where(x => x.RegistrarId == item)
                .Select(x => x.TopLevelDomainId)
                .ToList();
            
            this.ViewBag.TopLevelDomains = this.db.TopLevelDomains
                .Where(x => !supportedTlds.Contains(x.Id))
                .OrderBy(x => x.Domain)
                .Select(x => new SelectListItem("." + x.Domain, x.Id.ToString()))
                .ToList();

            return this.View(new RegistrarTldSupport{ RegistrarId = item});
        }
        
        [HttpPost]
        [Route("/registrar/{item:int}/tlds/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult TldAdd(int item, RegistrarTldSupport command)
        {
            if (!this.ModelState.IsValid)
            {
                var supportedTlds = this.db.RegistrarTldSupport.Where(x => x.RegistrarId == item)
                    .Select(x => x.TopLevelDomainId)
                    .ToList();
            
                this.ViewBag.TopLevelDomains = this.db.TopLevelDomains
                    .Where(x => !supportedTlds.Contains(x.Id))
                    .OrderBy(x => x.Domain)
                    .Select(x => new SelectListItem("." + x.Domain, x.Id.ToString()))
                    .ToList();
                
                return this.View(command);
            }


            command.RenewalPriceUpdated = DateTime.Now;
            
            this.db.RegistrarTldSupport.Add(command);
            this.db.SaveChanges();
            
            return this.RedirectToAction("TldList", new{item = item});
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/tlds/{tld:int}")]
        [Authorize(Roles = RoleDefinition.StaticData)]
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
        [Authorize(Roles = RoleDefinition.StaticData)]
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
            obj.TransferPrice = command.TransferPrice;
            obj.RenewalPriceUpdated = DateTime.Now;
            obj.TransferIncludesRenewal = command.TransferIncludesRenewal;
            obj.PrivacyIncluded = command.PrivacyIncluded;
            obj.AllowInboundTransfer = command.AllowInboundTransfer;

            this.db.SaveChanges();
            
            return this.RedirectToAction("TldList", new{item = item});
        }
        
        [HttpGet]
        [Route("/registrar/{item:int}/tld/{tld:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
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
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult TldDelete(int item, int tld, RegistrarTldSupport record)
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