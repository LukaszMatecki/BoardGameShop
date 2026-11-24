using System;
using System.Collections.Generic;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public partial class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected AppDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public virtual DbSet<Adre> Adres { get; set; }

    public virtual DbSet<AdresZamowienium> AdresZamowienia { get; set; }

    public virtual DbSet<Autor> Autors { get; set; }

    public virtual DbSet<Kategorium> Kategoria { get; set; }
    public virtual DbSet<KodPromocyjny> KodyPromocyjne { get; set; }

    public virtual DbSet<Koszyk> Koszyks { get; set; }

    public virtual DbSet<OpiniaProduktu> OpiniaProduktus { get; set; }

    public virtual DbSet<PozycjaKoszyka> PozycjaKoszykas { get; set; }

    public virtual DbSet<PozycjaZamowienium> PozycjaZamowienia { get; set; }

    public virtual DbSet<Produkt> Produkts { get; set; }

    public virtual DbSet<SlownikMetodDostawy> SlownikMetodDostawies { get; set; }

    public virtual DbSet<SlownikMetodPlatnosci> SlownikMetodPlatnoscis { get; set; }

    public virtual DbSet<SlownikStatusow> SlownikStatusows { get; set; }
    public DbSet<SlownikStatusZgloszenia> StatusZgloszenia { get; set; }
    public DbSet<SlownikTypZgloszenia> TypZgloszenia { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<UzytkownikDetail> UzytkownikDetails { get; set; }

    public virtual DbSet<ZadaniePowiadomienie> ZadaniePowiadomienies { get; set; }

    public virtual DbSet<Zamowienie> Zamowienies { get; set; }

    public virtual DbSet<ZapytanieOProdukt> ZapytanieOProdukts { get; set; }

    public virtual DbSet<ZwrotZamowienium> ZwrotZamowienia { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Adre>(entity =>
        {
            entity.HasKey(e => e.IdAdresu).HasName("adres_pkey");

            entity.ToTable("adres");

            entity.Property(e => e.IdAdresu).HasColumnName("id_adresu");
            entity.Property(e => e.IdUzytkownika).HasColumnName("id_uzytkownika");
            entity.Property(e => e.KodPocztowy)
                .HasMaxLength(20)
                .HasColumnName("kod_pocztowy");
            entity.Property(e => e.Miasto)
                .HasMaxLength(255)
                .HasColumnName("miasto");
            entity.Property(e => e.NumerBudynku)
                .HasMaxLength(50)
                .HasColumnName("numer_budynku");
            entity.Property(e => e.NumerLokalu)
                .HasMaxLength(50)
                .HasColumnName("numer_lokalu");
            entity.Property(e => e.Ulica)
                .HasMaxLength(255)
                .HasColumnName("ulica");

            entity.HasOne(d => d.IdUzytkownikaNavigation).WithMany(p => p.Adres)
                .HasForeignKey(d => d.IdUzytkownika)
                .HasConstraintName("adres_id_uzytkownika_fkey");
        });

        modelBuilder.Entity<AdresZamowienium>(entity =>
        {
            entity.HasKey(e => e.IdAdresuZamowienia).HasName("adres_zamowienia_pkey");

            entity.ToTable("adres_zamowienia");

            entity.Property(e => e.IdAdresuZamowienia).HasColumnName("id_adresu_zamowienia");
            entity.Property(e => e.ImieOdbiorcy)
                .HasMaxLength(255)
                .HasColumnName("imie_odbiorcy");
            entity.Property(e => e.KodPocztowy)
                .HasMaxLength(20)
                .HasColumnName("kod_pocztowy");
            entity.Property(e => e.Miasto)
                .HasMaxLength(255)
                .HasColumnName("miasto");
            entity.Property(e => e.NazwiskoOdbiorcy)
                .HasMaxLength(255)
                .HasColumnName("nazwisko_odbiorcy");
            entity.Property(e => e.NumerBudynku)
                .HasMaxLength(50)
                .HasColumnName("numer_budynku");
            entity.Property(e => e.NumerLokalu)
                .HasMaxLength(50)
                .HasColumnName("numer_lokalu");
            entity.Property(e => e.Ulica)
                .HasMaxLength(255)
                .HasColumnName("ulica");
        });

        modelBuilder.Entity<Autor>(entity =>
        {
            entity.HasKey(e => e.IdAutora).HasName("autor_pkey");

            entity.ToTable("autor");

            entity.Property(e => e.IdAutora).HasColumnName("id_autora");
            entity.Property(e => e.Imie)
                .HasMaxLength(255)
                .HasColumnName("imie");
            entity.Property(e => e.Nazwisko)
                .HasMaxLength(255)
                .HasColumnName("nazwisko");
        });

        modelBuilder.Entity<Kategorium>(entity =>
        {
            entity.HasKey(e => e.IdKategorii).HasName("kategoria_pkey");

            entity.ToTable("kategoria");

            entity.Property(e => e.IdKategorii).HasColumnName("id_kategorii");
            entity.Property(e => e.NazwaKategorii)
                .HasMaxLength(255)
                .HasColumnName("nazwa_kategorii");

            entity.HasMany(d => d.IdProduktus).WithMany(p => p.IdKategoriis)
                .UsingEntity<Dictionary<string, object>>(
                    "KategoriaProdukt",
                    r => r.HasOne<Produkt>().WithMany()
                        .HasForeignKey("IdProduktu")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("kategoria_produkt_id_produktu_fkey"),
                    l => l.HasOne<Kategorium>().WithMany()
                        .HasForeignKey("IdKategorii")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("kategoria_produkt_id_kategorii_fkey"),
                    j =>
                    {
                        j.HasKey("IdKategorii", "IdProduktu").HasName("kategoria_produkt_pkey");
                        j.ToTable("kategoria_produkt");
                        j.IndexerProperty<int>("IdKategorii").HasColumnName("id_kategorii");
                        j.IndexerProperty<int>("IdProduktu").HasColumnName("id_produktu");
                    });
        });
        modelBuilder.Entity<KodPromocyjny>(entity =>
        {
            entity.HasKey(e => e.IdKodu).HasName("kod_promocyjny_pkey");

            entity.ToTable("kod_promocyjny");

            entity.Property(e => e.IdKodu).HasColumnName("id_kodu");

            entity.Property(e => e.Kod)
                .HasMaxLength(50)
                .HasColumnName("kod");

            entity.Property(e => e.ZnizkaProcentowa)
                .HasColumnName("znizka_procentowa");

            entity.HasMany(d => d.Uzytkownicy).WithMany(p => p.KodyPromocyjne)
                .UsingEntity<Dictionary<string, object>>(
                    "UzytkownikKodPromocyjny",
                    r => r.HasOne<UzytkownikDetail>().WithMany()
                        .HasForeignKey("IdUzytkownika")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("uzytkownik_kod_promocyjny_id_uzytkownika_fkey"),
                    l => l.HasOne<KodPromocyjny>().WithMany()
                        .HasForeignKey("IdKodu")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("uzytkownik_kod_promocyjny_id_kodu_fkey"),
                    j =>
                    {
                        j.HasKey("IdKodu", "IdUzytkownika").HasName("uzytkownik_kod_promocyjny_pkey");
                        j.ToTable("uzytkownik_kod_promocyjny");
                        j.IndexerProperty<int>("IdKodu").HasColumnName("id_kodu");
                        j.IndexerProperty<int>("IdUzytkownika").HasColumnName("id_uzytkownika");
                    });
        });
        modelBuilder.Entity<Koszyk>(entity =>
        {
            entity.HasKey(e => e.IdKoszyka).HasName("koszyk_pkey");

            entity.ToTable("koszyk");

            entity.HasIndex(e => e.IdUzytkownika, "koszyk_id_uzytkownika_key").IsUnique();

            entity.Property(e => e.IdKoszyka).HasColumnName("id_koszyka");
            entity.Property(e => e.DataOstatniejModyfikacji)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_ostatniej_modyfikacji");
            entity.Property(e => e.IdUzytkownika).HasColumnName("id_uzytkownika");

            entity.HasOne(d => d.IdUzytkownikaNavigation).WithOne(p => p.Koszyk)
                .HasForeignKey<Koszyk>(d => d.IdUzytkownika)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("koszyk_id_uzytkownika_fkey");
        });

        modelBuilder.Entity<OpiniaProduktu>(entity =>
        {
            entity.HasKey(e => e.IdOpinii).HasName("opinia_produktu_pkey");

            entity.ToTable("opinia_produktu");

            entity.Property(e => e.IdOpinii).HasColumnName("id_opinii");
            entity.Property(e => e.CzyZatwierdzona).HasColumnName("czy_zatwierdzona");
            entity.Property(e => e.DataWystawienia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_wystawienia");
            entity.Property(e => e.IdProduktu).HasColumnName("id_produktu");
            entity.Property(e => e.IdUzytkownika).HasColumnName("id_uzytkownika");
            entity.Property(e => e.Komentarz).HasColumnName("komentarz");
            entity.Property(e => e.Ocena).HasColumnName("ocena");

            entity.HasOne(d => d.IdProduktuNavigation).WithMany(p => p.OpiniaProduktus)
                .HasForeignKey(d => d.IdProduktu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("opinia_produktu_id_produktu_fkey");

            entity.HasOne(d => d.IdUzytkownikaNavigation).WithMany(p => p.OpiniaProduktus)
                .HasForeignKey(d => d.IdUzytkownika)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("opinia_produktu_id_uzytkownika_fkey");
        });

        modelBuilder.Entity<PozycjaKoszyka>(entity =>
        {
            entity.HasKey(e => e.IdPozycji).HasName("pozycja_koszyka_pkey");

            entity.ToTable("pozycja_koszyka");

            entity.Property(e => e.IdPozycji).HasColumnName("id_pozycji");
            entity.Property(e => e.IdKoszyka).HasColumnName("id_koszyka");
            entity.Property(e => e.IdProduktu).HasColumnName("id_produktu");
            entity.Property(e => e.Ilosc).HasColumnName("ilosc");

            entity.HasOne(d => d.IdKoszykaNavigation).WithMany(p => p.PozycjaKoszykas)
                .HasForeignKey(d => d.IdKoszyka)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pozycja_koszyka_id_koszyka_fkey");

            entity.HasOne(d => d.IdProduktuNavigation).WithMany(p => p.PozycjaKoszykas)
                .HasForeignKey(d => d.IdProduktu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pozycja_koszyka_id_produktu_fkey");
        });

        modelBuilder.Entity<PozycjaZamowienium>(entity =>
        {
            entity.HasKey(e => e.IdPozycji).HasName("pozycja_zamowienia_pkey");

            entity.ToTable("pozycja_zamowienia");

            entity.Property(e => e.IdPozycji).HasColumnName("id_pozycji");
            entity.Property(e => e.CenaZakupu)
                .HasPrecision(10, 2)
                .HasColumnName("cena_zakupu");
            entity.Property(e => e.IdProduktu).HasColumnName("id_produktu");
            entity.Property(e => e.IdZamowienia).HasColumnName("id_zamowienia");
            entity.Property(e => e.Ilosc).HasColumnName("ilosc");
            entity.Property(e => e.NazwaWMomencieZakupu)
                .HasMaxLength(255)
                .HasColumnName("nazwa_w_momencie_zakupu");

            entity.HasOne(d => d.IdProduktuNavigation).WithMany(p => p.PozycjaZamowienia)
                .HasForeignKey(d => d.IdProduktu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pozycja_zamowienia_id_produktu_fkey");

            entity.HasOne(d => d.IdZamowieniaNavigation).WithMany(p => p.PozycjaZamowienia)
                .HasForeignKey(d => d.IdZamowienia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pozycja_zamowienia_id_zamowienia_fkey");
        });

        modelBuilder.Entity<Produkt>(entity =>
        {
            entity.HasKey(e => e.IdProduktu).HasName("produkt_pkey");

            entity.ToTable("produkt");

            entity.Property(e => e.IdProduktu).HasColumnName("id_produktu");
            entity.Property(e => e.Cena)
                .HasPrecision(10, 2)
                .HasColumnName("cena");
            entity.Property(e => e.CzyUkryty).HasColumnName("czy_ukryty");
            entity.Property(e => e.DataWydania).HasColumnName("data_wydania");
            entity.Property(e => e.IdAutora).HasColumnName("id_autora");
            entity.Property(e => e.NazwaProduktu)
                .HasMaxLength(255)
                .HasColumnName("nazwa_produktu");
            entity.Property(e => e.Opis).HasColumnName("opis");
            entity.Property(e => e.Zdjecie).HasColumnName("zdjecie");
            entity.Property(e => e.StanMagazynowy).HasColumnName("stan_magazynowy");

            entity.HasOne(d => d.IdAutoraNavigation).WithMany(p => p.Produkts)
                .HasForeignKey(d => d.IdAutora)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("produkt_id_autora_fkey");
        });

        modelBuilder.Entity<SlownikMetodDostawy>(entity =>
        {
            entity.HasKey(e => e.IdMetodyDostawy).HasName("slownik_metod_dostawy_pkey");

            entity.ToTable("slownik_metod_dostawy");

            entity.Property(e => e.IdMetodyDostawy).HasColumnName("id_metody_dostawy");
            entity.Property(e => e.KosztBazowy)
                .HasPrecision(10, 2)
                .HasColumnName("koszt_bazowy");
            entity.Property(e => e.NazwaMetody)
                .HasMaxLength(255)
                .HasColumnName("nazwa_metody");
        });

        modelBuilder.Entity<SlownikMetodPlatnosci>(entity =>
        {
            entity.HasKey(e => e.IdMetodyPlatnosci).HasName("slownik_metod_platnosci_pkey");

            entity.ToTable("slownik_metod_platnosci");

            entity.Property(e => e.IdMetodyPlatnosci).HasColumnName("id_metody_platnosci");
            entity.Property(e => e.NazwaPlatnosci)
                .HasMaxLength(255)
                .HasColumnName("nazwa_platnosci");
        });

        modelBuilder.Entity<SlownikStatusow>(entity =>
        {
            entity.HasKey(e => e.IdStatusu).HasName("slownik_statusow_pkey");

            entity.ToTable("slownik_statusow");

            entity.Property(e => e.IdStatusu).HasColumnName("id_statusu");
            entity.Property(e => e.NazwaStatusu)
                .HasMaxLength(100)
                .HasColumnName("nazwa_statusu");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.IdTagu).HasName("tag_pkey");

            entity.ToTable("tag");

            entity.HasIndex(e => e.NazwaTagu, "tag_nazwa_tagu_key").IsUnique();

            entity.Property(e => e.IdTagu).HasColumnName("id_tagu");
            entity.Property(e => e.NazwaTagu)
                .HasMaxLength(100)
                .HasColumnName("nazwa_tagu");

            entity.HasMany(d => d.IdProduktus).WithMany(p => p.IdTagus)
                .UsingEntity<Dictionary<string, object>>(
                    "TagProdukt",
                    r => r.HasOne<Produkt>().WithMany()
                        .HasForeignKey("IdProduktu")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tag_produkt_id_produktu_fkey"),
                    l => l.HasOne<Tag>().WithMany()
                        .HasForeignKey("IdTagu")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tag_produkt_id_tagu_fkey"),
                    j =>
                    {
                        j.HasKey("IdTagu", "IdProduktu").HasName("tag_produkt_pkey");
                        j.ToTable("tag_produkt");
                        j.IndexerProperty<int>("IdTagu").HasColumnName("id_tagu");
                        j.IndexerProperty<int>("IdProduktu").HasColumnName("id_produktu");
                    });
        });

        modelBuilder.Entity<UzytkownikDetail>(entity =>
        {
            entity.HasKey(e => e.IdUzytkownika)
                .HasName("uzytkownik_details_pkey");

            entity.ToTable("uzytkownik_details");

            entity.Property(e => e.IdUzytkownika)
                .HasColumnName("id_uzytkownika");

            entity.Property(e => e.CzyUsuniety)
                .HasColumnName("czy_usuniety");

            entity.Property(e => e.DataRejestracji)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_rejestracji");

            entity.HasOne(e => e.User)
                .WithOne(u => u.Details)
                .HasForeignKey<UzytkownikDetail>(e => e.IdUzytkownika)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.IdProduktus)
                .WithMany(p => p.IdUzytkownikas)
                .UsingEntity<Dictionary<string, object>>(
                    "UlubioneProdukty",
                    r => r.HasOne<Produkt>()
                        .WithMany()
                        .HasForeignKey("IdProduktu")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("ulubione_produkty_id_produktu_fkey"),
                    l => l.HasOne<UzytkownikDetail>()
                        .WithMany()
                        .HasForeignKey("IdUzytkownika")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("ulubione_produkty_id_uzytkownika_fkey"),
                    j =>
                    {
                        j.HasKey("IdUzytkownika", "IdProduktu")
                            .HasName("ulubione_produkty_pkey");

                        j.ToTable("ulubione_produkty");

                        j.IndexerProperty<int>("IdUzytkownika")
                            .HasColumnName("id_uzytkownika");

                        j.IndexerProperty<int>("IdProduktu")
                            .HasColumnName("id_produktu");
                    });
        });

        modelBuilder.Entity<ZadaniePowiadomienie>(entity =>
        {
            entity.HasKey(e => e.IdZadania).HasName("zadanie_powiadomienie_pkey");

            entity.ToTable("zadanie_powiadomienie");

            entity.Property(e => e.IdZadania).HasColumnName("id_zadania");
            entity.Property(e => e.DataUtworzenia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_utworzenia");
            entity.Property(e => e.IdPrzypisanegoAdmina).HasColumnName("id_przypisanego_admina");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'DO_ZROBIENIA'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Tresc).HasColumnName("tresc");
            entity.Property(e => e.Tytul)
                .HasMaxLength(255)
                .HasColumnName("tytul");

            entity.HasOne(d => d.IdPrzypisanegoAdminaNavigation).WithMany(p => p.ZadaniePowiadomienies)
                .HasForeignKey(d => d.IdPrzypisanegoAdmina)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("zadanie_powiadomienie_id_przypisanego_admina_fkey");
        });

        modelBuilder.Entity<Zamowienie>(entity =>
        {
            entity.HasKey(e => e.IdZamowienia).HasName("zamowienie_pkey");

            entity.ToTable("zamowienie");

            entity.Property(e => e.IdZamowienia).HasColumnName("id_zamowienia");
            entity.Property(e => e.CalkowitaKwota)
                .HasPrecision(10, 2)
                .HasColumnName("calkowita_kwota");
            entity.Property(e => e.DataZamowienia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_zamowienia");
            entity.Property(e => e.IdAdresuZamowienia).HasColumnName("id_adresu_zamowienia");
            entity.Property(e => e.IdMetodyDostawy).HasColumnName("id_metody_dostawy");
            entity.Property(e => e.IdMetodyPlatnosci).HasColumnName("id_metody_platnosci");
            entity.Property(e => e.IdStatusu).HasColumnName("id_statusu");
            entity.Property(e => e.IdUzytkownika).HasColumnName("id_uzytkownika");
            entity.Property(e => e.Imie).HasColumnName("imie");
            entity.Property(e => e.Nazwisko).HasColumnName("nazwisko");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.NumerTelefonu).HasColumnName("numer_telefonu");
            entity.HasOne(d => d.IdAdresuZamowieniaNavigation).WithMany(p => p.Zamowienies)
                .HasForeignKey(d => d.IdAdresuZamowienia)
                .HasConstraintName("zamowienie_id_adresu_zamowienia_fkey");

            entity.HasOne(d => d.IdMetodyDostawyNavigation).WithMany(p => p.Zamowienies)
                .HasForeignKey(d => d.IdMetodyDostawy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("zamowienie_id_metody_dostawy_fkey");

            entity.HasOne(d => d.IdMetodyPlatnosciNavigation).WithMany(p => p.Zamowienies)
                .HasForeignKey(d => d.IdMetodyPlatnosci)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("zamowienie_id_metody_platnosci_fkey");

            entity.HasOne(d => d.IdStatusuNavigation).WithMany(p => p.Zamowienies)
                .HasForeignKey(d => d.IdStatusu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("zamowienie_id_statusu_fkey");

            entity.HasOne(d => d.IdUzytkownikaNavigation).WithMany(p => p.Zamowienies)
                .HasForeignKey(d => d.IdUzytkownika)
                .HasConstraintName("zamowienie_id_uzytkownika_fkey");
        });

        modelBuilder.Entity<ZapytanieOProdukt>(entity =>
        {
            entity.HasKey(e => e.IdZapytania).HasName("zapytanie_o_produkt_pkey");

            entity.ToTable("zapytanie_o_produkt");

            entity.Property(e => e.IdZapytania).HasColumnName("id_zapytania");
            entity.Property(e => e.CzyObsluzone).HasColumnName("czy_obsluzone");
            entity.Property(e => e.DataZapytania)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_zapytania");
            entity.Property(e => e.EmailPytajacego)
                .HasMaxLength(255)
                .HasColumnName("email_pytajacego");
            entity.Property(e => e.IdProduktu).HasColumnName("id_produktu");
            entity.Property(e => e.TrescZapytania).HasColumnName("tresc_zapytania");

            entity.HasOne(d => d.IdProduktuNavigation).WithMany(p => p.ZapytanieOProdukts)
                .HasForeignKey(d => d.IdProduktu)
                .HasConstraintName("zapytanie_o_produkt_id_produktu_fkey");
        });

        modelBuilder.Entity<ZwrotZamowienium>(entity =>
        {
            entity.HasKey(e => e.IdZwrotu).HasName("zwrot_zamowienia_pkey");

            entity.ToTable("zwrot_zamowienia");

            entity.HasIndex(e => e.IdZamowienia, "zwrot_zamowienia_id_zamowienia_key").IsUnique();

            entity.Property(e => e.IdZwrotu).HasColumnName("id_zwrotu");
            entity.Property(e => e.CzyZaakceptowany).HasColumnName("czy_zaakceptowany");
            entity.Property(e => e.DataZgloszenia)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("data_zgloszenia");
            entity.Property(e => e.IdZamowienia).HasColumnName("id_zamowienia");
            entity.Property(e => e.Powod).HasColumnName("powod");

            entity.HasOne(d => d.IdZamowieniaNavigation)
                .WithOne(p => p.ZwrotZamowienium)
                .HasForeignKey<ZwrotZamowienium>(d => d.IdZamowienia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("zwrot_zamowienia_id_zamowienia_fkey");

            entity.Property(e => e.IdPozycji).HasColumnName("id_pozycji");
            entity.Property(e => e.IdTypuZgloszenia).HasColumnName("id_typu_zgloszenia");
            entity.Property(e => e.IdStatusuZgloszenia).HasColumnName("id_statusu_zgloszenia");

            entity.HasOne(d => d.IdPozycjiZamowienia)
                .WithOne(p => p.ZwrotZamowienium)
                .HasForeignKey<ZwrotZamowienium>(d => d.IdPozycji)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_zwrot_zamowienia_pozycja_zamowienia");

            entity.HasOne(d => d.IdTypuZgloszeniaNavigation)
                .WithMany(p => p.Zwroty)
                .HasForeignKey(d => d.IdTypuZgloszenia)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_zwrot_zamowienia_slownik_typ_zgloszenia");

            entity.HasOne(d => d.IdStatusuZgloszeniaNavigation)
                .WithMany(p => p.Zwroty)
                .HasForeignKey(d => d.IdStatusuZgloszenia)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_zwrot_zamowienia_slownik_status_zgloszenia");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
