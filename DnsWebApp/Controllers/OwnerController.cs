using Microsoft.AspNetCore.Mvc;

namespace LdapDnsWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using LdapDnsWebApp.Extensions;
    using LdapDnsWebApp.Models;
    using LdapDnsWebApp.Services;
    using Novell.Directory.Ldap;

    public class OwnerController : Controller
    {
        private readonly ILdapManager ldapManager;
        private readonly WhoisService whoisService;
        private const string NoneSpecifier = "(none)";

        public OwnerController(ILdapManager ldapManager, WhoisService whoisService)
        {
            this.ldapManager = ldapManager;
            this.whoisService = whoisService;
        }
        
        [Route("/owner/{item}")]
        public IActionResult Item(string item)
        {
            this.ldapManager.EnsureConnected(this.User);
            
            string ownerFilter;
            if (item == NoneSpecifier)
            {
                this.ViewData.Add("Owner", NoneSpecifier);

                ownerFilter = "(!(owner=*))";
            }
            else
            {
                var filter = "(uid=" + item + ")";
            
                
                var uidResult = this.ldapManager.GetConnection()
                    .Search(
                        this.ldapManager.Configuration.BaseDn,
                        LdapConnection.SCOPE_SUB,
                        filter,
                        new []{"displayName"},
                        false);

                if (!uidResult.hasMore())
                {
                    return this.View(new List<ZoneSummary>());
                }

                var ldapEntry = uidResult.next();
                var udn = ldapEntry.DN;
                
                ownerFilter = "(owner=" + udn + ")";
                this.ViewData.Add("Owner", ldapEntry.getAttribute("displayName").StringValue);
            }
            
            var zoneSummaries = this.ldapManager.GetZoneList("(&(soaRecord=*)" + ownerFilter + ")", this.whoisService);
            
            return this.View(zoneSummaries);
        }
        
        [Route("/owner")]
        public IActionResult Index()
        {
            this.ldapManager.EnsureConnected(this.User);
            var ldapConnection = this.ldapManager.GetConnection();

            var ldapSearchResults = ldapConnection.Search(
                this.ldapManager.Configuration.DnsBaseDn,
                LdapConnection.SCOPE_SUB,
                "(soaRecord=*)",
                new[] {"associatedDomain", "owner", "disabled"},
                false);

            var zones = new Dictionary<string, GroupedZoneSummary>();
            var enabledZones = new Dictionary<string,string>();
            // var disabledZones = new Dictionary<string,string>();
            
            while (ldapSearchResults.hasMore())
            {
                var entry = ldapSearchResults.next();

                var owner = entry.getAttribute("owner")?.StringValue ?? NoneSpecifier;

                if (!zones.ContainsKey(owner))
                {
                    zones.Add(owner, new GroupedZoneSummary {GroupName = owner, DisabledRecords = -1, EnabledRecords = -1});
                }

                var disabledAttr = entry.getAttribute("disabled")?.StringValue;

                if (disabledAttr == null || disabledAttr != "TRUE")
                {
                    zones[owner].EnabledZones++;
                    enabledZones.Add(entry.DN, owner);
                }
                else
                {
                    zones[owner].DisabledZones++;
//                    disabledZones.Add(entry.DN, owner);
                }
            }

            foreach (var zoneDn in enabledZones)    
            {
                var zoneRecords = ldapConnection.Search(
                    zoneDn.Key,
                    LdapConnection.SCOPE_SUB,
                    "(objectclass=domainrelatedobject)",
                    new[] {"associatedDomain", "owner", "disabled"},
                    false);
                
                // todo - count zone records
            }

            var userDns = zones.Where(x => x.Value.GroupName != NoneSpecifier).Select(x => x.Value.GroupName).Distinct().ToList();
            var names = this.ldapManager.LookupUsers(userDns, "displayName");
            var uids = this.ldapManager.LookupUsers(userDns, "uid");
            
            foreach (var summary in zones.Where(x => x.Value.GroupName != NoneSpecifier))
            {
                summary.Value.GroupKey = uids[summary.Value.GroupName];
                summary.Value.GroupName = names[summary.Value.GroupName];
            }
            
            foreach (var summary in zones.Where(x => x.Value.GroupName == NoneSpecifier))
            {
                summary.Value.GroupKey = NoneSpecifier;
            }

            return this.View(zones);
        }

    }
}