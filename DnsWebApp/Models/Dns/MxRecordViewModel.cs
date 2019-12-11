namespace DnsWebApp.Models.Dns
{
    using DnsWebApp.Models.Database;

    public class MxRecordViewModel : RecordViewModelBase
    {
        public MxRecordViewModel(Record record) : base(record, RecordType.MX)
        {
        }

        public string Preference
        {
            get => this.Parse()[0];
            set => this.Set(0, value);
        }

        public string MailServer
        {
            get => this.Parse()[1];
            set => this.Set(1, value);
        }
    }
}