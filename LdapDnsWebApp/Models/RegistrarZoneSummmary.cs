namespace LdapDnsWebApp.Models
{
    public class RegistrarZoneSummmary
    {
        public string Registrar { get; set; }
        public int EnabledZones { get; set; }
        public int EnabledRecords { get; set; }
        public int DisabledZones { get; set; }
        public int DisabledRecords { get; set; }
    }
}