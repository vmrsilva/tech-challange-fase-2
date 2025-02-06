using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechChallange.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class is_deleted_flag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Region",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Contact",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Region");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Contact");
        }
    }
}
