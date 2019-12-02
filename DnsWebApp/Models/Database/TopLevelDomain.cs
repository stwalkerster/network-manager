namespace DnsWebApp.Models.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class TopLevelDomain
    {
        public long Id { get; set; }
        [Required]
        public string Domain { get; set; }
        
        public string WhoisServer { get; set; }
        
        public List<Zone> Zones { get; set; }
        public List<RegistrarTldSupport> RegistrarTldSupports { get; set; }
    }
}