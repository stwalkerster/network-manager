namespace DnsWebApp.Models.Database
{
    using System;
    using Microsoft.AspNetCore.Identity;

    public class FavouriteDomains
    {
        public long Id { get; set; }
        public IdentityUser User { get; set; }
        public string UserId { get; set; }
        public Zone Zone { get; set; }
    }
}