namespace DnsWebApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ZoneGroupCommand
    {
        [Required]
        public string Name { get; set; }
        
        [Display(Name = "Zones in group")]
        public string NewZoneGroupMembers { get; set; }
        public dynamic ZoneGroupMembers { get; set; }
        
        public dynamic AllZones { get; set; }
    }
}