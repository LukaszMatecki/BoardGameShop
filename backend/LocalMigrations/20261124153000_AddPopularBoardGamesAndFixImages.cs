using API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.LocalMigrations;

[DbContext(typeof(LocalAppDbContext))]
[Migration("20261124153000_AddPopularBoardGamesAndFixImages")]
public sealed class AddPopularBoardGamesAndFixImages : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            -- Tainted Grail's previous image host does not serve the image reliably.
            -- Replacement image source: https://www.jaskinia-graczy.pl/pl/products/gry-planszowe-i-towarzyskie/przygodowe/tainted-grail-upadek-avalonu-1621.html
            UPDATE produkt
            SET zdjecie = 'https://www.jaskinia-graczy.pl/hpeciai/b63861c09fb00eda60c63e5bb1570d81/pol_pl_TAINTED-GRAIL-UPADEK-AVALONU-1621_1.webp'
            WHERE nazwa_produktu = 'Tainted Grail: Upadek Avalonu';

            INSERT INTO autor (imie, nazwisko)
            SELECT wanted.imie, wanted.nazwisko
            FROM (VALUES
                ('Charles', 'Darrow'),
                ('Klaus', 'Teuber'),
                ('Jean-Louis', 'Roubira'),
                ('Michael', 'Kiesling'),
                ('Alan R.', 'Moon'),
                ('Vlaada', 'Chvátil'),
                ('Klaus-Jürgen', 'Wrede'),
                ('Antoine', 'Bauza'),
                ('Alfred Mosher', 'Butts'),
                ('Denis', 'Blanchot')
            ) AS wanted(imie, nazwisko)
            WHERE NOT EXISTS (
                SELECT 1 FROM autor a
                WHERE a.imie = wanted.imie AND a.nazwisko = wanted.nazwisko
            );

            INSERT INTO produkt
                (nazwa_produktu, id_autora, cena, "PromocjaWProc", "NajnizszaCenaW30Dni",
                 "DataNajnizszejCenyW30Dni", stan_magazynowy, data_wydania, opis, zdjecie, czy_ukryty)
            SELECT
                game.nazwa,
                (SELECT a.id_autora FROM autor a
                 WHERE a.imie = game.imie AND a.nazwisko = game.nazwisko
                 ORDER BY a.id_autora LIMIT 1),
                game.cena, 0, NULL, NULL, game.stan, game.wydanie, game.opis, game.zdjecie, FALSE
            FROM (VALUES
                ('Monopoly Classic', 'Charles', 'Darrow', 129.99, 10, DATE '1935-01-01',
                 'Klasyczna gra ekonomiczna o kupowaniu nieruchomości, pobieraniu czynszu i zarządzaniu pieniędzmi.',
                 'https://www.mediaexpert.pl/media/cache/gallery/images/74/7495778/HASBRO-Monopoly-Classic-Refresh-G0009-1.jpg'),
                ('Catan', 'Klaus', 'Teuber', 179.99, 8, DATE '1995-01-01',
                 'Rozwijaj osadę, zdobywaj surowce i handluj z innymi graczami na wyspie Catan.',
                 'https://www.eduksiegarnia.pl/img/imagecache/17001-18000/700x700/product-media/17001-18000/1/Catan-strategiczna-gra-planszowa-78502-700x700-nobckgr.jpg'),
                ('Dixit', 'Jean-Louis', 'Roubira', 149.99, 8, DATE '2008-01-01',
                 'Rodzinna gra skojarzeń i opowiadania historii za pomocą pięknie ilustrowanych kart.',
                 'https://abondegames.pl/wp-content/uploads/2023/10/rebel-gra-familijna-dixit-2021-box3d.png'),
                ('Azul', 'Michael', 'Kiesling', 129.99, 10, DATE '2017-01-01',
                 'Układaj kolorowe kafelki i twórz wzory inspirowane portugalskimi azulejos.',
                 'https://files.rebel.pl/products/100/606/_2012442/rebel-gra-rodzinna-azul-box3d.png'),
                ('Wsiąść do pociągu: Europa', 'Alan R.', 'Moon', 199.99, 7, DATE '2005-01-01',
                 'Buduj trasy kolejowe między europejskimi miastami i realizuj bilety podróżne.',
                 'https://bonito.pl/zdjecia/7/3e0a108-wsiasc-do-pociagu-eu.webp'),
                ('Tajniacy', 'Vlaada', 'Chvátil', 79.99, 12, DATE '2015-01-01',
                 'Dwie drużyny próbują odnaleźć swoich agentów, odgadując jednowyrazowe wskazówki.',
                 'https://files.rebel.pl/products/100/1203/_2027329/rebel-gra-imprezowa-tajniacy-2025-box3d-2000x2000-k-ffffff.png'),
                ('Carcassonne', 'Klaus-Jürgen', 'Wrede', 119.99, 9, DATE '2000-01-01',
                 'Dokładaj kafelki i rozwijaj średniowieczne miasta, drogi, klasztory oraz pola.',
                 'https://gryplanszowe.pl/hpeciai/2861b4a49b4b0e67a86d93868753109b/pol_pl_Carcassonne-nowa-edycja-2176_1.jpg'),
                ('7 Cudów Świata', 'Antoine', 'Bauza', 179.99, 7, DATE '2010-01-01',
                 'Rozwijaj jedną ze starożytnych cywilizacji i buduj jej cud w strategicznej grze karcianej.',
                 'https://flamberg.com.pl/hpeciai/c0962c26329c92d0e6f0b3c46fe072e8/pol_pl_7-cudow-swiata-nowa-edycja-61447_1.png'),
                ('Scrabble Original', 'Alfred Mosher', 'Butts', 99.99, 8, DATE '1948-01-01',
                 'Twórz krzyżujące się słowa z wylosowanych liter i zdobywaj punkty za pola premiowe.',
                 'https://www.toys4boys.pl/8260-superlarge_default/scrabble-original-wersja-polska.jpg'),
                ('Dobble', 'Denis', 'Blanchot', 59.99, 15, DATE '2009-01-01',
                 'Szybka gra na spostrzegawczość: znajdź jedyny wspólny symbol na dwóch kartach.',
                 'https://a.allegroimg.com/s720/117a6e/2feb8a4140dbbe7680a0399b3509/DOBBLE-Polska-Edycja')
            ) AS game(nazwa, imie, nazwisko, cena, stan, wydanie, opis, zdjecie)
            WHERE NOT EXISTS (
                SELECT 1 FROM produkt p WHERE p.nazwa_produktu = game.nazwa
            );

            INSERT INTO kategoria_produkt (id_kategorii, id_produktu)
            SELECT k.id_kategorii, p.id_produktu
            FROM (VALUES
                ('Monopoly Classic', 'Rodzinne'),
                ('Catan', 'Strategiczne'), ('Catan', 'Rodzinne'),
                ('Dixit', 'Rodzinne'),
                ('Azul', 'Strategiczne'), ('Azul', 'Rodzinne'),
                ('Wsiąść do pociągu: Europa', 'Strategiczne'), ('Wsiąść do pociągu: Europa', 'Rodzinne'),
                ('Tajniacy', 'Rodzinne'),
                ('Carcassonne', 'Strategiczne'), ('Carcassonne', 'Rodzinne'),
                ('7 Cudów Świata', 'Strategiczne'),
                ('Scrabble Original', 'Rodzinne'),
                ('Dobble', 'Rodzinne')
            ) AS wanted(nazwa_produktu, nazwa_kategorii)
            JOIN produkt p ON p.nazwa_produktu = wanted.nazwa_produktu
            JOIN kategoria k ON k.nazwa_kategorii = wanted.nazwa_kategorii
            WHERE NOT EXISTS (
                SELECT 1 FROM kategoria_produkt kp
                WHERE kp.id_kategorii = k.id_kategorii AND kp.id_produktu = p.id_produktu
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
                    'Monopoly Classic', 'Catan', 'Dixit', 'Azul', 'Wsiąść do pociągu: Europa',
                    'Tajniacy', 'Carcassonne', '7 Cudów Świata', 'Scrabble Original', 'Dobble'
                )
            );

            DELETE FROM produkt
            WHERE nazwa_produktu IN (
                'Monopoly Classic', 'Catan', 'Dixit', 'Azul', 'Wsiąść do pociągu: Europa',
                'Tajniacy', 'Carcassonne', '7 Cudów Świata', 'Scrabble Original', 'Dobble'
            );

            DELETE FROM autor a
            WHERE (a.imie, a.nazwisko) IN (
                ('Charles', 'Darrow'), ('Klaus', 'Teuber'), ('Jean-Louis', 'Roubira'),
                ('Michael', 'Kiesling'), ('Alan R.', 'Moon'), ('Vlaada', 'Chvátil'),
                ('Klaus-Jürgen', 'Wrede'), ('Antoine', 'Bauza'),
                ('Alfred Mosher', 'Butts'), ('Denis', 'Blanchot')
            )
            AND NOT EXISTS (SELECT 1 FROM produkt p WHERE p.id_autora = a.id_autora);

            UPDATE produkt
            SET zdjecie = 'https://sklep.awakenrealms.com/gfx/1616155286.2723.jpg'
            WHERE nazwa_produktu = 'Tainted Grail: Upadek Avalonu'
              AND zdjecie = 'https://www.jaskinia-graczy.pl/hpeciai/b63861c09fb00eda60c63e5bb1570d81/pol_pl_TAINTED-GRAIL-UPADEK-AVALONU-1621_1.webp';
            """);
    }
}
