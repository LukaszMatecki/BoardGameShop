using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataNajnizszejCenyW30Dni",
                table: "produkt",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NajnizszaCenaW30Dni",
                table: "produkt",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromocjaWProc",
                table: "produkt",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataNajnizszejCenyW30Dni",
                table: "produkt");

            migrationBuilder.DropColumn(
                name: "NajnizszaCenaW30Dni",
                table: "produkt");

            migrationBuilder.DropColumn(
                name: "PromocjaWProc",
                table: "produkt");
        }
    }
}
