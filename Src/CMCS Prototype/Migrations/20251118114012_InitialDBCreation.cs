using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMCS_Prototype.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedByCoordinator",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ApprovedByManager",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "CoordinatorApprovalDate",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "ManagerApprovalDate",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Claims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovedByCoordinator",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApprovedByManager",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CoordinatorApprovalDate",
                table: "Claims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ManagerApprovalDate",
                table: "Claims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Claims",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
