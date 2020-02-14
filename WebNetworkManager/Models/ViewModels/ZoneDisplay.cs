namespace DnsWebApp.Models.ViewModels
{
    using DnsWebApp.Models.Database;

    public class ZoneDisplay : ZoneDisplayBase
    {
        public Zone Zone { get; set; }
        public string Fqdn { get; set; }

        public override uint? DefaultTTL => this.Zone.DefaultTimeToLive;
    }
}