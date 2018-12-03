﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using _101mngr.WebApp.Data;

namespace _101mngr.WebApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180829190356_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("_101mngr.WebApp.Data.Match", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AwayTeamId");

                    b.Property<int>("HomeTeamId");

                    b.Property<string>("Name");

                    b.Property<string>("Scoreboard");

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("TournamentId");

                    b.HasKey("Id");

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("_101mngr.WebApp.Data.Player", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("AccountId");

                    b.Property<int>("Aggression");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("CountryCode");

                    b.Property<int>("Experience");

                    b.Property<string>("FirstName");

                    b.Property<double>("Height");

                    b.Property<string>("LastName");

                    b.Property<decimal>("WalletBalance");

                    b.Property<double>("Weight");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("_101mngr.WebApp.Data.PlayerMatchHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("MatchId");

                    b.Property<long>("PlayerId");

                    b.HasKey("Id");

                    b.HasIndex("MatchId");

                    b.HasIndex("PlayerId");

                    b.ToTable("MatchHistory");
                });

            modelBuilder.Entity("_101mngr.WebApp.Data.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CountryCode");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Teams");

                    b.HasData(
                        new { Id = 1, CountryCode = "UA", Name = "Carpathia" },
                        new { Id = 2, CountryCode = "UA", Name = "Seacrew" }
                    );
                });

            modelBuilder.Entity("_101mngr.WebApp.Data.Tournament", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CountryCode");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Tournaments");

                    b.HasData(
                        new { Id = 1, CountryCode = "UA", Description = "Ukrainian Division", Name = "Ukrainian Division" }
                    );
                });

            modelBuilder.Entity("_101mngr.WebApp.Data.PlayerMatchHistory", b =>
                {
                    b.HasOne("_101mngr.WebApp.Data.Match", "Match")
                        .WithMany()
                        .HasForeignKey("MatchId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("_101mngr.WebApp.Data.Player", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
