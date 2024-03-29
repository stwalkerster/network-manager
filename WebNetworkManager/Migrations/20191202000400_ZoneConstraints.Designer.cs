﻿// <auto-generated />
using DnsWebApp.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20191202000400_ZoneConstraints")]
    partial class ZoneConstraints
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("DnsWebApp.Models.Database.Zone", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Administrator")
                        .HasColumnType("text");

                    b.Property<int>("Expire")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PrimaryNameServer")
                        .HasColumnType("text");

                    b.Property<int>("Refresh")
                        .HasColumnType("integer");

                    b.Property<int>("Retry")
                        .HasColumnType("integer");

                    b.Property<int>("SerialNumber")
                        .HasColumnType("integer");

                    b.Property<int>("TimeToLive")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Zones");
                });
#pragma warning restore 612, 618
        }
    }
}
