namespace DnsWebApp.Models.Dns
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class SrvRecordViewModel : RecordViewModelBase
    {
        private readonly List<string> protocolList;

        public SrvRecordViewModel(Record record) : base(record, RecordType.SRV)
        {
            this.protocolList = new List<string>
            {
                "_tcp", "_udp", "_xmpp", "_tls", "_smtp"
            };
            this.NameMaxParts = 3;
        }

        public SrvRecordViewModel() : this(null)
        {
        }

        public IEnumerable<SelectListItem> Protocols => this.protocolList.Select(x => new SelectListItem(x, x));

        [Required]
        public string Protocol
        {
            get => this.Get(1, true);
            set => this.Set(1, value, true);
        }

        [Required]
        public string Service
        {
            get => this.Get(0, true);
            set => this.Set(0, value, true);
        }

        public override string Name
        {
            get => this.Get(2, true);
            set => this.Set(2, value, true);
        }

        public ushort Priority
        {
            get => ushort.Parse(string.IsNullOrEmpty(this.Get(0)) ? "10" : this.Get(0));
            set => this.Set(0, value.ToString(CultureInfo.InvariantCulture));
        }

        public ushort Weight
        {
            get => ushort.Parse(string.IsNullOrEmpty(this.Get(1)) ? "5" : this.Get(1));
            set => this.Set(1, value.ToString(CultureInfo.InvariantCulture));
        }

        public ushort Port
        {
            get => ushort.Parse(string.IsNullOrEmpty(this.Get(2)) ? "80" : this.Get(2));
            set => this.Set(2, value.ToString(CultureInfo.InvariantCulture));
        }

        [Required]
        public string Target
        {
            get => this.Get(3);
            set => this.Set(3, value);
        }
    }
}