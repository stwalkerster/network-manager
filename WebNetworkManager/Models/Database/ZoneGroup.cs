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
        
        public List<Record> Records { get; set; }
        public List<ZoneGroupMember> ZoneGroupMembers { get; set; }
    }
}