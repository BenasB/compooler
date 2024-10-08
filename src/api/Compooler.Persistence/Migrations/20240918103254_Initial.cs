﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Compooler.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Rides",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    DriverId = table.Column<string>(type: "text", nullable: false),
                    MaxPassengers = table.Column<int>(type: "integer", nullable: false),
                    TimeOfDeparture = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rides", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(28)", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "RidePassengers",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RideId = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RidePassengers", x => new { x.RideId, x.UserId });
                    table.ForeignKey(
                        name: "FK_RidePassengers_Rides_RideId",
                        column: x => x.RideId,
                        principalTable: "Rides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RideId = table.Column<int>(type: "integer", nullable: false),
                    Start_Point = table.Column<Point>(type: "geography (point)", nullable: false),
                    Finish_Point = table.Column<Point>(type: "geography (point)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RideId);
                    table.ForeignKey(
                        name: "FK_Routes_Rides_RideId",
                        column: x => x.RideId,
                        principalTable: "Rides",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_RidePassengers_UserId_RideId",
                table: "RidePassengers",
                columns: new[] { "UserId", "RideId" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Rides_DriverId_TimeOfDeparture_Id",
                table: "Rides",
                columns: new[] { "DriverId", "TimeOfDeparture", "Id" }
            );

            migrationBuilder
                .CreateIndex(
                    name: "IX_Routes_Finish_Point",
                    table: "Routes",
                    column: "Finish_Point"
                )
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder
                .CreateIndex(name: "IX_Routes_Start_Point", table: "Routes", column: "Start_Point")
                .Annotation("Npgsql:IndexMethod", "GIST");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RidePassengers");

            migrationBuilder.DropTable(name: "Routes");

            migrationBuilder.DropTable(name: "Users");

            migrationBuilder.DropTable(name: "Rides");
        }
    }
}
