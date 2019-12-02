namespace DnsWebApp.Models.Database
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
           // this.Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //modelBuilder.Entity<Zone>().HasIndex(x => new {x.TopLevelDomain, x.Name}).IsUnique();
            modelBuilder.Entity<Registrar>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<TopLevelDomain>().HasIndex(x => x.Domain).IsUnique();
            
            modelBuilder.Entity<ZoneRecord>().Property(x => x.Class).HasConversion<string>();
            modelBuilder.Entity<ZoneRecord>().Property(x => x.Type).HasConversion<string>();
        }

        public DbSet<Zone> Zones { get; set; }
        public DbSet<Registrar> Registrar { get; set; }
        public DbSet<ZoneRecord> ZoneRecord { get; set; }
        public DbSet<TopLevelDomain> TopLevelDomains { get; set; }
        public DbSet<RegistrarTldSupport> RegistrarTldSupport { get; set; }
    }
}