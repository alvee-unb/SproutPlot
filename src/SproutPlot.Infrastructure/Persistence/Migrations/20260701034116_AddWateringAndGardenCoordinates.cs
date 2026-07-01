using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SproutPlot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWateringAndGardenCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                schema: "app",
                table: "Gardens",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                schema: "app",
                table: "Gardens",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WateringEvents",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GardenId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlantId = table.Column<Guid>(type: "uuid", nullable: true),
                    WateredAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AmountLiters = table.Column<double>(type: "double precision", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WateringEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WateringEvents_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalSchema: "app",
                        principalTable: "Gardens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WateringEvents_Plants_PlantId",
                        column: x => x.PlantId,
                        principalSchema: "app",
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WateringEvents_GardenId_WateredAtUtc",
                schema: "app",
                table: "WateringEvents",
                columns: new[] { "GardenId", "WateredAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_WateringEvents_PlantId",
                schema: "app",
                table: "WateringEvents",
                column: "PlantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WateringEvents",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "Latitude",
                schema: "app",
                table: "Gardens");

            migrationBuilder.DropColumn(
                name: "Longitude",
                schema: "app",
                table: "Gardens");
        }
    }
}
