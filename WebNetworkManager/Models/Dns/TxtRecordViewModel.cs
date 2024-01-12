namespace DnsWebApp.Models.Dns
{
    using System.ComponentModel.DataAnnotations;
    using DnsWebApp.Models.Database;

    public class TxtRecordViewModel : RecordViewModelBase
    {
        public TxtRecordViewModel(Record record) : base(record, RecordType.TXT)
        {
        }

        public TxtRecordViewModel() : this(null)
        {
        }
    }
}