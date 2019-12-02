namespace DnsWebApp.Models.Database
{
    public class RegistrarTldSupport
    {
        public long Id { get; set; }
        public Registrar Registrar { get; set; }
        public TopLevelDomain TopLevelDomain { get; set; }
        public decimal RenewalPrice { get; set; }
    }
}