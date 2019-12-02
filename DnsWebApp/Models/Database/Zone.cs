namespace LdapDnsWebApp.Models.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Zone
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public TopLevelDomain TopLevelDomain { get; set; }
        [Required]
        public string PrimaryNameServer { get; set; }
        [Required]
        public string Administrator { get; set; }
        public int SerialNumber { get; set; }
        public int Refresh { get; set; }
        public int Retry { get; set; }
        public int Expire { get; set; }
        
        public int TimeToLive { get; set; }
        
        [Required]
        public bool Enabled { get; set; }
        
        public DateTime? RegistrationExpiry { get; set; }
        
        public Registrar Registrar { get; set; }
        
        public List<ZoneRecord> ZoneRecords { get; set; }
    }
}