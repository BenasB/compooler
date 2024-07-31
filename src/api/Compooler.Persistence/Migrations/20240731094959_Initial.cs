using System;
using Microsoft.EntityFrameworkCore.Migrations;
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
            migrationBuilder.CreateTable(
                name: "CommuteGroups",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    DriverId = table.Column<int>(type: "integer", nullable: false),
                    MaxPassengers = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommuteGroups", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "CommuteGroupsPassengers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CommuteGroupId = table.Column<int>(type: "integer", nullable: false),
                    JoinedAt = table.Column<DateTimeOffset>(
                        type: "timestamp with time zone",
                        nullable: false,
                        defaultValueSql: "CURRENT_TIMESTAMP"
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_CommuteGroupsPassengers",
                        x => new { x.CommuteGroupId, x.UserId }
                    );
                    table.ForeignKey(
                        name: "FK_CommuteGroupsPassengers_CommuteGroups_CommuteGroupId",
                        column: x => x.CommuteGroupId,
                        principalTable: "CommuteGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    CommuteGroupId = table.Column<int>(type: "integer", nullable: false),
                    Start_Latitude = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Start_Longitude = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Finish_Latitude = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    ),
                    Finish_Longitude = table.Column<double>(
                        type: "double precision",
                        nullable: false
                    )
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.CommuteGroupId);
                    table.ForeignKey(
                        name: "FK_Routes_CommuteGroups_CommuteGroupId",
                        column: x => x.CommuteGroupId,
                        principalTable: "CommuteGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "CommuteGroupsPassengers");

            migrationBuilder.DropTable(name: "Routes");

            migrationBuilder.DropTable(name: "Users");

            migrationBuilder.DropTable(name: "CommuteGroups");
        }
    }
}
