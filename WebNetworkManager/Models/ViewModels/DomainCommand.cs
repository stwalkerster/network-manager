namespace DnsWebApp.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Validation;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DomainCommand
    {
        public long Id { get; set; }

        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Domain base name")]
        public string BaseName { get; set; }
        
        [Required]
        [Display(Name = "Top level domain")]
        [NotEqual(0)]
        public long TopLevelDomain { get; set; }

        public string Owner { get; set; }
        
        public bool Placeholder { get; set; }

        [Display(Name = "Registration Expiry")]
        public DateTime? RegistrationExpiry { get; set; }

        public long? Registrar { get; set; }

        public List<SelectListItem> Registrars { get; set; }
        public List<SelectListItem> HorizonViews { get; set; }
        public List<SelectListItem> TopLevelDomains { get; set; }
        public List<SelectListItem> Owners { get; set; }
        
    }
}