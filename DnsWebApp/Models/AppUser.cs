namespace DnsWebApp.Models
{
    public class AppUser
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string DistinguishedName { get; set; }
        public bool IsAdmin { get; set; }
        public string Password { get; set; }
    }
}