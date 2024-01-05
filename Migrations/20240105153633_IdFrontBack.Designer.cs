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
    [Migration("20240105153633_IdFrontBack")]
    partial class IdFrontBack
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

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

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOrdered")
                        .HasColumnType("bit");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SizeQuantityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SizeQuantityId");

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
                        .HasColumnType("nvarchar(max)");

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

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("UserRole")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CancellationReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CartId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("OrderNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SizeQuantityId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SizeQuantityId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("BackViewImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FrontViewImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<float>("Price")
                        .HasColumnType("real");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductTypeId")
                        .HasColumnType("int");

                    b.Property<int?>("RatingId")
                        .HasColumnType("int");

                    b.Property<string>("SideViewImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SizeGuide")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("ProductTypeId");

                    b.HasIndex("RatingId");

                    b.HasIndex("SupplierId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.ProductDepartment", b =>
                {
                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.HasKey("ProductId", "DepartmentId");

                    b.HasIndex("DepartmentId");

                    b.ToTable("ProductDepartments");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.ProductType", b =>
                {
                    b.Property<int>("ProductTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductTypeId"));

                    b.Property<string>("Product_Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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
                            Product_Type = "PE Uniform"
                        },
                        new
                        {
                            ProductTypeId = 5,
                            Product_Type = "ID Sling"
                        });
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Rating", b =>
                {
                    b.Property<int>("RatingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RatingId"));

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<int>("SupplierId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("Value")
                        .HasColumnType("int");

                    b.HasKey("RatingId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SupplierId");

                    b.HasIndex("UserId");

                    b.ToTable("Ratings");
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
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BIR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BarangayClearance")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CityPermit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConfirmationCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int?>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailConfirmationToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsEmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("IsValidate")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RatingId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime2");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("SchoolPermit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ShopName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StudyLoad")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ValidIdBackImage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ValidIdFrontImage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("RatingId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 20163482,
                            Address = "123 Main Street",
                            DateCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DateUpdated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            Email = "garadojosephmartin98@gmail.com",
                            FirstName = "Admin",
                            Image = "PathImages\\Images\\Admin Profile.png",
                            IsActive = true,
                            IsEmailConfirmed = false,
                            IsValidate = true,
                            LastName = "Admin",
                            Password = "jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=",
                            PhoneNumber = "09199431060",
                            Role = 3
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
                    b.HasOne("UNITEE_BACKEND.Entities.Cart", "Cart")
                        .WithMany("Items")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.SizeQuantity", "SizeQuantity")
                        .WithMany()
                        .HasForeignKey("SizeQuantityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Product");

                    b.Navigation("SizeQuantity");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Notification", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Order", "Order")
                        .WithMany("Notifications")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Order", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Cart", "Cart")
                        .WithMany()
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.OrderItem", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.SizeQuantity", "SizeQuantity")
                        .WithMany()
                        .HasForeignKey("SizeQuantityId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");

                    b.Navigation("SizeQuantity");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Product", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.ProductType", "ProductType")
                        .WithMany()
                        .HasForeignKey("ProductTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.Rating", "Rating")
                        .WithMany()
                        .HasForeignKey("RatingId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("UNITEE_BACKEND.Entities.User", "Supplier")
                        .WithMany("Products")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("ProductType");

                    b.Navigation("Rating");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.ProductDepartment", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Department", "Department")
                        .WithMany("ProductDepartments")
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.Product", "Product")
                        .WithMany("ProductDepartments")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Rating", b =>
                {
                    b.HasOne("UNITEE_BACKEND.Entities.Product", "Product")
                        .WithMany("Ratings")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.User", "Supplier")
                        .WithMany("Ratings")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("UNITEE_BACKEND.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Supplier");

                    b.Navigation("User");
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

                    b.HasOne("UNITEE_BACKEND.Entities.Rating", "Rating")
                        .WithMany()
                        .HasForeignKey("RatingId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Department");

                    b.Navigation("Rating");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Cart", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Department", b =>
                {
                    b.Navigation("ProductDepartments");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Order", b =>
                {
                    b.Navigation("Notifications");

                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.Product", b =>
                {
                    b.Navigation("ProductDepartments");

                    b.Navigation("Ratings");

                    b.Navigation("Sizes");
                });

            modelBuilder.Entity("UNITEE_BACKEND.Entities.User", b =>
                {
                    b.Navigation("Carts");

                    b.Navigation("Products");

                    b.Navigation("Ratings");
                });
#pragma warning restore 612, 618
        }
    }
}
