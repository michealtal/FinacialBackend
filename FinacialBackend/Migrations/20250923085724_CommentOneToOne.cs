using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinacialBackend.Migrations
{
    /// <inheritdoc />
    public partial class CommentOneToOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Coments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Coments_AppUserId",
                table: "Coments",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coments_AspNetUsers_AppUserId",
                table: "Coments",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coments_AspNetUsers_AppUserId",
                table: "Coments");

            migrationBuilder.DropIndex(
                name: "IX_Coments_AppUserId",
                table: "Coments");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Coments");
        }
    }
}
