namespace DnsWebApp.Controllers
{
    using System.Threading.Tasks;
    using DnsWebApp.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AuthenticationController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("/login")]
        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        [Route("/login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCommand model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var result = await this.signInManager.PasswordSignInAsync(model.Username, model.Password, false, true);

            if (!result.Succeeded)
            {
                this.ModelState.AddModelError("LoginStatus", "Login failed.");
                return this.View(model);
            }

            return this.Redirect("/");
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return this.Redirect("/");
        }

        [Route("/changepw")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return this.View();
        }

        [Route("/changepw")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            var identityUser = await this.userManager.GetUserAsync(this.HttpContext.User);

            var result = await this.userManager.ChangePasswordAsync(
                identityUser,
                command.OldPassword,
                command.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.View();
            }

            return this.Redirect("/");
        }
    }
}