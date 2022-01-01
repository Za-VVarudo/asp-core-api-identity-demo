using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityDemoAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityDemoAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { set; get; }
        public virtual DbSet<Category> Categories { set; get; }
        public virtual DbSet<BookCategory> BookCategories { set; get; }
        public virtual DbSet<Order> Orders { set; get; }
        public virtual DbSet<OrderDetail> OrderDetails { set; get; }
        public virtual DbSet<RefreshToken> RefreshTokens { set; get; }
        public virtual DbSet<BackgroundService> BackgroundServices { set; get; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<AppUserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();

            builder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            builder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.BookId });

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var name = entityType.GetTableName();
                if (name.StartsWith("AspNet")) entityType.SetTableName(name.Substring(6));
            }
        }
    }
}
