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

        public int ClassMinWidth => 3;
        public int TypeMinWidth => 8;
        
        public string FormatValue(Record rr)
        {
            if (rr.Type != RecordType.TXT)
            {
                return rr.Value;
            }

            var escapedValue = rr.Value.Replace("\"", "\\\"");
            
            if (escapedValue.Length <= 200)
            {
                return "\"" + escapedValue + "\"";
            }

            var indent = "".PadLeft(this.NameMinWidth + this.TtlMinWidth + this.ClassMinWidth + this.TypeMinWidth);

            var data = "(\n";
            
            while (escapedValue.Length > 0)
            {
                 var sub = escapedValue.Substring(0, Math.Min(100, escapedValue.Length));
                 
                 // trim back to last space
                 var lastSpace = sub.LastIndexOf(' ');
                 if (lastSpace != -1)
                 {
                     sub = sub.Substring(0, lastSpace + 1);
                 }

                 // trim back to not split escapes
                 if (sub.EndsWith("\\"))
                 {
                     sub = sub.Substring(0, sub.Length - 1);
                 }
                 
                 escapedValue = escapedValue.Substring(sub.Length);

                 data += indent + "  \"" + sub + "\"\n";
            }

            data += indent + ")";
            
            return data;
        }
    }
}