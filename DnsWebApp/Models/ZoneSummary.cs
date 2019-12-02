namespace DnsWebApp.Models
{
    using System;

    public class ZoneSummary
    {
        public string ZoneName { get; set; }
        public string Registrar { get; set; }
        public string Owner { get; set; }
        public string NameServer { get; set; }
        public DateTime? Expiry { get; set; }
        public int Records { get; set; }
        public bool Enabled { get; set; }
    }
}