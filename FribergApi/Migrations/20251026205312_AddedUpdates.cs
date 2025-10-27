using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Update_Cars_CarId",
                table: "Update");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Update",
                table: "Update");

            migrationBuilder.RenameTable(
                name: "Update",
                newName: "Updates");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Updates",
                newName: "OldValue");

            migrationBuilder.RenameIndex(
                name: "IX_Update_CarId",
                table: "Updates",
                newName: "IX_Updates_CarId");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "Updates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Updates",
                table: "Updates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Updates_Cars_CarId",
                table: "Updates",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Updates_Cars_CarId",
                table: "Updates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Updates",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "Updates");

            migrationBuilder.RenameTable(
                name: "Updates",
                newName: "Update");

            migrationBuilder.RenameColumn(
                name: "OldValue",
                table: "Update",
                newName: "Value");

            migrationBuilder.RenameIndex(
                name: "IX_Updates_CarId",
                table: "Update",
                newName: "IX_Update_CarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Update",
                table: "Update",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Update_Cars_CarId",
                table: "Update",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
