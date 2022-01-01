using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityHttps.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IdentityHttps.Data
{
    public class AppsDbContext : IdentityDbContext<User>
    {
        public AppsDbContext(DbContextOptions<AppsDbContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                var conString = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                        .AddJsonFile("appsettings.json", true, true)
                                                        .Build()["ConnectionStrings:AppDbConnection"];
                builder.UseSqlServer(conString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var type in builder.Model.GetEntityTypes())
            {
                string name = type.GetTableName();
                if (name.StartsWith("AspNet"))
                {
                    type.SetTableName(name.Substring(6));
                }
            }
            
        }
    }
}
