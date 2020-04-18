namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CurrencyController : Controller
    {
        private readonly DataContext db;

        public CurrencyController(DataContext db)
        {
            this.db = db;
        }

        [Route("/currency")]
        public IActionResult Index()
        {
            var currencies = this.db.Currencies
                .ToList();

            return this.View(currencies);
        }
        
        [HttpGet]
        [Route("/currency/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New()
        {
            return this.View(new Currency());
        }
        
        [HttpPost]
        [Route("/currency/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New(Currency command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            this.db.Currencies.Add(command);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/currency/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item)
        {
            var obj = this.db.Currencies.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/currency/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item, Currency editedView)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(editedView);
            }
            
            var obj = this.db.Currencies.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            obj.Code = editedView.Code;
            obj.Name = editedView.Name;
            obj.Symbol = editedView.Symbol;
            obj.ExchangeRate = editedView.ExchangeRate;
            obj.ExchangeRateUpdated = editedView.ExchangeRateUpdated;

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/currency/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item)
        {
            var obj = this.db.Currencies
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/currency/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item, Currency record)
        {
            var obj = this.db.Currencies
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            this.db.Currencies.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
    }
}