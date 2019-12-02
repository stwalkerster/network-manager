namespace DnsWebApp.Services
{
    using DnsWebApp.Models;

    public interface IAuthenticationService
    {
        AppUser Login(string username, string password);
    }
}