using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.LocalMigrations
{
    /// <inheritdoc />
    public partial class LocalInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adres_zamowienia",
                columns: table => new
                {
                    id_adresu_zamowienia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    miasto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ulica = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    numer_budynku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    numer_lokalu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    kod_pocztowy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    imie_odbiorcy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    nazwisko_odbiorcy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("adres_zamowienia_pkey", x => x.id_adresu_zamowienia);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "autor",
                columns: table => new
                {
                    id_autora = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    imie = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    nazwisko = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("autor_pkey", x => x.id_autora);
                });

            migrationBuilder.CreateTable(
                name: "kategoria",
                columns: table => new
                {
                    id_kategorii = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa_kategorii = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("kategoria_pkey", x => x.id_kategorii);
                });

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
                name: "slownik_metod_dostawy",
                columns: table => new
                {
                    id_metody_dostawy = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa_metody = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    koszt_bazowy = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("slownik_metod_dostawy_pkey", x => x.id_metody_dostawy);
                });

            migrationBuilder.CreateTable(
                name: "slownik_metod_platnosci",
                columns: table => new
                {
                    id_metody_platnosci = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa_platnosci = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("slownik_metod_platnosci_pkey", x => x.id_metody_platnosci);
                });

            migrationBuilder.CreateTable(
                name: "slownik_statusow",
                columns: table => new
                {
                    id_statusu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa_statusu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("slownik_statusow_pkey", x => x.id_statusu);
                });

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
                name: "tag",
                columns: table => new
                {
                    id_tagu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa_tagu = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tag_pkey", x => x.id_tagu);
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

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "uzytkownik_details",
                columns: table => new
                {
                    id_uzytkownika = table.Column<int>(type: "integer", nullable: false),
                    Imie = table.Column<string>(type: "text", nullable: true),
                    Nazwisko = table.Column<string>(type: "text", nullable: true),
                    NumerTelefonu = table.Column<string>(type: "text", nullable: true),
                    czy_usuniety = table.Column<bool>(type: "boolean", nullable: false),
                    data_rejestracji = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("uzytkownik_details_pkey", x => x.id_uzytkownika);
                    table.ForeignKey(
                        name: "FK_uzytkownik_details_AspNetUsers_id_uzytkownika",
                        column: x => x.id_uzytkownika,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "produkt",
                columns: table => new
                {
                    id_produktu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nazwa_produktu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    id_autora = table.Column<int>(type: "integer", nullable: false),
                    cena = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    PromocjaWProc = table.Column<int>(type: "integer", nullable: false),
                    NajnizszaCenaW30Dni = table.Column<decimal>(type: "numeric", nullable: true),
                    DataNajnizszejCenyW30Dni = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    stan_magazynowy = table.Column<int>(type: "integer", nullable: false),
                    data_wydania = table.Column<DateOnly>(type: "date", nullable: false),
                    opis = table.Column<string>(type: "text", nullable: true),
                    zdjecie = table.Column<string>(type: "text", nullable: true),
                    czy_ukryty = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("produkt_pkey", x => x.id_produktu);
                    table.ForeignKey(
                        name: "produkt_id_autora_fkey",
                        column: x => x.id_autora,
                        principalTable: "autor",
                        principalColumn: "id_autora");
                });

            migrationBuilder.CreateTable(
                name: "adres",
                columns: table => new
                {
                    id_adresu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_uzytkownika = table.Column<int>(type: "integer", nullable: true),
                    miasto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ulica = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    numer_budynku = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    numer_lokalu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    kod_pocztowy = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("adres_pkey", x => x.id_adresu);
                    table.ForeignKey(
                        name: "adres_id_uzytkownika_fkey",
                        column: x => x.id_uzytkownika,
                        principalTable: "uzytkownik_details",
                        principalColumn: "id_uzytkownika");
                });

            migrationBuilder.CreateTable(
                name: "koszyk",
                columns: table => new
                {
                    id_koszyka = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_uzytkownika = table.Column<int>(type: "integer", nullable: false),
                    data_ostatniej_modyfikacji = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("koszyk_pkey", x => x.id_koszyka);
                    table.ForeignKey(
                        name: "koszyk_id_uzytkownika_fkey",
                        column: x => x.id_uzytkownika,
                        principalTable: "uzytkownik_details",
                        principalColumn: "id_uzytkownika");
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

            migrationBuilder.CreateTable(
                name: "zadanie_powiadomienie",
                columns: table => new
                {
                    id_zadania = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tytul = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tresc = table.Column<string>(type: "text", nullable: false),
                    id_przypisanego_admina = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValueSql: "'DO_ZROBIENIA'::character varying"),
                    data_utworzenia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("zadanie_powiadomienie_pkey", x => x.id_zadania);
                    table.ForeignKey(
                        name: "zadanie_powiadomienie_id_przypisanego_admina_fkey",
                        column: x => x.id_przypisanego_admina,
                        principalTable: "uzytkownik_details",
                        principalColumn: "id_uzytkownika");
                });

            migrationBuilder.CreateTable(
                name: "kategoria_produkt",
                columns: table => new
                {
                    id_kategorii = table.Column<int>(type: "integer", nullable: false),
                    id_produktu = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("kategoria_produkt_pkey", x => new { x.id_kategorii, x.id_produktu });
                    table.ForeignKey(
                        name: "kategoria_produkt_id_kategorii_fkey",
                        column: x => x.id_kategorii,
                        principalTable: "kategoria",
                        principalColumn: "id_kategorii");
                    table.ForeignKey(
                        name: "kategoria_produkt_id_produktu_fkey",
                        column: x => x.id_produktu,
                        principalTable: "produkt",
                        principalColumn: "id_produktu");
                });

            migrationBuilder.CreateTable(
                name: "opinia_produktu",
                columns: table => new
                {
                    id_opinii = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_produktu = table.Column<int>(type: "integer", nullable: false),
                    id_uzytkownika = table.Column<int>(type: "integer", nullable: false),
                    ocena = table.Column<int>(type: "integer", nullable: false),
                    komentarz = table.Column<string>(type: "text", nullable: true),
                    data_wystawienia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    czy_zatwierdzona = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("opinia_produktu_pkey", x => x.id_opinii);
                    table.ForeignKey(
                        name: "opinia_produktu_id_produktu_fkey",
                        column: x => x.id_produktu,
                        principalTable: "produkt",
                        principalColumn: "id_produktu");
                    table.ForeignKey(
                        name: "opinia_produktu_id_uzytkownika_fkey",
                        column: x => x.id_uzytkownika,
                        principalTable: "uzytkownik_details",
                        principalColumn: "id_uzytkownika");
                });

            migrationBuilder.CreateTable(
                name: "tag_produkt",
                columns: table => new
                {
                    id_tagu = table.Column<int>(type: "integer", nullable: false),
                    id_produktu = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tag_produkt_pkey", x => new { x.id_tagu, x.id_produktu });
                    table.ForeignKey(
                        name: "tag_produkt_id_produktu_fkey",
                        column: x => x.id_produktu,
                        principalTable: "produkt",
                        principalColumn: "id_produktu");
                    table.ForeignKey(
                        name: "tag_produkt_id_tagu_fkey",
                        column: x => x.id_tagu,
                        principalTable: "tag",
                        principalColumn: "id_tagu");
                });

            migrationBuilder.CreateTable(
                name: "ulubione_produkty",
                columns: table => new
                {
                    id_uzytkownika = table.Column<int>(type: "integer", nullable: false),
                    id_produktu = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ulubione_produkty_pkey", x => new { x.id_uzytkownika, x.id_produktu });
                    table.ForeignKey(
                        name: "ulubione_produkty_id_produktu_fkey",
                        column: x => x.id_produktu,
                        principalTable: "produkt",
                        principalColumn: "id_produktu");
                    table.ForeignKey(
                        name: "ulubione_produkty_id_uzytkownika_fkey",
                        column: x => x.id_uzytkownika,
                        principalTable: "uzytkownik_details",
                        principalColumn: "id_uzytkownika");
                });

            migrationBuilder.CreateTable(
                name: "zapytanie_o_produkt",
                columns: table => new
                {
                    id_zapytania = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_produktu = table.Column<int>(type: "integer", nullable: true),
                    email_pytajacego = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    tresc_zapytania = table.Column<string>(type: "text", nullable: false),
                    data_zapytania = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    czy_obsluzone = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("zapytanie_o_produkt_pkey", x => x.id_zapytania);
                    table.ForeignKey(
                        name: "zapytanie_o_produkt_id_produktu_fkey",
                        column: x => x.id_produktu,
                        principalTable: "produkt",
                        principalColumn: "id_produktu");
                });

            migrationBuilder.CreateTable(
                name: "zamowienie",
                columns: table => new
                {
                    id_zamowienia = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_uzytkownika = table.Column<int>(type: "integer", nullable: true),
                    id_statusu = table.Column<int>(type: "integer", nullable: false),
                    id_metody_dostawy = table.Column<int>(type: "integer", nullable: false),
                    id_metody_platnosci = table.Column<int>(type: "integer", nullable: false),
                    data_zamowienia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    calkowita_kwota = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    id_adresu_zamowienia = table.Column<int>(type: "integer", nullable: true),
                    imie = table.Column<string>(type: "text", nullable: true),
                    nazwisko = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    numer_telefonu = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("zamowienie_pkey", x => x.id_zamowienia);
                    table.ForeignKey(
                        name: "zamowienie_id_adresu_zamowienia_fkey",
                        column: x => x.id_adresu_zamowienia,
                        principalTable: "adres",
                        principalColumn: "id_adresu");
                    table.ForeignKey(
                        name: "zamowienie_id_metody_dostawy_fkey",
                        column: x => x.id_metody_dostawy,
                        principalTable: "slownik_metod_dostawy",
                        principalColumn: "id_metody_dostawy");
                    table.ForeignKey(
                        name: "zamowienie_id_metody_platnosci_fkey",
                        column: x => x.id_metody_platnosci,
                        principalTable: "slownik_metod_platnosci",
                        principalColumn: "id_metody_platnosci");
                    table.ForeignKey(
                        name: "zamowienie_id_statusu_fkey",
                        column: x => x.id_statusu,
                        principalTable: "slownik_statusow",
                        principalColumn: "id_statusu");
                    table.ForeignKey(
                        name: "zamowienie_id_uzytkownika_fkey",
                        column: x => x.id_uzytkownika,
                        principalTable: "uzytkownik_details",
                        principalColumn: "id_uzytkownika");
                });

            migrationBuilder.CreateTable(
                name: "pozycja_koszyka",
                columns: table => new
                {
                    id_pozycji = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_koszyka = table.Column<int>(type: "integer", nullable: false),
                    id_produktu = table.Column<int>(type: "integer", nullable: false),
                    ilosc = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pozycja_koszyka_pkey", x => x.id_pozycji);
                    table.ForeignKey(
                        name: "pozycja_koszyka_id_koszyka_fkey",
                        column: x => x.id_koszyka,
                        principalTable: "koszyk",
                        principalColumn: "id_koszyka");
                    table.ForeignKey(
                        name: "pozycja_koszyka_id_produktu_fkey",
                        column: x => x.id_produktu,
                        principalTable: "produkt",
                        principalColumn: "id_produktu");
                });

            migrationBuilder.CreateTable(
                name: "pozycja_zamowienia",
                columns: table => new
                {
                    id_pozycji = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_zamowienia = table.Column<int>(type: "integer", nullable: false),
                    id_produktu = table.Column<int>(type: "integer", nullable: false),
                    ilosc = table.Column<int>(type: "integer", nullable: false),
                    cena_zakupu = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    nazwa_w_momencie_zakupu = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pozycja_zamowienia_pkey", x => x.id_pozycji);
                    table.ForeignKey(
                        name: "pozycja_zamowienia_id_produktu_fkey",
                        column: x => x.id_produktu,
                        principalTable: "produkt",
                        principalColumn: "id_produktu");
                    table.ForeignKey(
                        name: "pozycja_zamowienia_id_zamowienia_fkey",
                        column: x => x.id_zamowienia,
                        principalTable: "zamowienie",
                        principalColumn: "id_zamowienia");
                });

            migrationBuilder.CreateTable(
                name: "zwrot_zamowienia",
                columns: table => new
                {
                    id_zwrotu = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_zamowienia = table.Column<int>(type: "integer", nullable: false),
                    data_zgloszenia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    powod = table.Column<string>(type: "text", nullable: false),
                    czy_zaakceptowany = table.Column<bool>(type: "boolean", nullable: false),
                    id_typu_zgloszenia = table.Column<int>(type: "integer", nullable: false),
                    id_statusu_zgloszenia = table.Column<int>(type: "integer", nullable: false),
                    id_pozycji = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("zwrot_zamowienia_pkey", x => x.id_zwrotu);
                    table.ForeignKey(
                        name: "fk_zwrot_zamowienia_pozycja_zamowienia",
                        column: x => x.id_pozycji,
                        principalTable: "pozycja_zamowienia",
                        principalColumn: "id_pozycji",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_zwrot_zamowienia_slownik_status_zgloszenia",
                        column: x => x.id_statusu_zgloszenia,
                        principalTable: "StatusZgloszenia",
                        principalColumn: "IdStatusuZgloszenia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_zwrot_zamowienia_slownik_typ_zgloszenia",
                        column: x => x.id_typu_zgloszenia,
                        principalTable: "TypZgloszenia",
                        principalColumn: "IdTypuZgloszenia",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "zwrot_zamowienia_id_zamowienia_fkey",
                        column: x => x.id_zamowienia,
                        principalTable: "zamowienie",
                        principalColumn: "id_zamowienia");
                });

            migrationBuilder.CreateIndex(
                name: "IX_adres_id_uzytkownika",
                table: "adres",
                column: "id_uzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_kategoria_produkt_id_produktu",
                table: "kategoria_produkt",
                column: "id_produktu");

            migrationBuilder.CreateIndex(
                name: "koszyk_id_uzytkownika_key",
                table: "koszyk",
                column: "id_uzytkownika",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_opinia_produktu_id_produktu",
                table: "opinia_produktu",
                column: "id_produktu");

            migrationBuilder.CreateIndex(
                name: "IX_opinia_produktu_id_uzytkownika",
                table: "opinia_produktu",
                column: "id_uzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_pozycja_koszyka_id_koszyka",
                table: "pozycja_koszyka",
                column: "id_koszyka");

            migrationBuilder.CreateIndex(
                name: "IX_pozycja_koszyka_id_produktu",
                table: "pozycja_koszyka",
                column: "id_produktu");

            migrationBuilder.CreateIndex(
                name: "IX_pozycja_zamowienia_id_produktu",
                table: "pozycja_zamowienia",
                column: "id_produktu");

            migrationBuilder.CreateIndex(
                name: "IX_pozycja_zamowienia_id_zamowienia",
                table: "pozycja_zamowienia",
                column: "id_zamowienia");

            migrationBuilder.CreateIndex(
                name: "IX_produkt_id_autora",
                table: "produkt",
                column: "id_autora");

            migrationBuilder.CreateIndex(
                name: "tag_nazwa_tagu_key",
                table: "tag",
                column: "nazwa_tagu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tag_produkt_id_produktu",
                table: "tag_produkt",
                column: "id_produktu");

            migrationBuilder.CreateIndex(
                name: "IX_ulubione_produkty_id_produktu",
                table: "ulubione_produkty",
                column: "id_produktu");

            migrationBuilder.CreateIndex(
                name: "IX_uzytkownik_kod_promocyjny_id_uzytkownika",
                table: "uzytkownik_kod_promocyjny",
                column: "id_uzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_zadanie_powiadomienie_id_przypisanego_admina",
                table: "zadanie_powiadomienie",
                column: "id_przypisanego_admina");

            migrationBuilder.CreateIndex(
                name: "IX_zamowienie_id_adresu_zamowienia",
                table: "zamowienie",
                column: "id_adresu_zamowienia");

            migrationBuilder.CreateIndex(
                name: "IX_zamowienie_id_metody_dostawy",
                table: "zamowienie",
                column: "id_metody_dostawy");

            migrationBuilder.CreateIndex(
                name: "IX_zamowienie_id_metody_platnosci",
                table: "zamowienie",
                column: "id_metody_platnosci");

            migrationBuilder.CreateIndex(
                name: "IX_zamowienie_id_statusu",
                table: "zamowienie",
                column: "id_statusu");

            migrationBuilder.CreateIndex(
                name: "IX_zamowienie_id_uzytkownika",
                table: "zamowienie",
                column: "id_uzytkownika");

            migrationBuilder.CreateIndex(
                name: "IX_zapytanie_o_produkt_id_produktu",
                table: "zapytanie_o_produkt",
                column: "id_produktu");

            migrationBuilder.CreateIndex(
                name: "IX_zwrot_zamowienia_id_pozycji",
                table: "zwrot_zamowienia",
                column: "id_pozycji",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_zwrot_zamowienia_id_statusu_zgloszenia",
                table: "zwrot_zamowienia",
                column: "id_statusu_zgloszenia");

            migrationBuilder.CreateIndex(
                name: "IX_zwrot_zamowienia_id_typu_zgloszenia",
                table: "zwrot_zamowienia",
                column: "id_typu_zgloszenia");

            migrationBuilder.CreateIndex(
                name: "zwrot_zamowienia_id_zamowienia_key",
                table: "zwrot_zamowienia",
                column: "id_zamowienia",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adres_zamowienia");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "kategoria_produkt");

            migrationBuilder.DropTable(
                name: "opinia_produktu");

            migrationBuilder.DropTable(
                name: "pozycja_koszyka");

            migrationBuilder.DropTable(
                name: "tag_produkt");

            migrationBuilder.DropTable(
                name: "ulubione_produkty");

            migrationBuilder.DropTable(
                name: "uzytkownik_kod_promocyjny");

            migrationBuilder.DropTable(
                name: "zadanie_powiadomienie");

            migrationBuilder.DropTable(
                name: "zapytanie_o_produkt");

            migrationBuilder.DropTable(
                name: "zwrot_zamowienia");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "kategoria");

            migrationBuilder.DropTable(
                name: "koszyk");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "kod_promocyjny");

            migrationBuilder.DropTable(
                name: "pozycja_zamowienia");

            migrationBuilder.DropTable(
                name: "StatusZgloszenia");

            migrationBuilder.DropTable(
                name: "TypZgloszenia");

            migrationBuilder.DropTable(
                name: "produkt");

            migrationBuilder.DropTable(
                name: "zamowienie");

            migrationBuilder.DropTable(
                name: "autor");

            migrationBuilder.DropTable(
                name: "adres");

            migrationBuilder.DropTable(
                name: "slownik_metod_dostawy");

            migrationBuilder.DropTable(
                name: "slownik_metod_platnosci");

            migrationBuilder.DropTable(
                name: "slownik_statusow");

            migrationBuilder.DropTable(
                name: "uzytkownik_details");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
