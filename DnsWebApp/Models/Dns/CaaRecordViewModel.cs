namespace DnsWebApp.Models.Dns
{
    using DnsWebApp.Models.Database;

    public class CaaRecordViewModel : RecordViewModelBase
    {
        public CaaRecordViewModel(Record record) : base(record, RecordType.CAA)
        {
        }

        public string Flag
        {
            get => this.Parse()[0];
            set => this.Set(0, value);
        }

        public string Tag
        {
            get => this.Parse()[1];
            set => this.Set(1, value);
        }

        public override string Value
        {
            get => this.Parse()[2];
            set => this.Set(2, value);
        }
    }
}