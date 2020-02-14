namespace DnsWebApp
{
    using DnsWebApp.Models.Database;
    using DnsWebApp.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<FormattingService>();
            services.AddScoped<WhoisService>();
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DataContext>();

            services.AddDbContext<DataContext>(
                options =>
                {
                    options.UseNpgsql(this.config.GetConnectionString("Postgres"));
                });
            
            services.ConfigureApplicationCookie(
                options =>
                {
                    options.LogoutPath = "/logout";
                    options.LoginPath = "/login";
                });

            services.AddControllersWithViews(
                o =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    o.Filters.Add(new AuthorizeFilter(policy));
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
                });

            app.UseStaticFiles();
        }
    }
}