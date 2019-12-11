namespace DnsWebApp.Models.Dns
{
    using DnsWebApp.Models.Database;

    public class TxtRecordViewModel : RecordViewModelBase
    {
        public TxtRecordViewModel(Record record) : base(record, RecordType.TXT)
        {
        }

        public override string Value
        {
            get => base.Value.Trim('"');
            set => base.Value = $"\"{value}\"";
        }
    }
}