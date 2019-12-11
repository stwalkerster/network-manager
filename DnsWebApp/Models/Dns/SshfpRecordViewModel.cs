namespace DnsWebApp.Models.Dns
{
    using System.Globalization;
    using DnsWebApp.Models.Database;

    public class SshfpRecordViewModel : RecordViewModelBase
    {
        public SshfpRecordViewModel(Record record) : base(record, RecordType.SSHFP)
        {
        }

        public SshfpAlgorithm Algorithm
        {
            get => (SshfpAlgorithm)int.Parse(this.Parse()[0]);
            set => this.Set(0, ((int)value).ToString(CultureInfo.InvariantCulture));
        }

        public SshfpFingerprintType Type
        {
            get => (SshfpFingerprintType)int.Parse(this.Parse()[1]);
            set => this.Set(1, ((int)value).ToString(CultureInfo.InvariantCulture));
        }

        public string Fingerprint
        {
            get => this.Parse()[2];
            set => this.Set(2, value);
        }
    }
}