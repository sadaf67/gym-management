using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tahila.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBodyAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyAnalyses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuestName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    IsMale = table.Column<bool>(type: "bit", nullable: false),
                    WaistCircumference = table.Column<float>(type: "real", nullable: true),
                    HipCircumference = table.Column<float>(type: "real", nullable: true),
                    NeckCircumference = table.Column<float>(type: "real", nullable: true),
                    ActivityLevel = table.Column<int>(type: "int", nullable: false),
                    BMI = table.Column<float>(type: "real", nullable: false),
                    BMICategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdealWeightMin = table.Column<float>(type: "real", nullable: false),
                    IdealWeightMax = table.Column<float>(type: "real", nullable: false),
                    BMR = table.Column<float>(type: "real", nullable: false),
                    TDEE = table.Column<float>(type: "real", nullable: false),
                    BodyFatPercentage = table.Column<float>(type: "real", nullable: true),
                    LeanBodyMass = table.Column<float>(type: "real", nullable: true),
                    Recommendation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyAnalyses", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 7, 21, 15, 6, 648, DateTimeKind.Local).AddTicks(482));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyAnalyses");

            migrationBuilder.UpdateData(
                table: "SiteSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 6, 7, 14, 24, 6, 245, DateTimeKind.Local).AddTicks(5049));
        }
    }
}
