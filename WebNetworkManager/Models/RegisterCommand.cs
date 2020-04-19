namespace DnsWebApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterCommand : UserCommand
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name="Password")]
        public string Password { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords must match")]
        [Display(Name="Confirm password")]
        public string ConfirmPassword { get; set; }
    }
}