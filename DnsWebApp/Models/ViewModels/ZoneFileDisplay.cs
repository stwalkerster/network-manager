namespace DnsWebApp.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public int NameMinWidth
        {
            get
            {
                var recordMax = 0;
                if (this.Records.Any())
                {
                    recordMax=this.Records.Max(x => (x.Name?.Length).GetValueOrDefault(1) + 1);
                }

                return Math.Max(8, recordMax);
            }
        }
        public int TtlMinWidth
        {
            get
            {
                var recordMax = 0;
                if (this.Records.Any())
                {
                    recordMax = this.Records.Max(x => x.TimeToLive.ToString().Length + 1);
                }

                return Math.Max(0, recordMax);
            }
        }
    }
}