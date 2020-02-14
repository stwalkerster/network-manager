namespace DnsWebApp.Models.Dns
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using DnsWebApp.Models.Database;

    public class CaaRecordViewModel : RecordViewModelBase
    {
        public CaaRecordViewModel(Record record) : base(record, RecordType.CAA)
        {
        }
        public CaaRecordViewModel() : this(null)
        {
        }

        public byte Flag
        {
            get => byte.Parse(string.IsNullOrEmpty(this.Get(0)) ? "0" : this.Get(0));
            set => this.Set(0, value.ToString(CultureInfo.InvariantCulture));
        }

        [Display(Name = "Issuer Critical")]
        public bool IssuerCritical
        {
            get => (this.Flag & 0b1000_0000) == 0b1000_0000;
            set
            {
                if (value)
                {
                    this.Flag |= 1 << 7;
                }
                else
                {
                    this.Flag &= unchecked((byte) ~(1 << 7));
                }
            }
        }

        [Required]
        public CaaTag Tag
        {
            get => string.IsNullOrWhiteSpace(this.Get(1))
                ? CaaTag.issue
                : (CaaTag) Enum.Parse(typeof(CaaTag), this.Get(1));
            set => this.Set(1, value.ToString());
        }

        [Required]
        public override string Value
        {
            get => this.Get(2);
            set => this.Set(2, value);
        }
    }
}