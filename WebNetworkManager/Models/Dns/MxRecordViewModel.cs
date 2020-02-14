namespace DnsWebApp.Models.Dns
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using DnsWebApp.Models.Database;

    public class MxRecordViewModel : RecordViewModelBase
    {
        public MxRecordViewModel(Record record) : base(record, RecordType.MX)
        {
        }

        public MxRecordViewModel() : this(null)
        {
        }

        [Required]
        public ushort Preference
        {
            get => ushort.Parse(string.IsNullOrEmpty(this.Get(0)) ? "10" : this.Get(0));
            set => this.Set(0, value.ToString(CultureInfo.InvariantCulture));
        }

        [Required]
        [Display(Name = "Mail server")]
        public string MailServer
        {
            get => this.Get(1);
            set => this.Set(1, value);
        }
    }
}