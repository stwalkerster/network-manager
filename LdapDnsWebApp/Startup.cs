using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LdapDnsWebApp
{
    using LdapDnsWebApp.Models;
    using LdapDnsWebApp.Services;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.Configuration;

    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IConfiguration config)
        {
            this.config = config;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LdapConnectionInfo>(this.config.GetSection("LdapConnectionInfo"));
            services.Configure<SshConnectionInfo>(this.config.GetSection("SshConnectionInfo"));
            
            services.AddSingleton<SshTunnelManager>();
            services.AddScoped<IAuthenticationService, LdapAuthenticationService>();
            services.AddScoped<ILdapManager, LdapManager>();
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
                options =>
                {
                    options.LoginPath = new PathString("/login");
                    options.LogoutPath = new PathString("/logout");
                });
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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