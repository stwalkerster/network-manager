namespace DnsWebApp.ViewComponents
{
    using System.Collections.Generic;
    using DnsWebApp.Models;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Novell.Directory.Ldap;

    public class SidebarMenuViewComponent : ViewComponent
    {
        private readonly ILdapManager ldapManager;
        private LdapConnectionInfo config;

        public SidebarMenuViewComponent(ILdapManager ldapManager, IOptions<LdapConnectionInfo> config)
        {
            this.ldapManager = ldapManager;
            this.config = config.Value;
        }

        public IViewComponentResult Invoke()
        {
            this.ldapManager.EnsureConnected(this.UserClaimsPrincipal);
            var ldapConnection = this.ldapManager.GetConnection();
            const string AssociatedDomainAttribute = "associatedDomain";

            var ldapSearchResults = ldapConnection.Search(
                this.config.DnsBaseDn,
                LdapConnection.SCOPE_SUB,
                "(&(soaRecord=*)(!(disabled=TRUE))(owner=" + this.ldapManager.GetCurrentUserDn() + "))",
                new[] {AssociatedDomainAttribute},
                false);

            var sidebarZoneList = new SidebarZoneList {Zones = new Dictionary<string, string>()};

            while (ldapSearchResults.hasMore())
            {
                var entry = ldapSearchResults.next();
                sidebarZoneList.Zones.Add(
                    entry.DN,
                    entry.getAttribute(AssociatedDomainAttribute).StringValue);
            }
            
            return this.View(sidebarZoneList);
        }
    }
}