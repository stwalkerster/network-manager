namespace DnsWebApp.Models.ViewModels
{
    using DnsWebApp.Models.Database;

    public class ZoneDisplay
    {
        public ZoneDisplay()
        {
            this.CaaRecords = new DnsRecordset();
            this.HostRecords = new DnsRecordset();
            this.MxRecords = new DnsRecordset();
            this.NsRecords = new DnsRecordset();
            this.SrvRecords = new DnsRecordset();
            this.SshfpRecords = new DnsRecordset();
            this.TxtRecords = new DnsRecordset();
        }
        
        public Zone Zone { get; set; }
        public string Fqdn { get; set; }
        
        public DnsRecordset NsRecords { get; }
        public DnsRecordset CaaRecords { get; }
        public DnsRecordset MxRecords { get; }
        public DnsRecordset TxtRecords { get; }
        public DnsRecordset SrvRecords { get; }
        public DnsRecordset SshfpRecords { get; }
        public DnsRecordset HostRecords { get; }
    }
}