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
            var favouriteDomains = this.db.FavouriteDomains
                .Include(x => x.Zone)
                .ThenInclude(x => x.TopLevelDomain)
                .Where(x => x.UserId == this.userManager.GetUserId(this.HttpContext.User))
                .ToDictionary(z => z.Id, x => x.Zone.Name + "." + x.Zone.TopLevelDomain.Domain);

            return this.View(favouriteDomains);
        }
    }
}