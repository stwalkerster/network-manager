namespace DnsWebApp.Models.Dns
{
    using DnsWebApp.Models.Database;

    public class HostRecordViewModel : RecordViewModelBase
    {
        public HostRecordViewModel() : base(null, RecordType.A)
        {
        } 
        
        public HostRecordViewModel(Record record) : base(record, record.Type)
        {
        }
    }
}