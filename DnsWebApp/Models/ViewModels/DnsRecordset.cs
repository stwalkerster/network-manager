namespace DnsWebApp.Models.ViewModels
{
    using System.Collections.Generic;
    using DnsWebApp.Models.Database;

    public class DnsRecordset
    {
        public DnsRecordset()
        {
            this.FromZone = new List<Record>();
            this.FromZoneGroup = new List<Record>();
        }
        
        public List<Record> FromZone { get; }
        public List<Record> FromZoneGroup { get; }
    }
}