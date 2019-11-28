namespace LdapDnsWebApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginCommand
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Username")]
        public string Username { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}