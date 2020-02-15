namespace DnsWebApp.Models.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class HorizonView
    {
        public long Id { get; set; }
        
        [Required]
        [Display(Name = "View name")]
        public string ViewName { get; set; }
        
        [Display(Name="Tag")]
        public string ViewTag { get; set; }
        
        [Display(Name = "Tag colour")]
        public string ViewTagColour { get; set; }
        
        public List<Zone> Zones { get; set; }
    }
}