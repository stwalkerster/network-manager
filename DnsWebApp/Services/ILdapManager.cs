namespace DnsWebApp.Services
{
    using System.Security.Claims;
    using DnsWebApp.Models;
    using Novell.Directory.Ldap;

    public interface ILdapManager
    {
        string Connect(string username, string password);

        LdapConnection GetConnection();
        void EnsureConnected(ClaimsPrincipal userClaimsPrincipal);
        string GetCurrentUserDn();
        LdapConnectionInfo Configuration { get; }
    }
}