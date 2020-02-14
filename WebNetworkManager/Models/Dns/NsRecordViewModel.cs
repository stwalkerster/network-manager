namespace DnsWebApp.Models.Dns
{
    using System.ComponentModel.DataAnnotations;
    using DnsWebApp.Models.Database;

    public class NsRecordViewModel : RecordViewModelBase
    {
        public NsRecordViewModel(Record record) : base(record, RecordType.NS)
        {
        }

        public NsRecordViewModel() : base(null, RecordType.NS)
        {
        }
        
        [Required]
        public string Nameserver { get => base.Value; set => base.Value = value; }
    }
}