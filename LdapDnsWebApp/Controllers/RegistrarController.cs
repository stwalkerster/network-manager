using Microsoft.AspNetCore.Mvc;

namespace LdapDnsWebApp.Controllers
{
    using System.Collections.Generic;
    using LdapDnsWebApp.Extensions;
    using LdapDnsWebApp.Models;
    using LdapDnsWebApp.Services;
    using Microsoft.Extensions.Options;
    using Novell.Directory.Ldap;

    public class RegistrarController : Controller
    {
        private readonly ILdapManager ldapManager;
        private readonly WhoisService whoisService;
        private readonly LdapConnectionInfo config;

        public RegistrarController(ILdapManager ldapManager, IOptions<LdapConnectionInfo> config, WhoisService whoisService)
        {
            this.ldapManager = ldapManager;
            this.whoisService = whoisService;
            this.config = config.Value;
        }
        
        [Route("/registrar/{item}")]
        public IActionResult Item(string item)
        {
            string registrarFilter;
            if (item == "(none)")
            {
                registrarFilter = "(!(o=*))";
            }
            else
            {
                registrarFilter = "(o=" + item + ")";
            }

            this.ldapManager.EnsureConnected(this.User);
            var zoneSummaries = this.ldapManager.GetZoneList("(&(soaRecord=*)" + registrarFilter + ")", this.whoisService);

            this.ViewData.Add("Registrar", item);
            
            return this.View(zoneSummaries);
        }
        
        [Route("/registrar")]
        public IActionResult Index()
        {
            this.ldapManager.EnsureConnected(this.User);
            var ldapConnection = this.ldapManager.GetConnection();

            var ldapSearchResults = ldapConnection.Search(
                this.config.DnsBaseDn,
                LdapConnection.SCOPE_SUB,
                "(soaRecord=*)",
                new[] {"associatedDomain", "o", "disabled"},
                false);

            var zones = new Dictionary<string, GroupedZoneSummary>();
            var enabledZones = new Dictionary<string,string>();
//            var disabledZones = new Dictionary<string,string>();
            
            while (ldapSearchResults.hasMore())
            {
                var entry = ldapSearchResults.next();

                var org = entry.getAttribute("o")?.StringValue ?? "(none)";

                if (!zones.ContainsKey(org))
                {
                    zones.Add(org, new GroupedZoneSummary
                    {
                        GroupName = org, GroupKey=org,DisabledRecords = -1, EnabledRecords = -1
                    });
                }

                var disabledAttr = entry.getAttribute("disabled")?.StringValue;

                if (disabledAttr == null || disabledAttr != "TRUE")
                {
                    zones[org].EnabledZones++;
                    enabledZones.Add(entry.DN, org);
                }
                else
                {
                    zones[org].DisabledZones++;
//                    disabledZones.Add(entry.DN, org);
                }
            }

            foreach (var zoneDn in enabledZones)    
            {
                var zoneRecords = ldapConnection.Search(
                    zoneDn.Key,
                    LdapConnection.SCOPE_SUB,
                    "(objectclass=domainrelatedobject)",
                    new[] {"associatedDomain", "o", "disabled"},
                    false);
                
                // todo - count zone records
            }

            return this.View(zones);
        }
    }
}