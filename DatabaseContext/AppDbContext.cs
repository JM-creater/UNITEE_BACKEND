using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Drawing;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.DatabaseContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<SizeQuantity> SizeQuantities { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(CoreEventId.NavigationBaseIncludeIgnored));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    DepartmentId = 1,
                    Department_Name = "Senior High School"
                },
                new Department
                {
                    DepartmentId = 2,
                    Department_Name = "Elementary and Junior High School"
                },
                new Department
                {
                    DepartmentId = 3,
                    Department_Name = "Criminology"
                },
                new Department
                {
                    DepartmentId = 4,
                    Department_Name = "Nursing"
                },
                new Department
                {
                    DepartmentId = 5,
                    Department_Name = "Allied Engineering"
                },
                new Department
                {
                    DepartmentId = 6,
                    Department_Name = "Customs Management"
                },
                new Department
                {
                    DepartmentId = 7,
                    Department_Name = "Computer Studies"
                },
                new Department
                {
                    DepartmentId = 8,
                    Department_Name = "Marine Transportation"
                },
                new Department
                {
                    DepartmentId = 9,
                    Department_Name = "Business and Accountancy"
                },
                new Department
                {
                    DepartmentId = 10,
                    Department_Name = "Teacher Education"
                },
                new Department
                {
                    DepartmentId = 11,
                    Department_Name = "Marine Engineering"
                },
                new Department
                {
                    DepartmentId = 12,
                    Department_Name = "Hotel and Tourism"
                }
                );

            modelBuilder.Entity<ProductType>().HasData(
                new ProductType
                {
                    ProductTypeId = 1,
                    Product_Type = "School Uniform"
                },
                new ProductType
                {
                    ProductTypeId = 2,
                    Product_Type = "Event T-shirt"
                },
                new ProductType
                {
                    ProductTypeId = 3,
                    Product_Type = "Department Shirt"
                },
                new ProductType
                {
                    ProductTypeId = 4,
                    Product_Type = "ID Sling"
                }
                );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 20163482,
                    FirstName = "Admin",
                    LastName = "Admin",
                    Password = "123456",
                    Email = "admin@gmail.com",
                    PhoneNumber = "639199431060",
                    Image = "Images/0d218025-7843-4cee-beed-0a62655a9664.png",
                    IsActive = true,
                    IsValidate = true,
                    Role = 3
                }
                );
        }
    }
}
