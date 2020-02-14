namespace DnsWebApp.Models.Dns
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using DnsWebApp.Models.Database;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class HostRecordViewModel : RecordViewModelBase
    {
        public HostRecordViewModel() : base(null, RecordType.A)
        {
        } 
        
        public HostRecordViewModel(Record record) : base(record, record.Type)
        {
        }

        public List<SelectListItem> AvailableTypes => new[] {RecordType.A, RecordType.AAAA, RecordType.CNAME}
            .Select(x => new SelectListItem(x.ToString(), ((int) x).ToString(CultureInfo.InvariantCulture)))
            .ToList();
        
        [Required]
        public override string Value { get => base.Value; set => base.Value = value; }
    }
}