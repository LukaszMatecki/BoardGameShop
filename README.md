<p align="center">
  <a href="#wersja-polska"><img src="https://flagcdn.com/w40/pl.png" alt="Wersja polska" title="Wersja polska"></a>
  &nbsp;
  <a href="#english-version"><img src="https://flagcdn.com/w40/gb.png" alt="English version" title="English version"></a>
</p>

<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=vue,vite,js,cs,dotnet,postgres,docker,git&perline=8" alt="Vue.js, Vite, JavaScript, C#, .NET, PostgreSQL, Docker i Git">
  </a>
</p>

<a id="wersja-polska"></a>

# Sklep z planszówkami

## O projekcie

**Sklep z planszówkami** to aplikacja internetowa symulująca działanie nowoczesnego sklepu z grami planszowymi. Użytkownicy mogą przeglądać katalog, zapoznawać się ze szczegółami tytułów oraz przejść przez całą ścieżkę zakupową, od dodania gry do koszyka aż po złożenie zamówienia i sprawdzenie jego historii.

To projekt edukacyjny. Przykładowy katalog zawiera 10 istniejących gier planszowych, natomiast ceny i stany magazynowe mają charakter demonstracyjny.

<a id="spis-tresci"></a>

## Spis treści

1. [Funkcje](#funkcje)
2. [Układ projektu](#uklad-projektu)
3. [Uruchomienie lokalne (Windows)](#uruchomienie-lokalne)
4. [Konfiguracja i bezpieczeństwo](#konfiguracja-i-bezpieczenstwo)
5. [English version](#english-version)

---

<a id="funkcje"></a>

## Funkcje

### Dla użytkownika

- **Katalog produktów** - przeglądanie, wyszukiwanie i filtrowanie gier według kategorii.
- **Konta i autoryzacja** - rejestracja, logowanie i uwierzytelnianie użytkowników.
- **Proces zakupowy** - zarządzanie koszykiem, składanie zamówień i przeglądanie historii zakupów.
- **Lista życzeń i opinie** - zapisywanie ulubionych gier oraz wystawianie ocen i recenzji.

### Panel administracyjny

- **Zarządzanie katalogiem** - dodawanie, edycja i usuwanie produktów.
- **Obsługa sklepu** - zarządzanie zamówieniami i aktualną ofertą.

<a id="uklad-projektu"></a>

## Układ projektu

```text
projekt_sklep/
├── backend/       # REST API, modele danych, logika biznesowa i migracje EF Core
├── frontend/      # aplikacja kliencka SPA (Vue + Vite)
├── compose.yaml   # konfiguracja lokalnej bazy PostgreSQL
├── .env.example   # szablon zmiennych środowiskowych bazy danych
└── README.md
```

<a id="uruchomienie-lokalne"></a>

## Uruchomienie lokalne (Windows)

### Wymagania

- .NET SDK 10;
- Node.js wraz z npm;
- Docker Desktop z Docker Compose.

### 1. Pobierz projekt i uruchom bazę danych

```powershell
git clone https://github.com/LukaszMatecki/projekt_sklep.git
cd projekt_sklep
Copy-Item .env.example .env
```

Otwórz plik `.env`, wpisz własne lokalne hasło w polu `POSTGRES_PASSWORD`, a następnie uruchom bazę:

```powershell
docker compose up -d
```

### 2. Skonfiguruj sekrety backendu

W katalogu głównym projektu wykonaj poniższe polecenia. Zapiszą one connection string z lokalnego pliku `.env` w .NET User Secrets i wygenerują lokalny klucz JWT. Te wartości nie trafiają do repozytorium.

```powershell
cd backend

$passwordLine = Get-Content ../.env | Where-Object { $_ -match '^POSTGRES_PASSWORD=' } | Select-Object -First 1
if (-not $passwordLine) { throw "Ustaw POSTGRES_PASSWORD w pliku .env" }

$dbPassword = $passwordLine.Substring('POSTGRES_PASSWORD='.Length).Trim([char]39, [char]34)
$connectionString = "Host=localhost;Port=5432;Database=projektskle;Username=projektskle;Password=$dbPassword"
dotnet user-secrets set "ConnectionStrings:Postgres" $connectionString

$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
$jwtBytes = New-Object byte[] 48
$rng.GetBytes($jwtBytes)
$rng.Dispose()
$jwtKey = [Convert]::ToBase64String($jwtBytes)
dotnet user-secrets set "Jwt:Key" $jwtKey

Remove-Variable passwordLine, dbPassword, connectionString, rng, jwtBytes, jwtKey
```

### 3. Zastosuj migracje i uruchom backend

Jeśli narzędzie `dotnet-ef` nie jest jeszcze zainstalowane, zainstaluj je:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.7
```

Następnie, w katalogu `backend`, zaktualizuj bazę i uruchom API:

```powershell
dotnet ef database update --context LocalAppDbContext
dotnet run --launch-profile http
```

### 4. Uruchom frontend w drugim terminalu

Otwórz nowe okno PowerShell w katalogu głównym projektu i wykonaj:

```powershell
cd frontend
Copy-Item .env.example .env
npm.cmd install
npm.cmd run dev
```

Polecenie `npm.cmd` w PowerShell omija ewentualną blokadę wykonywania skryptu `npm.ps1`.

### Adresy lokalne

| Usługa | Adres |
| :--- | :--- |
| Frontend (sklep) | [http://localhost:5173](http://localhost:5173) |
| Backend (REST API) | [http://localhost:5238](http://localhost:5238) |
| PostgreSQL | `localhost:5432` |

Pozostaw backend i frontend uruchomione w osobnych terminalach. Każdy serwer zatrzymasz skrótem **Ctrl+C**.

<a id="konfiguracja-i-bezpieczenstwo"></a>

## Konfiguracja i bezpieczeństwo

Lokalne hasła, klucze JWT oraz connection stringi przechowuj w pliku `.env` lub w .NET User Secrets. Te dane są pomijane przez Git. Do repozytorium dodawaj wyłącznie pliki przykładowe, takie jak `.env.example` - nigdy prawdziwych haseł, kluczy ani connection stringów.

---

<a id="english-version"></a>

# Board Game Shop

## About

**Board Game Shop** is an online store for board games. Visitors can browse the catalogue, learn about each title, and follow the shopping process from adding a game to the cart through placing an order and reviewing their order history.

This is an educational project. The sample catalogue contains 10 real board games; prices and stock levels are for demonstration purposes.

## Contents

1. [Features](#features)
2. [Project structure](#project-structure)
3. [Run locally on Windows](#run-locally-on-windows)
4. [Configuration and security](#configuration-and-security)

## Features

### For customers

- **Product catalogue** - browse, search, and filter games by category.
- **Accounts and sign-in** - user registration and authentication.
- **Shopping flow** - manage the cart, place orders, and view order history.
- **Wishlist and reviews** - save favourite games and share ratings and reviews.

### Admin panel

- **Catalogue management** - add, edit, and remove products.
- **Shop operations** - manage orders and the current product offer.

<a id="project-structure"></a>

## Project structure

```text
projekt_sklep/
├── backend/       # REST API, data models, business logic, and EF Core migrations
├── frontend/      # client-side SPA (Vue + Vite)
├── compose.yaml   # local PostgreSQL database configuration
├── .env.example   # database environment variables template
└── README.md
```

<a id="run-locally-on-windows"></a>

## Run locally on Windows

### Requirements

- .NET SDK 10;
- Node.js with npm;
- Docker Desktop with Docker Compose.

### 1. Clone the repository and start the database

```powershell
git clone https://github.com/LukaszMatecki/projekt_sklep.git
cd projekt_sklep
Copy-Item .env.example .env
```

Set your local `POSTGRES_PASSWORD` in `.env`, then start PostgreSQL:

```powershell
docker compose up -d
```

### 2. Configure backend secrets

From the project root, use the PowerShell commands in the [Polish setup guide](#uruchomienie-lokalne) to save the connection string and a locally generated JWT key with .NET User Secrets.

### 3. Apply migrations and start the API

From the `backend` directory, run:

```powershell
dotnet ef database update --context LocalAppDbContext
dotnet run --launch-profile http
```

### 4. Start the frontend in a second terminal

From the project root, open a new PowerShell window and run:

```powershell
cd frontend
Copy-Item .env.example .env
npm.cmd install
npm.cmd run dev
```

`npm.cmd` avoids PowerShell's script execution policy blocking `npm.ps1`.

| Service | Address |
| :--- | :--- |
| Frontend (shop) | [http://localhost:5173](http://localhost:5173) |
| Backend (REST API) | [http://localhost:5238](http://localhost:5238) |
| PostgreSQL | `localhost:5432` |

Keep the backend and frontend terminals open while using the application. Press **Ctrl+C** in each terminal to stop the servers.

<a id="configuration-and-security"></a>

## Configuration and security

Keep local passwords, JWT keys, and connection strings in `.env` or .NET User Secrets. They are ignored by Git. Commit example templates such as `.env.example` only; never add real passwords, keys, or connection strings to the repository.
