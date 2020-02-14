namespace DnsWebApp.Models.Database
{
    using System.ComponentModel.DataAnnotations;

    public class RegistrarTldSupport
    {
        public long Id { get; set; }
        [Required]
        public Registrar Registrar { get; set; }
        [Required]
        public TopLevelDomain TopLevelDomain { get; set; }
        public decimal? RenewalPrice { get; set; }
    }
}