namespace DnsWebApp.Models.Database
{
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
    }
}