namespace DnsWebApp.Models.Database
{
    using System.ComponentModel.DataAnnotations;

    public class ZoneGroupMember
    {
        public long Id { get; set; }

        [Required]
        public Zone Zone { get; set; }        
        public long ZoneId { get; set; }        
        
        public ZoneGroup ZoneGroup { get; set; }
        public long ZoneGroupId { get; set; }
    }
}