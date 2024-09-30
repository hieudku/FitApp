using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitApp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSpecificWorkout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSpecificWorkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    CaloriesBurned = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSpecificWorkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSpecificWorkouts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSpecificWorkouts_UserId",
                table: "UserSpecificWorkouts",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSpecificWorkouts");
        }
    }
}
