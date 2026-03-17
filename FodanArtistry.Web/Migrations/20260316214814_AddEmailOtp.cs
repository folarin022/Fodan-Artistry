using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FodanArtistry.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CodeExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationCode",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailVerificationCode",
                table: "AspNetUsers");
        }
    }
}
