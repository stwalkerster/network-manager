namespace DnsWebApp.Models.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TopLevelDomain
    {
        public long Id { get; set; }
        [Required]
        public string Domain { get; set; }

        [Display(Name = "TLD Whois Server")]
        public string WhoisServer { get; set; }
        
        [Display(Name = "Expiry Date Format")]
        public string WhoisExpiryDateFormat { get; set; }
        
        [Display(Name = "Registry")]
        public string Registry { get; set; }
        
        [Display(Name = "Registry URL")]
        public string RegistryUrl { get; set; }
        
        public List<Zone> Zones { get; set; }
        public List<RegistrarTldSupport> RegistrarTldSupports { get; set; }
    }
}