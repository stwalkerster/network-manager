namespace LdapDnsWebApp.Services
{
    using LdapDnsWebApp.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Renci.SshNet;

    public class SshTunnelManager
    {
        private readonly SshConnectionInfo config;
        private readonly ILogger<SshTunnelManager> log;
        private SshClient client;

        public SshTunnelManager(IOptions<SshConnectionInfo> config, ILogger<SshTunnelManager> log)
        {
            // this.log.LogDebug("Initialising SSH tunnel manager");
            this.config = config.Value;
            this.log = log;
        }

        public void Start()
        {
            if (this.client != null)
            {
                if (this.client.IsConnected)
                {
                    return;
                }

                this.client = null;
            }

            if (!this.config.UseSsh)
            {
                // this.log.LogInformation("SSH connectivity is disabled.");
                return;
            }

            // this.log.LogInformation("Connecting to SSH...");

            var key = new PrivateKeyAuthenticationMethod(
                this.config.Username,
                new PrivateKeyFile(this.config.PrivateKeyPath));

            var connectionInfo = new ConnectionInfo(this.config.Hostname, this.config.Port, this.config.Username, key);

            this.client = new SshClient(connectionInfo);
            this.client.Connect();
            var forwardedSshPort = new ForwardedPortLocal(
                this.config.LocalTunnelEndpoint,
                this.config.RemoteTunnelEndpointHost,
                this.config.RemoteTunnelEndpointPort);
            this.client.AddForwardedPort(forwardedSshPort);
            forwardedSshPort.Start();
            
            // this.log.LogInformation("Connected to SSH.");
        }

        public void Stop()
        {
            if (this.client == null)
            {
                return;
            }

            if (this.client.IsConnected)
            {
                this.client.Disconnect();
                // this.log.LogInformation("Disconnected from SSH.");
            }

            this.client = null;
        }
    }
}