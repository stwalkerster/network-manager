namespace DnsWebApp.Models.Dns
{
    using DnsWebApp.Models.Database;

    public class NsRecordViewModel : RecordViewModelBase
    {
        public NsRecordViewModel(Record record) : base(record, RecordType.NS)
        {
        }

        public NsRecordViewModel() : base(null, RecordType.NS)
        {
        }

    }
}