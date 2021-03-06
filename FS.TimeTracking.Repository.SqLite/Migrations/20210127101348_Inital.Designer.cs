﻿// <auto-generated />
using System;
using FS.TimeTracking.Repository.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FS.TimeTracking.Repository.SqLite.Migrations
{
    [DbContext(typeof(TimeTrackingDbContext))]
    [Migration("20210127101348_Inital")]
    partial class Inital
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("FS.TimeTracking.Shared.Models.TimeTracking.Activity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("CustomerId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Hidden")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("Name", "Hidden");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("FS.TimeTracking.Shared.Models.TimeTracking.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<string>("CompanyName")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactName")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Country")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Hidden")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("TEXT");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Street")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("ZipCode")
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ShortName", "Hidden");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("FS.TimeTracking.Shared.Models.TimeTracking.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Hidden")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("Name", "Hidden");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("FS.TimeTracking.Shared.Models.TimeTracking.TimeSheet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ActivityId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Billable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Modified")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("CustomerId");

                    b.ToTable("TimeSheets");
                });

            modelBuilder.Entity("FS.TimeTracking.Shared.Models.TimeTracking.Activity", b =>
                {
                    b.HasOne("FS.TimeTracking.Shared.Models.TimeTracking.Customer", null)
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("FS.TimeTracking.Shared.Models.TimeTracking.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("FS.TimeTracking.Shared.Models.TimeTracking.Project", b =>
                {
                    b.HasOne("FS.TimeTracking.Shared.Models.TimeTracking.Customer", null)
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("FS.TimeTracking.Shared.Models.TimeTracking.TimeSheet", b =>
                {
                    b.HasOne("FS.TimeTracking.Shared.Models.TimeTracking.Activity", null)
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("FS.TimeTracking.Shared.Models.TimeTracking.Customer", null)
                        .WithMany()
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
