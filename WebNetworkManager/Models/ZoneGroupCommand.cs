namespace DnsWebApp.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ZoneGroupCommand
    {
        public long Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Display(Name = "Zones in group")]
        public string NewZoneGroupMembers { get; set; }
        
        public string Owner { get; set; }
        public dynamic ZoneGroupMembers { get; set; }
        
        public dynamic AllZones { get; set; }
        public List<SelectListItem> Owners { get; set; }
    }
}