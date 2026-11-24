using API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.LocalMigrations;

[DbContext(typeof(LocalAppDbContext))]
[Migration("20261124170000_SeedOrderLookups")]
public sealed class SeedOrderLookups : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            INSERT INTO slownik_metod_dostawy (id_metody_dostawy, nazwa_metody, koszt_bazowy)
            VALUES
                (1, 'Standard Courier', 15.00),
                (2, 'Express Shipping', 25.50),
                (3, 'Local Pickup', 0.00)
            ON CONFLICT (id_metody_dostawy) DO UPDATE
            SET nazwa_metody = EXCLUDED.nazwa_metody,
                koszt_bazowy = EXCLUDED.koszt_bazowy;

            INSERT INTO slownik_metod_platnosci (id_metody_platnosci, nazwa_platnosci)
            VALUES
                (1, 'BLIK'),
                (2, 'Credit Card'),
                (3, 'Bank Transfer')
            ON CONFLICT (id_metody_platnosci) DO UPDATE
            SET nazwa_platnosci = EXCLUDED.nazwa_platnosci;

            INSERT INTO slownik_statusow (id_statusu, nazwa_statusu)
            VALUES (1, 'Pending')
            ON CONFLICT (id_statusu) DO NOTHING;

            SELECT setval(
                pg_get_serial_sequence('slownik_metod_dostawy', 'id_metody_dostawy'),
                COALESCE(MAX(id_metody_dostawy), 1),
                MAX(id_metody_dostawy) IS NOT NULL
            ) FROM slownik_metod_dostawy;

            SELECT setval(
                pg_get_serial_sequence('slownik_metod_platnosci', 'id_metody_platnosci'),
                COALESCE(MAX(id_metody_platnosci), 1),
                MAX(id_metody_platnosci) IS NOT NULL
            ) FROM slownik_metod_platnosci;

            SELECT setval(
                pg_get_serial_sequence('slownik_statusow', 'id_statusu'),
                COALESCE(MAX(id_statusu), 1),
                MAX(id_statusu) IS NOT NULL
            ) FROM slownik_statusow;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM slownik_statusow s
            WHERE s.id_statusu = 1
              AND s.nazwa_statusu = 'Pending'
              AND NOT EXISTS (SELECT 1 FROM zamowienie z WHERE z.id_statusu = s.id_statusu);

            DELETE FROM slownik_metod_platnosci m
            WHERE m.id_metody_platnosci IN (1, 2, 3)
              AND NOT EXISTS (SELECT 1 FROM zamowienie z WHERE z.id_metody_platnosci = m.id_metody_platnosci);

            DELETE FROM slownik_metod_dostawy m
            WHERE m.id_metody_dostawy IN (1, 2, 3)
              AND NOT EXISTS (SELECT 1 FROM zamowienie z WHERE z.id_metody_dostawy = m.id_metody_dostawy);
            """);
    }
}
