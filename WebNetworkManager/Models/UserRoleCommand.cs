namespace DnsWebApp.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class UserRoleCommand
    {
        // required for asp.net core
        public UserRoleCommand()
        {
        }
        
        public UserRoleCommand(ICollection<string> roles)
        {
            this.RoleAdministrator = roles.Contains(RoleDefinition.Administrator);
            this.RoleDnsManager = roles.Contains(RoleDefinition.DnsManager);
            this.RoleDnsUser = roles.Contains(RoleDefinition.DnsUser);
            this.RoleStaticData = roles.Contains(RoleDefinition.StaticData);
        }

        [Display(Name = RoleDefinition.StaticData)]    
        public bool RoleStaticData { get; set; }
        
        [Display(Name = RoleDefinition.Administrator)]
        public bool RoleAdministrator { get; set; }
        
        [Display(Name = RoleDefinition.DnsManager)]
        public bool RoleDnsManager { get; set; }
        
        [Display(Name = RoleDefinition.DnsUser)]
        public bool RoleDnsUser { get; set; }

        public IList<string> ToList()
        {
            var result = new List<string>();

            if (this.RoleAdministrator) { result.Add(RoleDefinition.Administrator); }
            if (this.RoleDnsManager) { result.Add(RoleDefinition.DnsManager); }
            if (this.RoleDnsUser) { result.Add(RoleDefinition.DnsUser); }
            if (this.RoleStaticData) { result.Add(RoleDefinition.StaticData); }

            return result;
        }
    }
}