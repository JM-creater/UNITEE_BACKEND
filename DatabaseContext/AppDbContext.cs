﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Security;

namespace UNITEE_BACKEND.DatabaseContext
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<SizeQuantity> SizeQuantities { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<ProductDepartment> ProductDepartments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();

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
                },
                new Department
                {
                    DepartmentId = 13,
                    Department_Name = "Basic Education"
                },
                new Department
                {
                    DepartmentId = 14,
                    Department_Name = "Seacast"
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
                    Product_Type = "Department T-Shirt"
                },
                new ProductType
                {
                    ProductTypeId = 4,
                    Product_Type = "PE Uniform"
                },
                new ProductType
                {
                    ProductTypeId = 5,
                    Product_Type = "ID Sling"
                },
                new ProductType
                {
                    ProductTypeId = 6,
                    Product_Type = "Accessories"
                }
                );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 20163482,
                    FirstName = "Joseph Martin",
                    LastName = "Garado",
                    Password = PasswordEncryptionService.EncryptPassword("123456"),
                    Email = "garadojosephmartin98@gmail.com",
                    PhoneNumber = "09199431060",
                    Image = "PathImages\\Images\\Admin Profile.png",
                    IsActive = true,
                    IsValidate = true,
                    Role = 3,
                    Address = "123 Main Street"
                }
                );


            modelBuilder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(u => u.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductDepartments)
                .WithOne(pd => pd.Product)
                .HasForeignKey(pd => pd.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Department>()
                .HasMany(d => d.ProductDepartments)
                .WithOne(pd => pd.Department)
                .HasForeignKey(pd => pd.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductDepartment>()
                .HasKey(pd => new { pd.ProductId, pd.DepartmentId });

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.SizeQuantity)
                .WithMany()
                .HasForeignKey(c => c.SizeQuantityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Order)
                .WithMany(o => o.Notifications)
                .HasForeignKey(n => n.OrderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.SizeQuantity)
                .WithMany() 
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany() 
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Supplier)
                .WithMany(r => r.Ratings)
                .HasForeignKey(r => r.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany(r => r.Ratings)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Rating)
                .WithMany()
                .HasForeignKey(u => u.RatingId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Product>()
                .HasOne(u => u.Rating)
                .WithMany()
                .HasForeignKey(u => u.RatingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
