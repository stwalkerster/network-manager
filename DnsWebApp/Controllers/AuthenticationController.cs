namespace DnsWebApp.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using DnsWebApp.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
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

            var result = await this.signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (!result.Succeeded)
            {
                this.ModelState.AddModelError("LoginStatus", "Login failed.");
                return this.View(model);
            }
            
            return this.Redirect("/");
        }

        [Route("/register")]
        [HttpGet]
        public IActionResult Register()
        {
            return this.View();
        }
        
        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            var newUser = new IdentityUser
            {
                UserName = command.Username
            };

            var result = await this.userManager.CreateAsync(newUser, command.Password);

            if (!result.Succeeded)
            {
                foreach (var errorMessage in result.Errors.Select(x => x.Description))
                {
                    this.ModelState.AddModelError("LoginStatus", errorMessage);
                }

                return this.View(command);
            }

            return this.RedirectToAction("Login");
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            return this.Redirect("/");
        }
    }
}