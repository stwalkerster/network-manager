namespace LdapDnsWebApp.Models.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Registrar
    {
        public long Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public List<Zone> Zones { get; set; }
        public List<RegistrarTldSupport> RegistrarTldSupports { get; set; }
    }
}