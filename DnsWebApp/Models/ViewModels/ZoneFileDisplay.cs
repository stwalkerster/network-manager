namespace DnsWebApp.Models.ViewModels
{
    using System.Collections.Generic;
    using DnsWebApp.Models.Database;

    public class ZoneFileDisplay
    {
        public ZoneFileDisplay()
        {
            this.Records = new List<Record>();
        }
        
        public Zone Zone { get; set; }
        public string Fqdn { get; set; }
        
        public List<Record> Records { get; }
    }
}