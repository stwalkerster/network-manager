namespace DnsWebApp.Models.Dns
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using DnsWebApp.Models.Database;

    public class SshfpRecordViewModel : RecordViewModelBase
    {
        public SshfpRecordViewModel(Record record) : base(record, RecordType.SSHFP)
        {
        }

        public SshfpRecordViewModel() : this(null)
        {
        }
        
        [Required]
        public SshfpAlgorithm Algorithm
        {
            get => (SshfpAlgorithm)int.Parse(string.IsNullOrEmpty(this.Get(0)) ? "0" : this.Get(0));
            set => this.Set(0, ((int)value).ToString(CultureInfo.InvariantCulture));
        }

        [Required]
        [Display(Name = "Fingerprint type")]
        public SshfpFingerprintType FingerprintType
        {
            get => (SshfpFingerprintType)int.Parse(string.IsNullOrEmpty(this.Get(1)) ? "0" : this.Get(1));
            set => this.Set(1, ((int)value).ToString(CultureInfo.InvariantCulture));
        }

        [Required]
        public string Fingerprint
        {
            get => this.Get(2);
            set => this.Set(2, value);
        }
    }
}