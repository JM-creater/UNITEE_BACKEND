﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UNITEE_BACKEND.DatabaseContext;

#nullable disable

namespace UNITEE_BACKEND.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230923185454_AddedVirtualCart")]
    partial class AddedVirtualCart
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SupplierId");

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CartId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SizeQuantityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentId"));

                    b.Property<string>("Department_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("DepartmentId");

                    b.ToTable("Departments");

                    b.HasData(
                        new
                        {
                            DepartmentId = 1,
                            Department_Name = "Senior High School"
                        },
                        new
                        {
                            DepartmentId = 2,
                            Department_Name = "Elementary and Junior High School"
                        },
                        new
                        {
                            DepartmentId = 3,
                            Department_Name = "Criminology"
                        },
                        new
                        {
                            DepartmentId = 4,
                            Department_Name = "Nursing"
                        },
                        new
                        {
                            DepartmentId = 5,
                            Department_Name = "Allied Engineering"
                        },
                        new
                        {
                            DepartmentId = 6,
                            Department_Name = "Customs Management"
                        },
                        new
                        {
                            DepartmentId = 7,
                            Department_Name = "Computer Studies"
                        },
                        new
                        {
                            DepartmentId = 8,
                            Department_Name = "Marine Transportation"
                        },
                        new
                        {
                            DepartmentId = 9,
                            Department_Name = "Business and Accountancy"
                        },
                        new
                        {
                            DepartmentId = 10,
                            Department_Name = "Teacher Education"
                        },
                        new
                        {
                            DepartmentId = 11,
                            Department_Name = "Marine Engineering"
                        },
                        new
                        {
                            DepartmentId = 12,
                            Department_Name = "Hotel and Tourism"
                        });
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EstimateDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PaymentType")
                        .HasColumnType("int");

                    b.Property<string>("ProofOfPayment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReferenceId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<float>("Total")
                        .HasColumnType("real");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("ProductTypeId")
                        .HasColumnType("int");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("ProductTypeId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.ProductType", b =>
                {
                    b.Property<int>("ProductTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductTypeId"));

                    b.Property<string>("Product_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("ProductTypeId");

                    b.ToTable("ProductTypes");

                    b.HasData(
                        new
                        {
                            ProductTypeId = 1,
                            Product_Type = "School Uniform"
                        },
                        new
                        {
                            ProductTypeId = 2,
                            Product_Type = "Event T-shirt"
                        },
                        new
                        {
                            ProductTypeId = 3,
                            Product_Type = "Department Shirt"
                        },
                        new
                        {
                            ProductTypeId = 4,
                            Product_Type = "ID Sling"
                        });
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.SizeQuantity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<string>("Size")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("SizeQuantities");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("BIR")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("CityPermit")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsValidate")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("SchoolPermit")
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("ShopName")
                        .IsRequired()
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("StudyLoad")
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 20163482,
                            Address = "",
                            Email = "admin@gmail.com",
                            FirstName = "Admin",
                            Gender = "",
                            Image = "Images/0d218025-7843-4cee-beed-0a62655a9664.png",
                            IsActive = true,
                            IsValidate = true,
                            LastName = "Admin",
                            Password = "123456",
                            PhoneNumber = "639199431060",
                            Role = 3,
                            ShopName = ""
                        });
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Cart", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.User", "Supplier")
                        .WithMany("Carts")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.CartItem", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Cart", null)
                        .WithMany("Items")
                        .HasForeignKey("CartId");

                    b.HasOne("UNITEE_BACKEND.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Order", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Cart", "Cart")
                        .WithMany()
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Product", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.ProductType", "ProductType")
                        .WithMany()
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.SizeQuantity", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Product", "Product")
                        .WithMany("Sizes")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.User", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId");

                    b.Navigation("Department");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Cart", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Product", b =>
                {
                    b.Navigation("Sizes");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.User", b =>
                {
                    b.Navigation("Carts");
                });
#pragma warning restore 612, 618
        }
    }
}
