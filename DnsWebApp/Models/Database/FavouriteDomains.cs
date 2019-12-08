namespace DnsWebApp.Models.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Identity;

    public class FavouriteDomains
    {
        public long Id { get; set; }
        [Required]
        public IdentityUser User { get; set; }
        public string UserId { get; set; }
        [Required]
        public Zone Zone { get; set; }
        public long ZoneId { get; set; }
    }
}