﻿// <auto-generated />
using System;
using DnsWebApp.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DnsWebApp.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20200422135352_ZoneGroupOwner")]
    partial class ZoneGroupOwner
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("DnsWebApp.Models.Database.Currency", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text");

                    b.Property<decimal?>("ExchangeRate")
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("ExchangeRateUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Symbol")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Code")
                        .IsUnique();

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.FavouriteDomains", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("ZoneId");

                    b.ToTable("FavouriteDomains");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.HorizonView", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ViewName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ViewTag")
                        .HasColumnType("text");

                    b.Property<string>("ViewTagColour")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("HorizonViews");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.Record", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Class")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long?>("TimeToLive")
                        .HasColumnType("bigint");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("ZoneGroupId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ZoneGroupId");

                    b.HasIndex("ZoneId");

                    b.ToTable("Record");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.Registrar", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("AllowRenewals")
                        .HasColumnType("boolean");

                    b.Property<bool>("AllowTransfers")
                        .HasColumnType("boolean");

                    b.Property<long?>("CurrencyId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("PricesIncludeVat")
                        .HasColumnType("boolean");

                    b.Property<decimal?>("PrivacyFee")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("TransferOutFee")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Registrar");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.RegistrarTldSupport", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<bool>("PrivacyIncluded")
                        .HasColumnType("boolean");

                    b.Property<long>("RegistrarId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("RenewalPrice")
                        .HasColumnType("numeric");

                    b.Property<DateTime?>("RenewalPriceUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("TopLevelDomainId")
                        .HasColumnType("bigint");

                    b.Property<bool>("TransferIncludesRenewal")
                        .HasColumnType("boolean");

                    b.Property<decimal?>("TransferPrice")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("TopLevelDomainId");

                    b.HasIndex("RegistrarId", "TopLevelDomainId")
                        .IsUnique();

                    b.ToTable("RegistrarTldSupport");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.TopLevelDomain", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Domain")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Registry")
                        .HasColumnType("text");

                    b.Property<string>("RegistryUrl")
                        .HasColumnType("text");

                    b.Property<string>("WhoisServer")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Domain")
                        .IsUnique();

                    b.ToTable("TopLevelDomains");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.Zone", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Administrator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("DefaultTimeToLive")
                        .HasColumnType("bigint");

                    b.Property<bool>("Enabled")
                        .HasColumnType("boolean");

                    b.Property<long>("Expire")
                        .HasColumnType("bigint");

                    b.Property<long?>("HorizonViewId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .HasColumnType("text");

                    b.Property<string>("PrimaryNameServer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("Refresh")
                        .HasColumnType("bigint");

                    b.Property<long?>("RegistrarId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("RegistrationExpiry")
                        .HasColumnType("timestamp without time zone");

                    b.Property<long>("Retry")
                        .HasColumnType("bigint");

                    b.Property<long>("SerialNumber")
                        .HasColumnType("bigint");

                    b.Property<long>("TimeToLive")
                        .HasColumnType("bigint");

                    b.Property<long>("TopLevelDomainId")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("WhoisLastUpdated")
                        .HasColumnType("timestamp without time zone");

                    b.HasKey("Id");

                    b.HasIndex("HorizonViewId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("RegistrarId");

                    b.HasIndex("TopLevelDomainId", "Name", "HorizonViewId")
                        .IsUnique();

                    b.ToTable("Zones");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.ZoneGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("ZoneGroups");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.ZoneGroupMember", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<long>("ZoneGroupId")
                        .HasColumnType("bigint");

                    b.Property<long>("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ZoneGroupId");

                    b.HasIndex("ZoneId");

                    b.ToTable("ZoneGroupMember");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");

                    b.HasData(
                        new
                        {
                            Id = "d435bc2d-baa2-4937-b460-9e2d25135855",
                            ConcurrencyStamp = "24c0e2cf-b4c8-4cf1-8386-dbf2187ac6a2",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        },
                        new
                        {
                            Id = "99860125-ed38-4839-adb5-750d3ed03b33",
                            ConcurrencyStamp = "4d29d3fa-9646-451c-9dee-6084d5d5424d",
                            Name = "DNS Manager",
                            NormalizedName = "DNS MANAGER"
                        },
                        new
                        {
                            Id = "66617cb8-7658-45d7-8e8c-827812ab88f7",
                            ConcurrencyStamp = "2b908bc3-42bb-4a57-9786-4771f09f98d0",
                            Name = "DNS User",
                            NormalizedName = "DNS USER"
                        },
                        new
                        {
                            Id = "4fcd452b-4033-481f-b434-15baf2e8d86d",
                            ConcurrencyStamp = "c9bb108e-1751-4ebb-9601-1a0cd5d709de",
                            Name = "Static Data",
                            NormalizedName = "STATIC DATA"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasColumnType("character varying(256)")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.FavouriteDomains", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DnsWebApp.Models.Database.Zone", "Zone")
                        .WithMany("FavouriteDomains")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.Record", b =>
                {
                    b.HasOne("DnsWebApp.Models.Database.ZoneGroup", "ZoneGroup")
                        .WithMany("Records")
                        .HasForeignKey("ZoneGroupId");

                    b.HasOne("DnsWebApp.Models.Database.Zone", "Zone")
                        .WithMany("Records")
                        .HasForeignKey("ZoneId");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.Registrar", b =>
                {
                    b.HasOne("DnsWebApp.Models.Database.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.RegistrarTldSupport", b =>
                {
                    b.HasOne("DnsWebApp.Models.Database.Registrar", "Registrar")
                        .WithMany("RegistrarTldSupports")
                        .HasForeignKey("RegistrarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DnsWebApp.Models.Database.TopLevelDomain", "TopLevelDomain")
                        .WithMany("RegistrarTldSupports")
                        .HasForeignKey("TopLevelDomainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.Zone", b =>
                {
                    b.HasOne("DnsWebApp.Models.Database.HorizonView", "HorizonView")
                        .WithMany("Zones")
                        .HasForeignKey("HorizonViewId");

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");

                    b.HasOne("DnsWebApp.Models.Database.Registrar", "Registrar")
                        .WithMany("Zones")
                        .HasForeignKey("RegistrarId");

                    b.HasOne("DnsWebApp.Models.Database.TopLevelDomain", "TopLevelDomain")
                        .WithMany("Zones")
                        .HasForeignKey("TopLevelDomainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.ZoneGroup", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("DnsWebApp.Models.Database.ZoneGroupMember", b =>
                {
                    b.HasOne("DnsWebApp.Models.Database.ZoneGroup", "ZoneGroup")
                        .WithMany("ZoneGroupMembers")
                        .HasForeignKey("ZoneGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DnsWebApp.Models.Database.Zone", "Zone")
                        .WithMany("ZoneGroupMembers")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
