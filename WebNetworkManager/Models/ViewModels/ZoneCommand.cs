namespace DnsWebApp.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ZoneCommand
    {
        public long Id { get; set; }

        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Domain base name")]
        public string BaseName { get; set; }
        
        [Required]
        [Display(Name = "Primary Name Server")]
        public string PrimaryNameServer { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Administrator { get; set; }
        
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

        public long? Registrar { get; set; }
        
        [Required]
        [Display(Name = "Top level domain")]
        public long TopLevelDomain { get; set; }

        public string Owner { get; set; }

        public List<SelectListItem> Registrars { get; set; }
        public List<SelectListItem> TopLevelDomains { get; set; }
        public List<SelectListItem> Owners { get; set; }
        
    }
}