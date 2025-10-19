using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS_Prototype.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Claims");
        }
    }
}
