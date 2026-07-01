using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SproutPlot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGardenTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GardenId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlantId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    DueOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Gardens_GardenId",
                        column: x => x.GardenId,
                        principalSchema: "app",
                        principalTable: "Gardens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Plants_PlantId",
                        column: x => x.PlantId,
                        principalSchema: "app",
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_GardenId_DueOn",
                schema: "app",
                table: "Tasks",
                columns: new[] { "GardenId", "DueOn" });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PlantId",
                schema: "app",
                table: "Tasks",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Status_DueOn",
                schema: "app",
                table: "Tasks",
                columns: new[] { "Status", "DueOn" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "app");
        }
    }
}
