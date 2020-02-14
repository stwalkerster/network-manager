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
        
        [Required]
        public override string Value
        {
            get => base.Value?.Trim('"');
            set => base.Value = $"\"{value}\"";
        }
    }
}