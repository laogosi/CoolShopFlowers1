using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoolShopFlowers.Models
{
    public class DBContext : IdentityDbContext<User>
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Flower> Flowers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка для Order
            modelBuilder.Entity<Order>(o =>
            {
                o.HasOne(o => o.User)
                 .WithMany(u => u.Orders)
                 .HasForeignKey(o => o.UserId)
                 .OnDelete(DeleteBehavior.Restrict);  

                o.HasOne(o => o.OrderStatus)
                 .WithMany()
                 .HasForeignKey(o => o.OrderStatusId)
                 .OnDelete(DeleteBehavior.Restrict);  
            });
             
            modelBuilder.Entity<OrderDetail>(od =>
            {
                od.HasKey(od => new { od.OrderId, od.FlowerId });  

                od.HasOne(od => od.Order)
                 .WithMany(o => o.OrderDetails)
                 .HasForeignKey(od => od.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);

                od.HasOne(od => od.Flower)
                 .WithMany(f => f.OrderDetails)
                 .HasForeignKey(od => od.FlowerId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
             
            modelBuilder.Entity<Flower>(f =>
            {
                f.HasOne(f => f.Category)
                 .WithMany(c => c.Flowers)
                 .HasForeignKey(f => f.CategoryId)
                 .OnDelete(DeleteBehavior.SetNull);  
            });
             
            modelBuilder.Entity<ShoppingCart>(sc =>
            {
                sc.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(sc => sc.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public enum Roles
        {
            Admin,
            Moder,
            User
        }

        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<User>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Moder.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));

            var user = new User
            {
                UserName = "floweradmin",
                Email = "admin@gmail.com",
                BirthDay = DateTime.UtcNow,
                Name = "Marina",
                Sex = "Male",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                IsBanned = false,
                IsMuted = false,
            };

            var userInDb = await userManager.FindByEmailAsync(user.Email);
            if (userInDb == null)
            {
                await userManager.CreateAsync(user, "Adm100@@@");
                await userManager.AddToRoleAsync(user, Roles.Admin.ToString());
            }
        }
    }
}
