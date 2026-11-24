using API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.LocalMigrations;

[DbContext(typeof(LocalAppDbContext))]
[Migration("20261124140000_SetBoardGameProductImages")]
public sealed class SetBoardGameProductImages : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            -- Product cover images are linked from their publisher/retailer image hosts.
            -- Source pages for the selected box images:
            -- Kolejka: https://www.krainazabawek24.pl/trefl-gra-kolejka-ipn-pl-01228.html
            -- Reglamentacja: https://www.ceneo.pl/34615237
            -- Neuroshima HEX 3.0: https://www.mediaexpert.pl/zabawki/gry-planszowe-i-puzzle/gry-planszowe/gra-planszowa-portal-games-neuroshima-hex-3-0
            -- Robinson Crusoe: https://www.rebel.pl/gry-planszowe/robinson-crusoe-przygoda-na-przekletej-wyspie-edycja-gra-roku-24428.html
            -- Detektyw: https://www.empik.com/detektyw-kryminalna-gra-planszowa-portal-games-portal-games,p1213065277,zabawki-p
            -- Nemesis: https://www.mediaexpert.pl/zabawki/gry-planszowe-i-puzzle/gry-planszowe/gra-planszowa-rebel-nemesis-edycja-polska
            -- Lords of Hellas: https://gamefound.com/en/creators/awaken-realms
            -- Tainted Grail: https://sklep.awakenrealms.com/tainted-grail---upadek-avalonu,60,53.html
            -- Wiedźmin: Stary Świat: https://www.planszomania.pl/przygodowe/22317/Wiedzmin-Stary-Swiat.html
            -- Frostpunk: https://www.rebel.pl/gry-planszowe/frostpunk-gra-planszowa-2001392.html
            UPDATE produkt AS p
            SET zdjecie = CASE p.nazwa_produktu
                WHEN 'Kolejka' THEN 'https://www.krainazabawek24.pl/media/products/419bc3a1c4c039c35aba95be384d2590/images/thumbnail/large_01228-0.webp?lm=1731433763'
                WHEN 'Reglamentacja. Gra na kartki' THEN 'https://image.ceneostatic.pl/data/products/34615237/4d41bbe1-670f-4e80-bce0-b509e44f6923_i-reglamentacja.jpg'
                WHEN 'Neuroshima HEX! 3.0' THEN 'https://www.mediaexpert.pl/media/cache/resolve/gallery/images/24/2479390/Gra-planszowa-PORTAL-GAMES-Neuroshima-Hex-3.0-front.jpg'
                WHEN 'Robinson Crusoe: Przygoda na Przeklętej Wyspie' THEN 'https://files.rebel.pl/products/100/1379/_24428/portal-games-kooperacyjna-robinson-crusoe-nowa-edycja-box--1200x900-ffffff.png'
                WHEN 'Detektyw: Kryminalna Gra Planszowa' THEN 'https://ecsmedia.pl/c/detektyw-kryminalna-gra-planszowa-portal-games-b-iext202840091.jpg'
                WHEN 'Nemesis' THEN 'https://www.mediaexpert.pl/media/cache/resolve/gallery/images/28/2861149/Gra-planszowa-REBEL-Nemesis-Edycja-Polska-pudelko.jpg'
                WHEN 'Lords of Hellas' THEN 'https://imgcdn.gamefound.com/projectimage/projects/5/84a22029-2674-48c3-b16e-6c9c6e1ce38f.jpg'
                WHEN 'Tainted Grail: Upadek Avalonu' THEN 'https://sklep.awakenrealms.com/gfx/1616155286.2723.jpg'
                WHEN 'Wiedźmin: Stary Świat' THEN 'https://www.planszomania.pl/image/22317-front.png'
                WHEN 'Frostpunk: The Board Game' THEN 'https://files.rebel.pl/products/100/302/_2001392/rebel-gra-strategiczna-frostpunk-box-3d-pl.png'
                ELSE p.zdjecie
            END
            WHERE (p.zdjecie IS NULL OR btrim(p.zdjecie) = '')
              AND p.nazwa_produktu IN (
                'Kolejka',
                'Reglamentacja. Gra na kartki',
                'Neuroshima HEX! 3.0',
                'Robinson Crusoe: Przygoda na Przeklętej Wyspie',
                'Detektyw: Kryminalna Gra Planszowa',
                'Nemesis',
                'Lords of Hellas',
                'Tainted Grail: Upadek Avalonu',
                'Wiedźmin: Stary Świat',
                'Frostpunk: The Board Game'
              );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            UPDATE produkt
            SET zdjecie = NULL
            WHERE (nazwa_produktu, zdjecie) IN (
                ('Kolejka', 'https://www.krainazabawek24.pl/media/products/419bc3a1c4c039c35aba95be384d2590/images/thumbnail/large_01228-0.webp?lm=1731433763'),
                ('Reglamentacja. Gra na kartki', 'https://image.ceneostatic.pl/data/products/34615237/4d41bbe1-670f-4e80-bce0-b509e44f6923_i-reglamentacja.jpg'),
                ('Neuroshima HEX! 3.0', 'https://www.mediaexpert.pl/media/cache/resolve/gallery/images/24/2479390/Gra-planszowa-PORTAL-GAMES-Neuroshima-Hex-3.0-front.jpg'),
                ('Robinson Crusoe: Przygoda na Przeklętej Wyspie', 'https://files.rebel.pl/products/100/1379/_24428/portal-games-kooperacyjna-robinson-crusoe-nowa-edycja-box--1200x900-ffffff.png'),
                ('Detektyw: Kryminalna Gra Planszowa', 'https://ecsmedia.pl/c/detektyw-kryminalna-gra-planszowa-portal-games-b-iext202840091.jpg'),
                ('Nemesis', 'https://www.mediaexpert.pl/media/cache/resolve/gallery/images/28/2861149/Gra-planszowa-REBEL-Nemesis-Edycja-Polska-pudelko.jpg'),
                ('Lords of Hellas', 'https://imgcdn.gamefound.com/projectimage/projects/5/84a22029-2674-48c3-b16e-6c9c6e1ce38f.jpg'),
                ('Tainted Grail: Upadek Avalonu', 'https://sklep.awakenrealms.com/gfx/1616155286.2723.jpg'),
                ('Wiedźmin: Stary Świat', 'https://www.planszomania.pl/image/22317-front.png'),
                ('Frostpunk: The Board Game', 'https://files.rebel.pl/products/100/302/_2001392/rebel-gra-strategiczna-frostpunk-box-3d-pl.png')
            );
            """);
    }
}
