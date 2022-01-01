using System;
using IdentityHttps.Areas.Identity.Data;
using IdentityHttps.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(IdentityHttps.Areas.Identity.IdentityHostingStartup))]
namespace IdentityHttps.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AppsDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DbConString")));

                services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<AppsDbContext>();
            });
        }
    }
}