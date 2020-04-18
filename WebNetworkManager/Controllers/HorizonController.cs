namespace DnsWebApp.Controllers
{
    using System.Linq;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class HorizonController : Controller
    {
        private readonly DataContext db;

        public HorizonController(DataContext db)
        {
            this.db = db;
        }

        [Route("/horizon")]
        public IActionResult Index()
        {
            var horizons = this.db.HorizonViews
                .Include(x => x.Zones)
                .ToList();

            return this.View(horizons);
        }
        
        [HttpGet]
        [Route("/horizon/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New()
        {
            return this.View(new HorizonView());
        }
        
        [HttpPost]
        [Route("/horizon/new")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult New(HorizonView command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            this.db.HorizonViews.Add(command);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
        
        [HttpGet]
        [Route("/horizon/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item)
        {
            var obj = this.db.HorizonViews.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/horizon/{item:int}/edit")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Edit(int item, HorizonView editedView)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(editedView);
            }
            
            var obj = this.db.HorizonViews.FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }

            obj.ViewName = editedView.ViewName;
            obj.ViewTag = editedView.ViewTag;
            obj.ViewTagColour = editedView.ViewTagColour;

            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/horizon/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item)
        {
            var obj = this.db.HorizonViews
                .Include(x => x.Zones)
                .FirstOrDefault(x => x.Id == item);
            
            if (obj == null)
            {
                return this.RedirectToAction("Index");
            }
            
            return this.View(obj);
        }
        
        [HttpPost]
        [Route("/horizon/{item:int}/delete")]
        [Authorize(Roles = RoleDefinition.StaticData)]
        public IActionResult Delete(int item, HorizonView record)
        {
            var obj = this.db.HorizonViews
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
          
            this.db.HorizonViews.Remove(obj);
            this.db.SaveChanges();
            
            return this.RedirectToAction("Index");
        }
    }
}