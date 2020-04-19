namespace DnsWebApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class UserCommand
    {
        public string Id { get; set; }
        
        [Required]
        [Display(Name="Username")]
        public string Username { get; set; }
        
        [Required]
        [Display(Name = "Name")]
        public string RealName { get; set; }
        
        [Display(Name = "E-mail Address")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
        
        [Display(Name = "Enable Account Lockout")]
        public bool LockoutEnabled { get; set; }
    }
}