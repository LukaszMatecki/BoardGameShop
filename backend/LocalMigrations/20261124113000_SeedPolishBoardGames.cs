using API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.LocalMigrations;

[DbContext(typeof(LocalAppDbContext))]
[Migration("20261124113000_SeedPolishBoardGames")]
public sealed class SeedPolishBoardGames : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            INSERT INTO autor (imie, nazwisko)
            SELECT wanted.imie, wanted.nazwisko
            FROM (VALUES
                ('Karol', 'Madaj'),
                ('Michał', 'Oracz'),
                ('Ignacy', 'Trzewiczek'),
                ('Adam', 'Kwapiński'),
                ('Krzysztof', 'Piskorski'),
                ('Łukasz', 'Woźniak')
            ) AS wanted(imie, nazwisko)
            WHERE NOT EXISTS (
                SELECT 1 FROM autor a
                WHERE a.imie = wanted.imie AND a.nazwisko = wanted.nazwisko
            );

            INSERT INTO kategoria (nazwa_kategorii)
            SELECT wanted.nazwa
            FROM (VALUES
                ('Strategiczne'),
                ('Kooperacyjne'),
                ('Przygodowe'),
                ('Historyczne'),
                ('Rodzinne')
            ) AS wanted(nazwa)
            WHERE NOT EXISTS (
                SELECT 1 FROM kategoria k WHERE k.nazwa_kategorii = wanted.nazwa
            );

            INSERT INTO tag (nazwa_tagu)
            SELECT wanted.nazwa
            FROM (VALUES
                ('Polski projektant'),
                ('Polski wydawca'),
                ('PRL'),
                ('Postapokalipsa'),
                ('Przetrwanie'),
                ('Dedukcja'),
                ('Science fiction'),
                ('Fantasy')
            ) AS wanted(nazwa)
            WHERE NOT EXISTS (
                SELECT 1 FROM tag t WHERE t.nazwa_tagu = wanted.nazwa
            );

            INSERT INTO produkt
                (nazwa_produktu, id_autora, cena, "PromocjaWProc", "NajnizszaCenaW30Dni",
                 "DataNajnizszejCenyW30Dni", stan_magazynowy, data_wydania, opis, zdjecie, czy_ukryty)
            SELECT
                game.nazwa,
                (SELECT a.id_autora FROM autor a
                 WHERE a.imie = game.imie AND a.nazwisko = game.nazwisko
                 ORDER BY a.id_autora LIMIT 1),
                game.cena, 0, NULL, NULL, game.stan, game.wydanie, game.opis, NULL, FALSE
            FROM (VALUES
                ('Kolejka', 'Karol', 'Madaj', 109.99, 12, DATE '2011-01-01',
                 'Gra historyczna o codzienności w PRL-u: gracze próbują zdobyć towary, zanim znikną z półek.'),
                ('Reglamentacja. Gra na kartki', 'Karol', 'Madaj', 89.99, 8, DATE '2014-01-01',
                 'Szybka gra negocjacyjna osadzona w realiach reglamentacji towarów w PRL-u.'),
                ('Neuroshima HEX! 3.0', 'Michał', 'Oracz', 129.99, 10, DATE '2005-01-01',
                 'Taktyczna potyczka armii w postapokaliptycznym świecie Neuroshimy.'),
                ('Robinson Crusoe: Przygoda na Przeklętej Wyspie', 'Ignacy', 'Trzewiczek', 199.99, 7, DATE '2012-01-01',
                 'Kooperacyjna przygoda o przetrwaniu na wyspie, eksploracji i wspólnym podejmowaniu decyzji.'),
                ('Detektyw: Kryminalna Gra Planszowa', 'Ignacy', 'Trzewiczek', 159.99, 6, DATE '2018-01-01',
                 'Kooperacyjne śledztwo, w którym gracze łączą wskazówki i analizują sprawy kryminalne.'),
                ('Nemesis', 'Adam', 'Kwapiński', 499.99, 4, DATE '2018-01-01',
                 'Survival science fiction: załoga statku kosmicznego próbuje przetrwać wśród intruzów.'),
                ('Lords of Hellas', 'Adam', 'Kwapiński', 349.99, 4, DATE '2018-01-01',
                 'Strategiczna rywalizacja w świecie inspirowanym mitologią starożytnej Grecji.'),
                ('Tainted Grail: Upadek Avalonu', 'Krzysztof', 'Piskorski', 449.99, 3, DATE '2019-01-01',
                 'Kooperacyjna kampania fantasy z eksploracją, wyborami i rozbudowaną historią.'),
                ('Wiedźmin: Stary Świat', 'Łukasz', 'Woźniak', 329.99, 5, DATE '2022-01-01',
                 'Przygoda w świecie Wiedźmina: młodzi wiedźmini podróżują po Kontynencie i polują na potwory.'),
                ('Frostpunk: The Board Game', 'Adam', 'Kwapiński', 399.99, 3, DATE '2020-01-01',
                 'Kooperacyjne zarządzanie osadą w skutej lodem krainie i trudne decyzje o losie mieszkańców.')
            ) AS game(nazwa, imie, nazwisko, cena, stan, wydanie, opis)
            WHERE NOT EXISTS (
                SELECT 1 FROM produkt p WHERE p.nazwa_produktu = game.nazwa
            );

            INSERT INTO kategoria_produkt (id_kategorii, id_produktu)
            SELECT k.id_kategorii, p.id_produktu
            FROM (VALUES
                ('Kolejka', 'Historyczne'), ('Kolejka', 'Rodzinne'),
                ('Reglamentacja. Gra na kartki', 'Historyczne'), ('Reglamentacja. Gra na kartki', 'Strategiczne'),
                ('Neuroshima HEX! 3.0', 'Strategiczne'),
                ('Robinson Crusoe: Przygoda na Przeklętej Wyspie', 'Kooperacyjne'),
                ('Robinson Crusoe: Przygoda na Przeklętej Wyspie', 'Przygodowe'),
                ('Detektyw: Kryminalna Gra Planszowa', 'Kooperacyjne'),
                ('Detektyw: Kryminalna Gra Planszowa', 'Przygodowe'),
                ('Nemesis', 'Kooperacyjne'), ('Nemesis', 'Przygodowe'),
                ('Lords of Hellas', 'Strategiczne'), ('Lords of Hellas', 'Przygodowe'),
                ('Tainted Grail: Upadek Avalonu', 'Kooperacyjne'), ('Tainted Grail: Upadek Avalonu', 'Przygodowe'),
                ('Wiedźmin: Stary Świat', 'Przygodowe'), ('Wiedźmin: Stary Świat', 'Strategiczne'),
                ('Frostpunk: The Board Game', 'Kooperacyjne'), ('Frostpunk: The Board Game', 'Strategiczne')
            ) AS wanted(nazwa_produktu, nazwa_kategorii)
            JOIN produkt p ON p.nazwa_produktu = wanted.nazwa_produktu
            JOIN kategoria k ON k.nazwa_kategorii = wanted.nazwa_kategorii
            WHERE NOT EXISTS (
                SELECT 1 FROM kategoria_produkt kp
                WHERE kp.id_kategorii = k.id_kategorii AND kp.id_produktu = p.id_produktu
            );

            INSERT INTO tag_produkt (id_tagu, id_produktu)
            SELECT t.id_tagu, p.id_produktu
            FROM (VALUES
                ('Kolejka', 'Polski projektant'), ('Kolejka', 'Polski wydawca'), ('Kolejka', 'PRL'),
                ('Reglamentacja. Gra na kartki', 'Polski projektant'), ('Reglamentacja. Gra na kartki', 'Polski wydawca'), ('Reglamentacja. Gra na kartki', 'PRL'),
                ('Neuroshima HEX! 3.0', 'Polski projektant'), ('Neuroshima HEX! 3.0', 'Polski wydawca'), ('Neuroshima HEX! 3.0', 'Postapokalipsa'),
                ('Robinson Crusoe: Przygoda na Przeklętej Wyspie', 'Polski projektant'), ('Robinson Crusoe: Przygoda na Przeklętej Wyspie', 'Polski wydawca'), ('Robinson Crusoe: Przygoda na Przeklętej Wyspie', 'Przetrwanie'),
                ('Detektyw: Kryminalna Gra Planszowa', 'Polski projektant'), ('Detektyw: Kryminalna Gra Planszowa', 'Polski wydawca'), ('Detektyw: Kryminalna Gra Planszowa', 'Dedukcja'),
                ('Nemesis', 'Polski projektant'), ('Nemesis', 'Polski wydawca'), ('Nemesis', 'Science fiction'), ('Nemesis', 'Przetrwanie'),
                ('Lords of Hellas', 'Polski projektant'), ('Lords of Hellas', 'Polski wydawca'), ('Lords of Hellas', 'Fantasy'),
                ('Tainted Grail: Upadek Avalonu', 'Polski projektant'), ('Tainted Grail: Upadek Avalonu', 'Polski wydawca'), ('Tainted Grail: Upadek Avalonu', 'Fantasy'),
                ('Wiedźmin: Stary Świat', 'Polski projektant'), ('Wiedźmin: Stary Świat', 'Polski wydawca'), ('Wiedźmin: Stary Świat', 'Fantasy'),
                ('Frostpunk: The Board Game', 'Polski projektant'), ('Frostpunk: The Board Game', 'Polski wydawca'), ('Frostpunk: The Board Game', 'Science fiction'), ('Frostpunk: The Board Game', 'Przetrwanie')
            ) AS wanted(nazwa_produktu, nazwa_tagu)
            JOIN produkt p ON p.nazwa_produktu = wanted.nazwa_produktu
            JOIN tag t ON t.nazwa_tagu = wanted.nazwa_tagu
            WHERE NOT EXISTS (
                SELECT 1 FROM tag_produkt tp
                WHERE tp.id_tagu = t.id_tagu AND tp.id_produktu = p.id_produktu
            );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            DELETE FROM kategoria_produkt
            WHERE id_produktu IN (
                SELECT id_produktu FROM produkt
                WHERE nazwa_produktu IN (
                    'Kolejka', 'Reglamentacja. Gra na kartki', 'Neuroshima HEX! 3.0',
                    'Robinson Crusoe: Przygoda na Przeklętej Wyspie',
                    'Detektyw: Kryminalna Gra Planszowa', 'Nemesis', 'Lords of Hellas',
                    'Tainted Grail: Upadek Avalonu', 'Wiedźmin: Stary Świat',
                    'Frostpunk: The Board Game'
                )
            );
            DELETE FROM tag_produkt
            WHERE id_produktu IN (
                SELECT id_produktu FROM produkt
                WHERE nazwa_produktu IN (
                    'Kolejka', 'Reglamentacja. Gra na kartki', 'Neuroshima HEX! 3.0',
                    'Robinson Crusoe: Przygoda na Przeklętej Wyspie',
                    'Detektyw: Kryminalna Gra Planszowa', 'Nemesis', 'Lords of Hellas',
                    'Tainted Grail: Upadek Avalonu', 'Wiedźmin: Stary Świat',
                    'Frostpunk: The Board Game'
                )
            );
            DELETE FROM produkt
            WHERE nazwa_produktu IN (
                'Kolejka', 'Reglamentacja. Gra na kartki', 'Neuroshima HEX! 3.0',
                'Robinson Crusoe: Przygoda na Przeklętej Wyspie',
                'Detektyw: Kryminalna Gra Planszowa', 'Nemesis', 'Lords of Hellas',
                'Tainted Grail: Upadek Avalonu', 'Wiedźmin: Stary Świat',
                'Frostpunk: The Board Game'
            );
            DELETE FROM autor a
            WHERE (a.imie, a.nazwisko) IN (
                ('Karol', 'Madaj'), ('Michał', 'Oracz'), ('Ignacy', 'Trzewiczek'),
                ('Adam', 'Kwapiński'), ('Krzysztof', 'Piskorski'), ('Łukasz', 'Woźniak')
            )
            AND NOT EXISTS (SELECT 1 FROM produkt p WHERE p.id_autora = a.id_autora);
            DELETE FROM kategoria k
            WHERE k.nazwa_kategorii IN ('Strategiczne', 'Kooperacyjne', 'Przygodowe', 'Historyczne', 'Rodzinne')
              AND NOT EXISTS (
                  SELECT 1 FROM kategoria_produkt kp WHERE kp.id_kategorii = k.id_kategorii
              );
            DELETE FROM tag t
            WHERE t.nazwa_tagu IN (
                'Polski projektant', 'Polski wydawca', 'PRL', 'Postapokalipsa',
                'Przetrwanie', 'Dedukcja', 'Science fiction', 'Fantasy'
            )
              AND NOT EXISTS (SELECT 1 FROM tag_produkt tp WHERE tp.id_tagu = t.id_tagu);
            """);
    }
}