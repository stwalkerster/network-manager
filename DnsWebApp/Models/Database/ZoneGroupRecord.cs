namespace DnsWebApp.Models.Database
{
    using System.ComponentModel.DataAnnotations;

    public class ZoneGroupRecord
    {
        public long Id { get; set; }

        [Required]
        public ZoneGroup ZoneGroup { get; set; }
        
        [Required]
        public Record Record { get; set; }
    }
}