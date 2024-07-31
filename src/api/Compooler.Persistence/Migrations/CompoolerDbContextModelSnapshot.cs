﻿// <auto-generated />
using System;
using Compooler.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Compooler.Persistence.Migrations
{
    [DbContext(typeof(CompoolerDbContext))]
    partial class CompoolerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("DriverId")
                        .HasColumnType("integer");

                    b.Property<int>("MaxPassengers")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("CommuteGroups");
                });

            modelBuilder.Entity("Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroupPassenger", b =>
                {
                    b.Property<int?>("CommuteGroupId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("JoinedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    b.HasKey("CommuteGroupId", "UserId");

                    b.ToTable("CommuteGroupsPassengers");
                });

            modelBuilder.Entity("Compooler.Domain.Entities.UserEntity.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroup", b =>
                {
                    b.OwnsOne("Compooler.Domain.Entities.CommuteGroupEntity.Route", "Route", b1 =>
                        {
                            b1.Property<int>("CommuteGroupId")
                                .HasColumnType("integer");

                            b1.HasKey("CommuteGroupId");

                            b1.ToTable("Routes", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("CommuteGroupId");

                            b1.OwnsOne("Compooler.Domain.Entities.CommuteGroupEntity.GeographicCoordinates", "Finish", b2 =>
                                {
                                    b2.Property<int>("RouteCommuteGroupId")
                                        .HasColumnType("integer");

                                    b2.Property<double>("Latitude")
                                        .HasColumnType("double precision");

                                    b2.Property<double>("Longitude")
                                        .HasColumnType("double precision");

                                    b2.HasKey("RouteCommuteGroupId");

                                    b2.ToTable("Routes");

                                    b2.WithOwner()
                                        .HasForeignKey("RouteCommuteGroupId");
                                });

                            b1.OwnsOne("Compooler.Domain.Entities.CommuteGroupEntity.GeographicCoordinates", "Start", b2 =>
                                {
                                    b2.Property<int>("RouteCommuteGroupId")
                                        .HasColumnType("integer");

                                    b2.Property<double>("Latitude")
                                        .HasColumnType("double precision");

                                    b2.Property<double>("Longitude")
                                        .HasColumnType("double precision");

                                    b2.HasKey("RouteCommuteGroupId");

                                    b2.ToTable("Routes");

                                    b2.WithOwner()
                                        .HasForeignKey("RouteCommuteGroupId");
                                });

                            b1.Navigation("Finish")
                                .IsRequired();

                            b1.Navigation("Start")
                                .IsRequired();
                        });

                    b.Navigation("Route")
                        .IsRequired();
                });

            modelBuilder.Entity("Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroupPassenger", b =>
                {
                    b.HasOne("Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroup", null)
                        .WithMany("Passengers")
                        .HasForeignKey("CommuteGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Compooler.Domain.Entities.CommuteGroupEntity.CommuteGroup", b =>
                {
                    b.Navigation("Passengers");
                });
#pragma warning restore 612, 618
        }
    }
}
