using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jira.Migrations
{
    /// <inheritdoc />
    public partial class zmianyRelacji : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SprintMasterId",
                table: "Sprints",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_SprintMasterId",
                table: "Sprints",
                column: "SprintMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprints_AspNetUsers_SprintMasterId",
                table: "Sprints",
                column: "SprintMasterId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprints_AspNetUsers_SprintMasterId",
                table: "Sprints");

            migrationBuilder.DropIndex(
                name: "IX_Sprints_SprintMasterId",
                table: "Sprints");

            migrationBuilder.DropColumn(
                name: "SprintMasterId",
                table: "Sprints");
        }
    }
}
