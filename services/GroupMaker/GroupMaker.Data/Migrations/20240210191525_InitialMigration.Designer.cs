﻿// <auto-generated />
using System;
using GroupMaker.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GroupMaker.Data.Migrations
{
    [DbContext(typeof(GroupContext))]
    [Migration("20240210191525_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GroupMaker.Api.Entities.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Days")
                        .HasColumnType("integer");

                    b.Property<TimeOnly>("StartTime")
                        .HasColumnType("time without time zone");

                    b.Property<int>("TotalSeats")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("GroupMaker.Api.Entities.Group", b =>
                {
                    b.OwnsOne("GroupMaker.Api.Entities.Coordinates", "EndLocation", b1 =>
                        {
                            b1.Property<int>("GroupId")
                                .HasColumnType("integer");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double>("Longitude")
                                .HasColumnType("double precision");

                            b1.HasKey("GroupId");

                            b1.ToTable("Groups");

                            b1.WithOwner()
                                .HasForeignKey("GroupId");
                        });

                    b.OwnsOne("GroupMaker.Api.Entities.Coordinates", "StartLocation", b1 =>
                        {
                            b1.Property<int>("GroupId")
                                .HasColumnType("integer");

                            b1.Property<double>("Latitude")
                                .HasColumnType("double precision");

                            b1.Property<double>("Longitude")
                                .HasColumnType("double precision");

                            b1.HasKey("GroupId");

                            b1.ToTable("Groups");

                            b1.WithOwner()
                                .HasForeignKey("GroupId");
                        });

                    b.Navigation("EndLocation")
                        .IsRequired();

                    b.Navigation("StartLocation")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
