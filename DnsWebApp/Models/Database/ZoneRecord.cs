namespace DnsWebApp.Models.Database
{
    using System.ComponentModel.DataAnnotations;

    public class ZoneRecord
    {
        public long Id { get; set; }

        [Required]
        public Zone Zone { get; set; }
        
        [Required]
        public Record Record { get; set; }
    }
}