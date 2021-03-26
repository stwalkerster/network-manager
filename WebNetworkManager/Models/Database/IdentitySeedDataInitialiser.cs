namespace DnsWebApp.Models.Database
{
    using System.Linq;
    using Microsoft.AspNetCore.Identity;

    public static class IdentitySeedDataInitialiser
    {
        public static void SeedData(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new IdentityUser
                {
                    UserName = "Administrator",
                    Email = "root@localhost",
                    EmailConfirmed = true
                };
                
                var result = userManager.CreateAsync(user, "Adm1nistrator!").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }
        }
    }
}