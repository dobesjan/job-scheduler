﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebMonitoring.DataAccess.Data;

#nullable disable

namespace WebMonitoring.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240718195413_entityParameters")]
    partial class entityParameters
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("JobScheduler.Plugin.WebPage.WebpageResult", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<long>("ContentLength")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EntityTypeId")
                        .HasColumnType("int");

                    b.Property<string>("ErrorMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("HasError")
                        .HasColumnType("bit");

                    b.Property<long>("ResponseTime")
                        .HasColumnType("bigint");

                    b.Property<int>("StatusCode")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WebpageMetrics");
                });

            modelBuilder.Entity("WebMonitoring.Models.MonitoredEntityType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("EntityTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Web page"
                        });
                });

            modelBuilder.Entity("WebMonitoring.Models.MonitoredWebPage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Interval")
                        .HasColumnType("int");

                    b.Property<int>("MonitoredEntityTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("MonitoredEntityTypeId");

                    b.ToTable("WebPages");
                });

            modelBuilder.Entity("WebMonitoring.Models.MonitoredWebPage", b =>
                {
                    b.HasOne("WebMonitoring.Models.MonitoredEntityType", "MonitoredEntityType")
                        .WithMany()
                        .HasForeignKey("MonitoredEntityTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MonitoredEntityType");
                });
#pragma warning restore 612, 618
        }
    }
}
