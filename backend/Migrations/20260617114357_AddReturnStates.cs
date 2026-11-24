using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "id_statusu_zgloszenia",
                table: "zwrot_zamowienia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id_typu_zgloszenia",
                table: "zwrot_zamowienia",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "StatusZgloszenia",
                columns: table => new
                {
                    IdStatusuZgloszenia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nazwa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusZgloszenia", x => x.IdStatusuZgloszenia);
                });

            migrationBuilder.CreateTable(
                name: "TypZgloszenia",
                columns: table => new
                {
                    IdTypuZgloszenia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nazwa = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypZgloszenia", x => x.IdTypuZgloszenia);
                });

            migrationBuilder.CreateIndex(
                name: "IX_zwrot_zamowienia_id_pozycji",
                table: "zwrot_zamowienia",
                column: "id_pozycji");

            migrationBuilder.CreateIndex(
                name: "IX_zwrot_zamowienia_id_statusu_zgloszenia",
                table: "zwrot_zamowienia",
                column: "id_statusu_zgloszenia");

            migrationBuilder.CreateIndex(
                name: "IX_zwrot_zamowienia_id_typu_zgloszenia",
                table: "zwrot_zamowienia",
                column: "id_typu_zgloszenia");

            migrationBuilder.AddForeignKey(
                name: "fk_zwrot_zamowienia_pozycja_zamowienia",
                table: "zwrot_zamowienia",
                column: "id_pozycji",
                principalTable: "pozycja_zamowienia",
                principalColumn: "id_pozycji",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_zwrot_zamowienia_slownik_status_zgloszenia",
                table: "zwrot_zamowienia",
                column: "id_statusu_zgloszenia",
                principalTable: "StatusZgloszenia",
                principalColumn: "IdStatusuZgloszenia",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_zwrot_zamowienia_slownik_typ_zgloszenia",
                table: "zwrot_zamowienia",
                column: "id_typu_zgloszenia",
                principalTable: "TypZgloszenia",
                principalColumn: "IdTypuZgloszenia",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_zwrot_zamowienia_pozycja_zamowienia",
                table: "zwrot_zamowienia");

            migrationBuilder.DropForeignKey(
                name: "fk_zwrot_zamowienia_slownik_status_zgloszenia",
                table: "zwrot_zamowienia");

            migrationBuilder.DropForeignKey(
                name: "fk_zwrot_zamowienia_slownik_typ_zgloszenia",
                table: "zwrot_zamowienia");

            migrationBuilder.DropTable(
                name: "StatusZgloszenia");

            migrationBuilder.DropTable(
                name: "TypZgloszenia");

            migrationBuilder.DropIndex(
                name: "IX_zwrot_zamowienia_id_pozycji",
                table: "zwrot_zamowienia");

            migrationBuilder.DropIndex(
                name: "IX_zwrot_zamowienia_id_statusu_zgloszenia",
                table: "zwrot_zamowienia");

            migrationBuilder.DropIndex(
                name: "IX_zwrot_zamowienia_id_typu_zgloszenia",
                table: "zwrot_zamowienia");

            migrationBuilder.DropColumn(
                name: "id_statusu_zgloszenia",
                table: "zwrot_zamowienia");

            migrationBuilder.DropColumn(
                name: "id_typu_zgloszenia",
                table: "zwrot_zamowienia");
        }
    }
}
