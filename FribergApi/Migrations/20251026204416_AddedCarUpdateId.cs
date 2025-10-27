using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedCarUpdateId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Update_Cars_CarId",
                table: "Update");

            migrationBuilder.AlterColumn<string>(
                name: "CarId",
                table: "Update",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Update_Cars_CarId",
                table: "Update",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Update_Cars_CarId",
                table: "Update");

            migrationBuilder.AlterColumn<string>(
                name: "CarId",
                table: "Update",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Update_Cars_CarId",
                table: "Update",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id");
        }
    }
}
