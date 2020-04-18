namespace DnsWebApp.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordCommand
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
     
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}