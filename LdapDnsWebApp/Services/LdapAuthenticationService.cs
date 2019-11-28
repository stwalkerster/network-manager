namespace LdapDnsWebApp.Services
{
    using LdapDnsWebApp.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Novell.Directory.Ldap;

    public class LdapAuthenticationService : IAuthenticationService
    {
        private readonly ILogger<LdapAuthenticationService> logger;
        private readonly ILdapManager ldapManager;
        private LdapConnectionInfo config;

        private const string DisplayNameAttribute = "displayName";

        public LdapAuthenticationService(
            IOptions<LdapConnectionInfo> config,
            ILogger<LdapAuthenticationService> logger,
            ILdapManager ldapManager)
        {
            this.logger = logger;
            this.ldapManager = ldapManager;
            this.config = config.Value;
        }

        public AppUser Login(string username, string password)
        {
            var dn = this.ldapManager.Connect(username, password);

            if (dn == null)
            {
                return null;
            }

            var ldapConnection = this.ldapManager.GetConnection();

            var searchResult = ldapConnection.Search(
                dn,
                LdapConnection.SCOPE_BASE,
                "(objectClass=*)",
                new[] {DisplayNameAttribute},
                false);

            var user = searchResult.next();

            return new AppUser
            {
                DisplayName = user.getAttribute(DisplayNameAttribute).StringValue,
                DistinguishedName = dn,
                IsAdmin = false,
                Username = username,
                Password = password
            };
        }
    }
}