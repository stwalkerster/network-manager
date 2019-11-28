namespace LdapDnsWebApp.Models
{
    public class LdapConnectionInfo
    {
        public string BindTemplate { set; get; }
        public string Hostname { get; set; }
        public int Port { get; set; }
        public string BaseDn { get; set; }
        public string DnsBaseDn { get; set; }
    }
}