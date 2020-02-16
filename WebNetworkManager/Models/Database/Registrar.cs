namespace DnsWebApp.Models.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Registrar
    {
        public long Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Prices include VAT?")]
        public bool PricesIncludeVat { get; set; }
        
        [Required]
        [Display(Name = "Allow renewals?")]
        public bool AllowRenewals { get; set; }
        
        [Required]
        [Display(Name = "Allow transfers?")]
        public bool AllowTransfers { get; set; }
        
        [Display(Name = "Outbound transfer fee")]
        public decimal? TransferOutFee { get; set; }
        
        [Display(Name = "WHOIS Privacy Fee")]
        public decimal? PrivacyFee { get; set; }
            
        
        public Currency Currency { get; set; }
        [Display(Name="Currency")]
        public long? CurrencyId { get; set; }

        public List<Zone> Zones { get; set; }
        public List<RegistrarTldSupport> RegistrarTldSupports { get; set; }
    }
}