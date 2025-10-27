using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedSoftDeleteToCarsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Cars");
        }
    }
}
