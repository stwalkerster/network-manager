namespace DnsWebApp.ViewComponents
{
    using System.Linq;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly DataContext db;
        private readonly UserManager<IdentityUser> userManager;

        public SidebarMenuViewComponent(DataContext db, UserManager<IdentityUser> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var favouriteZones = this.db.FavouriteDomains
                .Include(x => x.Zone)
                .ThenInclude(x => x.Domain)
                .ThenInclude(x => x.TopLevelDomain)
                .Include(x => x.Zone)
                .ThenInclude(x => x.HorizonView)
                .Where(x => x.UserId == this.userManager.GetUserId(this.HttpContext.User))
                .ToList();
            
            return this.View(favouriteZones);
        }
    }
}