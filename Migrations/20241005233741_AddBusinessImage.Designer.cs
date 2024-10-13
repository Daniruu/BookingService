﻿// <auto-generated />
using System;
using BookingService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BookingService.Migrations
{
    [DbContext(typeof(BookingServiceDbContext))]
    [Migration("20241005233741_AddBusinessImage")]
    partial class AddBusinessImage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BookingService.Models.Booking", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("CreatedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.Property<DateTimeOffset>("DateTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ServiceId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ServiceId");

                    b.HasIndex("UserId");

                    b.ToTable("Bookings");
                });

            modelBuilder.Entity("BookingService.Models.Business", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Krs")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Regon")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Businesses");
                });

            modelBuilder.Entity("BookingService.Models.BusinessImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BusinessId")
                        .HasColumnType("integer");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsPrimary")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.ToTable("BusinessImages");
                });

            modelBuilder.Entity("BookingService.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BusinessId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("BookingService.Models.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BusinessId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Duration")
                        .HasColumnType("integer");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsFeatured")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("BookingService.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AvatarUrl")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RefreshExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BookingService.Models.WorkingHours", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BusinessId")
                        .HasColumnType("integer");

                    b.Property<string>("DayOfWeek")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<TimeSpan>("End")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("Start")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("BusinessId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("WorkingHours");
                });

            modelBuilder.Entity("BookingService.Models.Booking", b =>
                {
                    b.HasOne("BookingService.Models.Service", "Service")
                        .WithMany()
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingService.Models.User", "User")
                        .WithMany("Bookings")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Service");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BookingService.Models.Business", b =>
                {
                    b.OwnsOne("BookingService.Models.Address", "Address", b1 =>
                        {
                            b1.Property<int>("BusinessId")
                                .HasColumnType("integer");

                            b1.Property<string>("BuildingNumber")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("PostalCode")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("Region")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<string>("RoomNumber")
                                .HasColumnType("text");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("BusinessId");

                            b1.ToTable("Businesses");

                            b1.WithOwner()
                                .HasForeignKey("BusinessId");
                        });

                    b.Navigation("Address")
                        .IsRequired();
                });

            modelBuilder.Entity("BookingService.Models.BusinessImage", b =>
                {
                    b.HasOne("BookingService.Models.Business", "Business")
                        .WithMany("Images")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Business");
                });

            modelBuilder.Entity("BookingService.Models.Employee", b =>
                {
                    b.HasOne("BookingService.Models.Business", "Business")
                        .WithMany("Employees")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Business");
                });

            modelBuilder.Entity("BookingService.Models.Service", b =>
                {
                    b.HasOne("BookingService.Models.Business", "Business")
                        .WithMany("Services")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookingService.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Business");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("BookingService.Models.WorkingHours", b =>
                {
                    b.HasOne("BookingService.Models.Business", "Business")
                        .WithMany("WorkingHours")
                        .HasForeignKey("BusinessId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BookingService.Models.Employee", "Employee")
                        .WithMany("WorkingHours")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Business");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("BookingService.Models.Business", b =>
                {
                    b.Navigation("Employees");

                    b.Navigation("Images");

                    b.Navigation("Services");

                    b.Navigation("WorkingHours");
                });

            modelBuilder.Entity("BookingService.Models.Employee", b =>
                {
                    b.Navigation("WorkingHours");
                });

            modelBuilder.Entity("BookingService.Models.User", b =>
                {
                    b.Navigation("Bookings");
                });
#pragma warning restore 612, 618
        }
    }
}
