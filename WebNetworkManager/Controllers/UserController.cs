namespace DnsWebApp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using DnsWebApp.Models;
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : Controller
    {
        private const string ClaimRealName = "RealName";

        private readonly DataContext db;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IEmailSender emailSender;

        public UserController(DataContext db, UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            this.db = db;
            this.userManager = userManager;
            this.emailSender = emailSender;
        }

        [Route("/users")]
        public async Task<IActionResult> Index()
        {
            var identityUsers = this.db.Users.ToList();

            List<IList<Claim>> claims = new List<IList<Claim>>();
            foreach (var user in identityUsers)
            {
                claims.Add(await this.userManager.GetClaimsAsync(user));
            }

            var dataset = new List<dynamic>();
            for (var i = 0; i < identityUsers.Count; i++)
            {
                dynamic user = new ExpandoObject();

                user.IdentityUser = identityUsers[i];
                user.Id = identityUsers[i].Id;
                user.UserName = identityUsers[i].UserName;
                user.Email = identityUsers[i].Email;
                user.EmailConfirmed = identityUsers[i].EmailConfirmed;
                user.LockoutEnd = identityUsers[i].LockoutEnd;
                user.LockoutEnabled = identityUsers[i].LockoutEnabled;
                user.RealName = claims[i].FirstOrDefault(x => x.Type == ClaimRealName)?.Value;

                dataset.Add(user);
            }

            return this.View(dataset);
        }

        [Route("/users/edit/{item}")]
        [HttpGet]
        public async Task<IActionResult> EditUser(string item)
        {
            var command = new UserCommand();
            var identityUser = await this.userManager.FindByIdAsync(item);

            if (identityUser == null)
            {
                return this.RedirectToAction("Index");
            }

            command.Id = identityUser.Id;
            command.Email = identityUser.Email;
            command.Username = identityUser.UserName;
            command.LockoutEnabled = identityUser.LockoutEnabled;
            command.LockedOut = identityUser.LockoutEnd.HasValue && identityUser.LockoutEnd.Value > DateTimeOffset.Now;

            var claims = await this.userManager.GetClaimsAsync(identityUser);
            command.RealName = claims.FirstOrDefault(x => x.Type == ClaimRealName)?.Value;

            return this.View(command);
        }

        [Route("/users/edit/{item}")]
        [HttpPost]
        public async Task<IActionResult> EditUser(string item, UserCommand command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            var identityUser = await this.userManager.FindByIdAsync(item);

            if (identityUser == null)
            {
                return this.RedirectToAction("Index");
            }

            var overallSuccess = true;

            if (identityUser.LockoutEnabled != command.LockoutEnabled)
            {
                if (!command.LockoutEnabled)
                {
                    await this.userManager.SetLockoutEndDateAsync(identityUser, null);
                    command.LockedOut = false;
                    identityUser.LockoutEnd = null;
                }
                
                var lockoutResult = await this.userManager.SetLockoutEnabledAsync(identityUser, command.LockoutEnabled);
                if (!lockoutResult.Succeeded)
                {
                    overallSuccess = false;
                    foreach (var errorMessage in lockoutResult.Errors.Select(x => x.Description))
                    {
                        this.ModelState.AddModelError("LockoutEnabled", errorMessage);
                    }
                }
            }
            
            if ((identityUser.LockoutEnd.HasValue && identityUser.LockoutEnd.Value > DateTimeOffset.Now) != (command.LockedOut && command.LockoutEnabled))
            {
                var lockoutEnd = command.LockedOut && command.LockoutEnabled ? DateTime.MaxValue : (DateTimeOffset?)null;
                
                var lockoutResult = await this.userManager.SetLockoutEndDateAsync(identityUser, lockoutEnd);
                if (!lockoutResult.Succeeded)
                {
                    overallSuccess = false;
                    foreach (var errorMessage in lockoutResult.Errors.Select(x => x.Description))
                    {
                        this.ModelState.AddModelError(nameof(command.LockedOut), errorMessage);
                    }
                }
            }
            
            if (identityUser.Email != command.Email)
            {
                var resultEmail = await this.userManager.SetEmailAsync(identityUser, command.Email);
                if (!resultEmail.Succeeded)
                {
                    overallSuccess = false;
                    foreach (var errorMessage in resultEmail.Errors.Select(x => x.Description))
                    {
                        this.ModelState.AddModelError("Email", errorMessage);
                    }
                }
            }

            if (identityUser.UserName != command.Username)
            {
                var resultUsername = await this.userManager.SetUserNameAsync(identityUser, command.Username);
                if (!resultUsername.Succeeded)
                {
                    overallSuccess = false;
                    foreach (var errorMessage in resultUsername.Errors.Select(x => x.Description))
                    {
                        this.ModelState.AddModelError("Username", errorMessage);
                    }
                }
            }

            var claims = (await this.userManager.GetClaimsAsync(identityUser))
                .Where(x => x.Type == ClaimRealName)
                .ToList();

            if (claims.Count == 0
                || claims.Count > 1
                || claims.FirstOrDefault(x => x.Type == ClaimRealName)?.Value != command.RealName)
            {
                var resultRemoveRealname = await this.userManager.RemoveClaimsAsync(identityUser, claims);
                if (!resultRemoveRealname.Succeeded)
                {
                    overallSuccess = false;
                    foreach (var errorMessage in resultRemoveRealname.Errors.Select(x => x.Description))
                    {
                        this.ModelState.AddModelError("RealName", errorMessage);
                    }
                }
                else
                {
                    var resultRealName = await this.userManager.AddClaimAsync(
                        identityUser,
                        new Claim(ClaimRealName, command.RealName));
                    if (!resultRealName.Succeeded)
                    {
                        overallSuccess = false;
                        foreach (var errorMessage in resultRealName.Errors.Select(x => x.Description))
                        {
                            this.ModelState.AddModelError("RealName", errorMessage);
                        }
                    }
                }
            }

            if (overallSuccess)
            {
                return this.RedirectToAction("Index");
            }

            return this.View(command);
        }

        [Route("/users/new")]
        [HttpGet]
        public IActionResult NewUser()
        {
            return this.View(new RegisterCommand
            {
                LockoutEnabled = true
            });
        }

        [Route("/users/new")]
        [HttpPost]
        public async Task<IActionResult> NewUser(RegisterCommand command)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(command);
            }

            var newUser = new IdentityUser
            {
                UserName = command.Username,
                Email = command.Email,
                LockoutEnabled = command.LockoutEnabled,
                LockoutEnd = command.LockedOut && command.LockoutEnabled ? DateTimeOffset.MaxValue : (DateTimeOffset?) null
            };

            var result = await this.userManager.CreateAsync(newUser, command.Password);

            if (!result.Succeeded)
            {
                foreach (var errorMessage in result.Errors.Select(x => x.Description))
                {
                    this.ModelState.AddModelError(string.Empty, errorMessage);
                }

                return this.View(command);
            }

            await this.userManager.SetLockoutEnabledAsync(newUser, command.LockoutEnabled);
            
            result = await this.userManager.AddClaimAsync(newUser, new Claim(ClaimRealName, command.RealName));
            
            if (!result.Succeeded)
            {
                foreach (var errorMessage in result.Errors.Select(x => x.Description))
                {
                    this.ModelState.AddModelError(nameof(command.RealName), errorMessage);
                }

                return this.View(command);
            }
            

            return this.RedirectToAction("Index");
        }

        
        [Route("/users/delete/{item}")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(string item)
        {
            var command = new DeleteUserCommand();
            var identityUser = await this.userManager.FindByIdAsync(item);

            if (identityUser == null)
            {
                return this.RedirectToAction("Index");
            }

            command.Id = identityUser.Id;
            command.RealName = identityUser.Email;
            
            var claims = await this.userManager.GetClaimsAsync(identityUser);
            command.RealName = claims.FirstOrDefault(x => x.Type == ClaimRealName)?.Value;

            command.Zones = this.db.Zones.Where(x => x.Owner == identityUser).ToList();
            
            return this.View(command);
        }

        [Route("/users/delete/{item}")]
        [HttpPost] 
        public async Task<IActionResult> DeleteUser(string item, DeleteUserCommand command)
        {
            var identityUser = await this.userManager.FindByIdAsync(item);

            if (identityUser == null)
            {
                return this.RedirectToAction("Index");
            }

            if (this.db.Zones.Any(x => x.Owner == identityUser))
            {
                return this.RedirectToAction("Index");
            }

            await this.userManager.DeleteAsync(identityUser);
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> SendEmailConfirmation(string item)
        {
            var user = await this.userManager.FindByIdAsync(item);
            var token = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = this.Url.Action(
                nameof(this.ConfirmEmail),
                "User",
                new {token, id = user.Id},
                this.Request.Scheme);

            this.emailSender.Send(new EmailMessage(new[] {user.Email}, "Email address confirmation", confirmationLink));

            return this.RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/confirmemail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string id)
        {
            var user = await this.userManager.FindByIdAsync(id);

            if (user == null)
            {
                return this.RedirectToAction("Index", "Home");
            }

            var result = await this.userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return this.View();
            }

            return this.View("EmailConfirmationFailed");
        }
    }
}