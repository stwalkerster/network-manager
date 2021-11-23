namespace DnsWebApp.Models.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using DnsWebApp.Validation;

    public class RegistrarTldSupport
    {
        public long Id { get; set; }
        
        public Registrar Registrar { get; set; }
        [Required]
        [NotEqual(0)]
        public long RegistrarId { get; set; }
        
        [Display(Name = "Top level domain")]
        public TopLevelDomain TopLevelDomain { get; set; }
        
        [Required]
        [Display(Name = "Top level domain")]
        [NotEqual(0)]
        public long TopLevelDomainId { get; set; }
        
        [Display(Name = "Renewal price")]
        public decimal? RenewalPrice { get; set; }
        
        [Display(Name = "Transfer price")]
        public decimal? TransferPrice { get; set; }
        
        [Required]
        [Display(Name = "Transfer includes 1-year renewal?")]
        public bool TransferIncludesRenewal { get; set; }
        
        [Required]
        [Display(Name = "WHOIS Privacy Service included?")]
        public bool PrivacyIncluded { get; set; }
        
        [Display(Name = "Last price update")]
        public DateTime? RenewalPriceUpdated { get; set; }
        
        [Required]
        [Display(Name = "Allow inbound transfer?")]
        public bool AllowInboundTransfer { get; set; }
    }
}