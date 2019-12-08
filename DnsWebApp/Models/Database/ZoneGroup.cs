namespace DnsWebApp.Models.Database
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class ZoneGroup
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        public List<ZoneGroupRecord> ZoneGroupRecords { get; set; }
        public List<ZoneGroupMember> ZoneGroupMembers { get; set; }
    }
}