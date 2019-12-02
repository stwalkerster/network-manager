namespace DnsWebApp.ViewComponents
{
    using System.Collections.Generic;
    using DnsWebApp.Models;
    using Microsoft.AspNetCore.Mvc;

    public class SidebarMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var sidebarZoneList = new SidebarZoneList {Zones = new Dictionary<string, string>()};

            
            return this.View(sidebarZoneList);
        }
    }
}