namespace LdapDnsWebApp.Models
{
    public class SshConnectionInfo
    {
        public bool UseSsh { get; set; }
        
        public string Username { set; get; }
        public string PrivateKeyPath { get; set; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        
        public uint LocalTunnelEndpoint { get; set; }
        public uint RemoteTunnelEndpointPort { get; set; }
        public string RemoteTunnelEndpointHost { get; set; }
    }
}