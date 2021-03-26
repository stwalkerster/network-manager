namespace DnsWebApp.Models.Database
{
    using DnsWebApp.Models.Config;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;

    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options, IHostEnvironment env) : base(options)
        {
            if (env.IsProduction() || env.IsStaging())
            {
                this.Database.Migrate();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Zone>().HasIndex(x => new {x.TopLevelDomainId, x.Name, x.HorizonViewId}).IsUnique();
            
            modelBuilder.Entity<Registrar>().HasIndex(x => x.Name).IsUnique();
            
            modelBuilder.Entity<RegistrarTldSupport>()
                .HasIndex(x => new {x.RegistrarId, x.TopLevelDomainId})
                .IsUnique();
            
            modelBuilder.Entity<TopLevelDomain>().HasIndex(x => x.Domain).IsUnique();
          
            modelBuilder.Entity<Record>().Property(x => x.Class).HasConversion<string>();
            modelBuilder.Entity<Record>().Property(x => x.Type).HasConversion<string>();

            modelBuilder.Entity<Currency>().HasIndex(x => x.Code).IsUnique();

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }

        public DbSet<Zone> Zones { get; set; }
        public DbSet<ZoneGroup> ZoneGroups { get; set; }
        public DbSet<Registrar> Registrar { get; set; }
        public DbSet<Record> Record { get; set; }
        public DbSet<TopLevelDomain> TopLevelDomains { get; set; }
        public DbSet<RegistrarTldSupport> RegistrarTldSupport { get; set; }
        public DbSet<FavouriteDomains> FavouriteDomains { get; set; }
        public DbSet<HorizonView> HorizonViews { get; set; }
        
        public DbSet<Currency> Currencies { get; set; }
    }
}