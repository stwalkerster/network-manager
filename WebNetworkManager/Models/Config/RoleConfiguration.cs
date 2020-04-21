namespace DnsWebApp.Models.Config
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                // rights to manage access control stuff
                new IdentityRole{Name = "Administrator", NormalizedName = "ADMINISTRATOR"},
                
                // has rights to edit all zones
                new IdentityRole{Name="DNS Manager", NormalizedName = "DNS MANAGER"},
                
                // has rights to edit zones their own zones
                new IdentityRole{Name="DNS User", NormalizedName = "DNS USER"},
                
                // rights to edit static data eg currencies
                new IdentityRole{Name="Static Data", NormalizedName = "STATIC DATA"}
            );
        }
    }
}