namespace DnsWebApp.Models
{
    using System.Collections.Generic;
    using DnsWebApp.Models.Database;

    public class DeleteUserCommand
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string RealName { get; set; }
        public List<Zone> Zones { get; set; }
    }
}