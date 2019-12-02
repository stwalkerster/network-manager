namespace DnsWebApp.Services
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using DnsWebApp.Models;
    using Microsoft.Extensions.Options;
    using Novell.Directory.Ldap;

    public class LdapManager : ILdapManager, IDisposable
    {
        private readonly SshTunnelManager tunnelManager;
        private LdapConnection connection;
        public LdapConnectionInfo Configuration { get; }

        public LdapManager(IOptions<LdapConnectionInfo> config, SshTunnelManager tunnelManager)
        {
            this.tunnelManager = tunnelManager;
            this.Configuration = config.Value;
        }
        
        public string Connect(string username, string password)
        {
            this.Disconnect();
            
            this.tunnelManager.Start();
            
            this.connection = new LdapConnection();
            this.connection.Connect(this.Configuration.Hostname, this.Configuration.Port);

            var dn = string.Format(this.Configuration.BindTemplate, username);

            try
            {
                this.connection.Bind(dn, password);
            }
            catch (LdapException)
            {
                // squish.
                return null;
            }

            if (this.connection.Bound)
            {
                return dn;
            }

            return null;
        }

        private void Disconnect()
        {
            if (this.connection != null)
            {
                if (this.connection.Connected)
                {
                    this.connection.Disconnect();
                }

                this.connection.Dispose();
                this.connection = null;
            }
        }

        public LdapConnection GetConnection()
        {
            return this.connection;
        }

        public string GetCurrentUserDn()
        {
            return this.connection.AuthenticationDN;
        }

        public void EnsureConnected(ClaimsPrincipal userClaimsPrincipal)
        {
            if (this.GetConnection() == null)
            {
                var username = userClaimsPrincipal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var password = userClaimsPrincipal.Claims.First(x => x.Type == "Password").Value;
                this.Connect(username, password);
            }
        }

        public void Dispose()
        {
            this.connection?.Dispose();
        }
    }
}