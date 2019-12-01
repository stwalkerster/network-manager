using Microsoft.AspNetCore.Mvc;

namespace LdapDnsWebApp.Controllers
{
    using LdapDnsWebApp.Extensions;
    using LdapDnsWebApp.Services;

    public class ZoneController : Controller
    {
        private readonly ILdapManager ldapManager;
        private readonly WhoisService whoisService;

        public ZoneController(ILdapManager ldapManager, WhoisService whoisService)
        {
            this.ldapManager = ldapManager;
            this.whoisService = whoisService;
        }

        [Route("/zones")]
        public IActionResult Index()
        {
            this.ldapManager.EnsureConnected(this.User);
            var zoneSummaries = this.ldapManager.GetZoneList("(soaRecord=*)", this.whoisService);
            
            return this.View(zoneSummaries);
        }

        [Route("/zone/{zone}")]
        public IActionResult ShowZone(string zone)
        {
            this.ldapManager.EnsureConnected(this.User);
            return Content("zonefile for " + zone);
        }
    }
}