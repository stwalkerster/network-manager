namespace DnsWebApp.Models.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class Domain
    {
        public long Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public TopLevelDomain TopLevelDomain { get; set; }
        public long TopLevelDomainId { get; set; }
        
        [Display(Name = "Registration Expiry")]
        public DateTime? RegistrationExpiry { get; set; }
        
        public IdentityUser Owner { get; set; }
        public string OwnerId { get; set; }
        
        public Registrar Registrar { get; set; }
        public long? RegistrarId { get; set; }
        
        public DateTime? WhoisLastUpdated { get; set; }
        
        [Required]
        public DateTime LastUpdated { get; set; }
        
        public bool Placeholder { get; set; }
        
        public List<Zone> Zones { get; set; }
    }
}