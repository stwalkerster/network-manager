namespace DnsWebApp.Models.Dns
{
    using DnsWebApp.Models.Database;

    public class SrvRecordViewModel : RecordViewModelBase
    {
        public SrvRecordViewModel(Record record) : base(record, RecordType.SRV)
        {
            this.NameMaxParts = 3;
        }

        public string Protocol
        {
            get => this.Parse(true)[1];
            set => this.Set(1, value, true);
        }

        public string Service
        {
            get => this.Parse(true)[0];
            set => this.Set(0, value, true);
        }

        public override string Name
        {
            get => this.Parse(true)[2];
            set => this.Set(2, value, true);
        }

        public string Priority
        {
            get => this.Parse()[0];
            set => this.Set(0, value);
        }

        public string Weight
        {
            get => this.Parse()[1];
            set => this.Set(1, value);
        }

        public string Port
        {
            get => this.Parse()[2];
            set => this.Set(2, value);
        }

        public string Target
        {
            get => this.Parse()[3];
            set => this.Set(3, value);
        }
    }
}