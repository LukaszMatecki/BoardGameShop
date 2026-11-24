using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class discountCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "kod_promocyjny",
                columns: table => new
                {
                    id_kodu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    kod = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    znizka_procentowa = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("kod_promocyjny_pkey", x => x.id_kodu);
                });

            migrationBuilder.CreateTable(
                name: "uzytkownik_kod_promocyjny",
                columns: table => new
                {
                    id_kodu = table.Column<int>(type: "integer", nullable: false),
                    id_uzytkownika = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("uzytkownik_kod_promocyjny_pkey", x => new { x.id_kodu, x.id_uzytkownika });
                    table.ForeignKey(
                        name: "uzytkownik_kod_promocyjny_id_kodu_fkey",
                        column: x => x.id_kodu,
                        principalTable: "kod_promocyjny",
                        principalColumn: "id_kodu");
                    table.ForeignKey(
                        name: "uzytkownik_kod_promocyjny_id_uzytkownika_fkey",
                        column: x => x.id_uzytkownika,
                        principalTable: "uzytkownik_details",
                        principalColumn: "id_uzytkownika");
                });

            migrationBuilder.CreateIndex(
                name: "IX_uzytkownik_kod_promocyjny_id_uzytkownika",
                table: "uzytkownik_kod_promocyjny",
                column: "id_uzytkownika");

            migrationBuilder.AddForeignKey(
                name: "zamowienie_id_adresu_zamowienia_fkey",
                table: "zamowienie",
                column: "id_adresu_zamowienia",
                principalTable: "adres",
                principalColumn: "id_adresu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "zamowienie_id_adresu_zamowienia_fkey",
                table: "zamowienie");

            migrationBuilder.DropTable(
                name: "uzytkownik_kod_promocyjny");

            migrationBuilder.DropTable(
                name: "kod_promocyjny");


            migrationBuilder.DropColumn(
                name: "imie",
                table: "zamowienie");

            migrationBuilder.DropColumn(
                name: "nazwisko",
                table: "zamowienie");

            migrationBuilder.DropColumn(
                name: "numer_telefonu",
                table: "zamowienie");

            migrationBuilder.DropColumn(
                name: "zdjecie",
                table: "produkt");

            migrationBuilder.AddForeignKey(
                name: "zamowienie_id_adresu_zamowienia_fkey",
                table: "zamowienie",
                column: "id_adresu_zamowienia",
                principalTable: "adres_zamowienia",
                principalColumn: "id_adresu_zamowienia");
        }
    }
}
