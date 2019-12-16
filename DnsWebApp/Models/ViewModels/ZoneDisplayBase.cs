namespace DnsWebApp.Models.ViewModels
{
    public abstract class ZoneDisplayBase {
        protected ZoneDisplayBase()
        {
            this.CaaRecords = new DnsRecordset();
            this.HostRecords = new DnsRecordset();
            this.MxRecords = new DnsRecordset();
            this.NsRecords = new DnsRecordset();
            this.SrvRecords = new DnsRecordset();
            this.SshfpRecords = new DnsRecordset();
            this.TxtRecords = new DnsRecordset();
        }
        
        public virtual uint? DefaultTTL { get; }
        
        public DnsRecordset NsRecords { get; }
        public DnsRecordset CaaRecords { get; }
        public DnsRecordset MxRecords { get; }
        public DnsRecordset TxtRecords { get; }
        public DnsRecordset SrvRecords { get; }
        public DnsRecordset SshfpRecords { get; }
        public DnsRecordset HostRecords { get; }
    }
}