namespace DnsWebApp.Models.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using DnsWebApp.Models.Database;

    public class DnsRecordset
    {
        public DnsRecordset()
        {
            this.FromZone = new List<Record>();
            this.FromZoneGroup = new List<Record>();
        }

        public bool Any()
        {
            return this.FromZone.Any() || this.FromZoneGroup.Any();
        }
        
        public List<Record> FromZone { get; }
        public List<Record> FromZoneGroup { get; }
    }
}