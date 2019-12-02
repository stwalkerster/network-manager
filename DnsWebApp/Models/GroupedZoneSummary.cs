namespace LdapDnsWebApp.Models
{
    public class GroupedZoneSummary
    {
        public string GroupName { get; set; }
        public string GroupKey { get; set; }
        public int EnabledZones { get; set; }
        public int EnabledRecords { get; set; }
        public int DisabledZones { get; set; }
        public int DisabledRecords { get; set; }
    }
}