namespace LdapDnsWebApp.Services
{
    using LdapDnsWebApp.Models;

    public interface IAuthenticationService
    {
        AppUser Login(string username, string password);
    }
}