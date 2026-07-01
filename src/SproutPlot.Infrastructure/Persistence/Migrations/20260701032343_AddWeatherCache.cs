using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SproutPlot.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherCache",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LatitudeKey = table.Column<double>(type: "double precision", nullable: false),
                    LongitudeKey = table.Column<double>(type: "double precision", nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false),
                    FetchedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherCache", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherCache_LatitudeKey_LongitudeKey",
                schema: "app",
                table: "WeatherCache",
                columns: new[] { "LatitudeKey", "LongitudeKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherCache",
                schema: "app");
        }
    }
}
