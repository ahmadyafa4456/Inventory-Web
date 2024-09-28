using Inventory_Web.Models;
using Inventory_Web.SeedConfiguration;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Inventory_Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,IdentityRole<int>, int>
    {
        public DbSet<Category> Category { get; set; }
        public DbSet<Address> Address { get; set; }
        public DbSet<Sellers> Sellers { get; set; }
        public DbSet<AppUser> User { get; set; }
        public DbSet<Products> Product { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Order_history> Order_History { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("Server=localhost;Database=inventory;User=root;Password=4456", new MySqlServerVersion(new Version(8, 0, 21)));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<AppUser>(entity => entity.ToTable(name: "Users"));
            builder.Entity<IdentityRole<int>>(entity => entity.ToTable(name: "Roles"));
            builder.Entity<Sellers>(entity => entity.ToTable(name: "Sellers"));
            builder.Entity<Address>(entity => entity.ToTable(name: "Address"));
            builder.Entity<Sellers>()
            .HasOne(s => s.User)
            .WithMany(u => u.Sellers)
            .HasForeignKey(u => u.UserId);
            builder.Entity<Address>()
            .HasOne(a => a.Sellers)
            .WithOne(s => s.Address)
            .HasForeignKey<Address>(a => a.SellerId);
            builder.Entity<Category>(entity => entity.ToTable(name: "Category"));
            builder.Entity<Products>(entity => entity.ToTable(name: "Products"));
            builder.Entity<Orders>(entity => entity.ToTable(name: "Orders"));
            builder.Entity<Order_history>(entity => entity.ToTable(name: "Order_history"));
            builder.Entity<Products>().HasOne(p => p.Sellers).WithMany(s => s.Products).HasForeignKey(p => p.SellerId);
            builder.Entity<Products>().HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
            builder.Entity<Order_history>().HasOne(o => o.Sellers).WithMany(s => s.Order_history).HasForeignKey(o => o.SellerId);
            builder.Entity<Order_history>().HasOne(o => o.Orders).WithMany(c => c.Order_history).HasForeignKey(o => o.OrderId);
            builder.ApplyConfiguration(new RoleConfiguration());
        }
    }
}
