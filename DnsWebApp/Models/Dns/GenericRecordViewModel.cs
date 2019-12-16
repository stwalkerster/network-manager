namespace DnsWebApp.Models.Dns
{
    using DnsWebApp.Models.Database;

    public class GenericRecordViewModel : RecordViewModelBase
    {
        public GenericRecordViewModel(Record record) : base(record, record.Type)
        {
        }
    }
}