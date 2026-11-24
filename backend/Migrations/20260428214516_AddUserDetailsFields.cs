using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDetailsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imie",
                table: "uzytkownik_details",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nazwisko",
                table: "uzytkownik_details",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumerTelefonu",
                table: "uzytkownik_details",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imie",
                table: "uzytkownik_details");

            migrationBuilder.DropColumn(
                name: "Nazwisko",
                table: "uzytkownik_details");

            migrationBuilder.DropColumn(
                name: "NumerTelefonu",
                table: "uzytkownik_details");
        }
    }
}
