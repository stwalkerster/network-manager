namespace LdapDnsWebApp.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using LdapDnsWebApp.Models;
    using LdapDnsWebApp.Services;
    using Novell.Directory.Ldap;

    public static class LdapManagerExtensions
    {
        public static List<ZoneSummary> GetZoneList(this ILdapManager ldapManager, string filter, WhoisService whoisService)
        {
            var ldapConnection = ldapManager.GetConnection();

            var ldapSearchResults = ldapConnection.Search(
                ldapManager.Configuration.DnsBaseDn,
                LdapConnection.SCOPE_SUB,
                filter,
                new[] {"associatedDomain", "o", "owner", "disabled"},
                false);

            var bareZones = new List<ZoneSummary>();
            
            var userDNs = new HashSet<string>();
            
            while (ldapSearchResults.hasMore())
            {
                var entry = ldapSearchResults.next();

                var org = entry.getAttribute("o")?.StringValue;

                var owner = entry.getAttribute("owner")?.StringValue;
                if (owner != null)
                {
                    userDNs.Add(owner);
                }

                var disabledAttr = entry.getAttribute("disabled")?.StringValue;
                var domain = entry.getAttribute("associatedDomain").StringValue;

                var whoisResult = whoisService.GetWhoisData(domain);
                
                var zone = new ZoneSummary
                {
                    ZoneName = domain, 
                    Registrar = org,
                    Owner = owner,
                    Enabled = disabledAttr == null || disabledAttr != "TRUE",
                    Expiry = whoisResult.Expiry,
                    Records = -1
                };
                
                bareZones.Add(zone);
            }

            var userData = LookupUsers(ldapManager, userDNs, "displayName");
            foreach (var zoneSummary in bareZones.Where(x => x.Owner != null))
            {
                zoneSummary.Owner = userData[zoneSummary.Owner];
            }

            return bareZones;
        }
        
        public static Dictionary<string, string> LookupUsers(this ILdapManager ldapManager, IEnumerable<string> userDNs, string attribute)
        {
            var result = new Dictionary<string, string>();
            
            var ldapConnection = ldapManager.GetConnection();
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