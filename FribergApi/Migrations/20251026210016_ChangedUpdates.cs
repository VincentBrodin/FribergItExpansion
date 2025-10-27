using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangedUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Updates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Updates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
