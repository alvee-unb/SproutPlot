using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SproutPlot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPlants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plants",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GardenId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlantTypeId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Variety = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DatePlanted = table.Column<DateOnly>(type: "date", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Plants_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalSchema: "app",
                        principalTable: "Gardens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Plants_PlantTypes_PlantTypeId",
                        column: x => x.PlantTypeId,
                        principalSchema: "app",
                        principalTable: "PlantTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "app",
                table: "PlantTypes",
                columns: new[] { "Id", "Category", "CreatedAtUtc", "Name", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tomato", null },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pepper", null },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lettuce", null },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Carrot", null },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cucumber", null },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zucchini", null },
                    { new Guid("a0000000-0000-0000-0000-000000000007"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Onion", null },
                    { new Guid("a0000000-0000-0000-0000-000000000008"), 0, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Potato", null },
                    { new Guid("a0000000-0000-0000-0000-000000000009"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Basil", null },
                    { new Guid("a0000000-0000-0000-0000-000000000010"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mint", null },
                    { new Guid("a0000000-0000-0000-0000-000000000011"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rosemary", null },
                    { new Guid("a0000000-0000-0000-0000-000000000012"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Thyme", null },
                    { new Guid("a0000000-0000-0000-0000-000000000013"), 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cilantro", null },
                    { new Guid("a0000000-0000-0000-0000-000000000014"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Strawberry", null },
                    { new Guid("a0000000-0000-0000-0000-000000000015"), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blueberry", null },
                    { new Guid("a0000000-0000-0000-0000-000000000016"), 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rose", null },
                    { new Guid("a0000000-0000-0000-0000-000000000017"), 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Marigold", null },
                    { new Guid("a0000000-0000-0000-0000-000000000018"), 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sunflower", null },
                    { new Guid("a0000000-0000-0000-0000-000000000019"), 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tulip", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Plants_GardenId",
                schema: "app",
                table: "Plants",
                column: "GardenId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_PlantTypeId",
                schema: "app",
                table: "Plants",
                column: "PlantTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantTypes_Name",
                schema: "app",
                table: "PlantTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Plants",
                schema: "app");

            migrationBuilder.DropTable(
                name: "PlantTypes",
                schema: "app");
        }
    }
}
