namespace DnsWebApp.Models.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class Zone
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required]
        public TopLevelDomain TopLevelDomain { get; set; }
        public long TopLevelDomainId { get; set; }
        
        [Required]
        [Display(Name = "Primary Name Server")]
        public string PrimaryNameServer { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Administrator { get; set; }
        
        public uint SerialNumber { get; set; }
        
        [Range(60,31536000)]
        public uint Refresh { get; set; }
        
        [Range(60,31536000)]
        public uint Retry { get; set; }
        
        [Range(60,31536000)]
        public uint Expire { get; set; }
        
        [Range(60,31536000)]
        [Display(Name = "Negative Time to Live")]
        public uint TimeToLive { get; set; }
        
        [Range(60,31536000)]
        [Display(Name = "Default Time to Live")]
        public uint DefaultTimeToLive { get; set; }
        
        [Required]
        public bool Enabled { get; set; }
        
        [Display(Name = "Registration Expiry")]
        public DateTime? RegistrationExpiry { get; set; }
        
        public Registrar Registrar { get; set; }
        public long? RegistrarId { get; set; }
        
        public IdentityUser Owner { get; set; }
        public string OwnerId { get; set; }
        
        public DateTime? WhoisLastUpdated { get; set; }
        
        [Required]
        public DateTime LastUpdated { get; set; }
        
        public HorizonView HorizonView { get; set; }
        public long? HorizonViewId { get; set; }
        
        public List<Record> Records { get; set; }
        public List<FavouriteDomains> FavouriteDomains { get; set; }
        public List<ZoneGroupMember> ZoneGroupMembers { get; set; }
    }
}