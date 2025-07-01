using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var readerId = "f3a25e2f-6095-464c-8f29-18c44fa7f658";
            var writerID = "dccc0edf-c90f-4132-a314-1dbd45676a48";

            var roles = new List<IdentityRole>
            {
                new IdentityRole 
                {
                    Id = readerId,
                    ConcurrencyStamp = readerId,
                    Name = "Admin", 
                    NormalizedName = "ADMIN" 
                },
                new IdentityRole 
                {
                    Id = writerID,
                    ConcurrencyStamp = writerID,
                    Name = "User",
                    NormalizedName = "USER" 
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
    
}
