namespace DnsWebApp.Models.Database
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;

    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options, IHostEnvironment env) : base(options)
        {
            if (env.IsProduction())
            {
                this.Database.Migrate();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //modelBuilder.Entity<Zone>().HasIndex(x => new {x.TopLevelDomain, x.Name}).IsUnique();
            modelBuilder.Entity<Registrar>().HasIndex(x => x.Name).IsUnique();
            
            modelBuilder.Entity<TopLevelDomain>().HasIndex(x => x.Domain).IsUnique();
            modelBuilder.Entity<TopLevelDomain>()
                .HasData(
                    new TopLevelDomain
                    {
                        Id = -1,
                        Domain = "com", 
                        Registry = "Verisign",
                        RegistryUrl = "https://www.verisign.com/en_US/domain-names/com-domain-names/index.xhtml",
                        WhoisServer = "whois.verisign-grs.com"
                    },
                    new TopLevelDomain
                    {
                        Id = -2,
                        Domain = "net", 
                        Registry = "Verisign",
                        RegistryUrl = "https://www.verisign.com/en_US/domain-names/net-domain-names/index.xhtml",
                        WhoisServer = "whois.verisign-grs.com"

                    },
                    new TopLevelDomain
                    {
                        Id = -3,
                        Domain = "org", 
                        Registry = "Public Interest Registry",
                        RegistryUrl = "https://thenew.org/org-people/",
                        WhoisServer = "whois.pir.org"
                    });
            
            modelBuilder.Entity<Record>().Property(x => x.Class).HasConversion<string>();
            modelBuilder.Entity<Record>().Property(x => x.Type).HasConversion<string>();
        }

        public DbSet<Zone> Zones { get; set; }
        public DbSet<ZoneGroup> ZoneGroups { get; set; }
        public DbSet<Registrar> Registrar { get; set; }
        public DbSet<Record> Record { get; set; }
        public DbSet<TopLevelDomain> TopLevelDomains { get; set; }
        public DbSet<RegistrarTldSupport> RegistrarTldSupport { get; set; }
        public DbSet<FavouriteDomains> FavouriteDomains { get; set; }
    }
}