using Microsoft.AspNetCore.Mvc;

namespace LdapDnsWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Policy;
    using LdapDnsWebApp.Models;
    using LdapDnsWebApp.Services;
    using Microsoft.Extensions.Options;
    using Novell.Directory.Ldap;

    public class RegistrarController : Controller
    {
        private readonly ILdapManager ldapManager;
        private readonly LdapConnectionInfo config;

        public RegistrarController(ILdapManager ldapManager, IOptions<LdapConnectionInfo> config)
        {
            this.ldapManager = ldapManager;
            this.config = config.Value;
        }
        
        [Route("/registrar/{registrar}")]
        public IActionResult Registrar(string registrar)
        {
            string registrarFilter;
            if (registrar == "(none)")
            {
                registrarFilter = "(!(o=*))";
            }
            else
            {
                registrarFilter = "(o=" + registrar + ")";
            }

            var zoneSummaries = this.GetZoneList("(&(soaRecord=*)" + registrarFilter + ")");

            this.ViewData.Add("Registrar", registrar);
            
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

            var zones = new Dictionary<string, RegistrarZoneSummmary>();
            var enabledZones = new Dictionary<string,string>();
            var disabledZones = new Dictionary<string,string>();
            
            while (ldapSearchResults.hasMore())
            {
                var entry = ldapSearchResults.next();

                var org = entry.getAttribute("o")?.StringValue ?? "(none)";

                if (!zones.ContainsKey(org))
                {
                    zones.Add(org, new RegistrarZoneSummmary {Registrar = org, DisabledRecords = -1, EnabledRecords = -1});
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
                    disabledZones.Add(entry.DN, org);
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

        public List<ZoneSummary> GetZoneList(string filter)
        {
            this.ldapManager.EnsureConnected(this.User);
            var ldapConnection = this.ldapManager.GetConnection();

            var ldapSearchResults = ldapConnection.Search(
                this.config.DnsBaseDn,
                LdapConnection.SCOPE_SUB,
                filter,
                new[] {"associatedDomain", "o", "owner", "disabled"},
                false);

            var bareZones = new List<ZoneSummary>();
            
            var userDNs = new HashSet<string>();
            
            while (ldapSearchResults.hasMore())
            {
                var entry = ldapSearchResults.next();

                var org = entry.getAttribute("o")?.StringValue ?? "(none)";

                var owner = entry.getAttribute("owner")?.StringValue;
                if (owner != null)
                {
                    userDNs.Add(owner);
                }

                var disabledAttr = entry.getAttribute("disabled")?.StringValue;
                
                var zone = new ZoneSummary
                {
                    ZoneName = entry.getAttribute("associatedDomain").StringValue, 
                    Org = org,
                    Owner = owner,
                    Enabled = disabledAttr == null || disabledAttr != "TRUE"
                };
                
                bareZones.Add(zone);
            }

            var userData = this.LookupUsers(userDNs, "displayName");
            foreach (var zoneSummary in bareZones.Where(x => x.Owner != null))
            {
                zoneSummary.Owner = userData[zoneSummary.Owner];
            }

            return bareZones;
        }
        
        private Dictionary<string, string> LookupUsers(IEnumerable<string> userDNs, string attribute)
        {
            var result = new Dictionary<string, string>();
            
            var ldapConnection = this.ldapManager.GetConnection();
            foreach (var udn in userDNs)
            {
                result.Add(udn, udn);
                
                var ldapSearchResults = ldapConnection.Search(
                    udn,
                    LdapConnection.SCOPE_BASE,
                    "(objectClass=*)",
                    new[] {attribute},
                    false);

                if (!ldapSearchResults.hasMore())
                {
                    continue;
                }

                var r = ldapSearchResults.next();
                result[udn] = r.getAttribute(attribute)?.StringValue;
            }

            return result;
        }
    }
}