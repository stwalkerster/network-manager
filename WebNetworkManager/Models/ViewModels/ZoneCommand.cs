namespace DnsWebApp.Models.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Validation;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class ZoneCommand
    {
        public long Id { get; set; }

        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Domain")]
        public long Domain { get; set; }
        
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
        
        [Required]
        [Display(Name = "Top level domain")]
        [NotEqual(0)]
        public long TopLevelDomain { get; set; }

        public string Owner { get; set; }
        
        [Display(Name = "Split-horizon view")]
        public long? HorizonView { get; set; }

        public List<SelectListItem> HorizonViews { get; set; }
        public List<SelectListItem> Owners { get; set; }
        
    }
}