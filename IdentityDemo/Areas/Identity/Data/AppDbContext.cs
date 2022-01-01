using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemo.Areas.Identity.Data;
using IdentityDemo.Areas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IdentityDemo.Areas.Identity.Data
{
    public class AppDbContext : IdentityDbContext<AppUsers>
    {
        public AppDbContext() { 

        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            
        }
        public virtual DbSet<OrderObject> Orders { get; set; }
        public virtual DbSet<OrderDetailObject> OrderDetails { get; set; }
        public virtual DbSet<ProductObject> Products { get; set; }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderDetailObject>()
                        .HasKey(table => new { table.OrderId, table.ProductId });
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                string tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }
    }
}
