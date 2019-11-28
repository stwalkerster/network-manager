namespace LdapDnsWebApp.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using LdapDnsWebApp.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using IAuthenticationService = LdapDnsWebApp.Services.IAuthenticationService;

    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService authService;

        public AuthenticationController(IAuthenticationService authService)
        {
            this.authService = authService;
        }

        [HttpGet]
        [Route("/login")]
        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        [Route("/login")]
        //[AllowAnonymous]
        public async Task<IActionResult> Login(LoginCommand model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = this.authService.Login(model.Username, model.Password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new Claim(ClaimTypes.Name, user.DisplayName),
                    new Claim(ClaimTypes.X500DistinguishedName, user.DistinguishedName),
                    new Claim("Password", user.Password)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var props = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false
                };

                await this.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    props);

                return this.Redirect("/");
            }

            this.ModelState.AddModelError("LoginStatus", "Login failed.");
            return this.View(model);
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return this.Redirect("/");
        }
        
    }
}