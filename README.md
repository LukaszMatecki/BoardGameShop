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

# Projekt_sklep — sklep z planszówkami

## O projekcie

Projekt_sklep to internetowy sklep z planszówkami polskich twórców. Pozwala odkrywać gry, poznawać ich szczegóły i przejść przez najważniejsze etapy zakupów — od dodania produktu do koszyka po złożenie zamówienia.

To projekt edukacyjny. Katalog zawiera 10 istniejących gier, między innymi „Kolejkę”, „Neuroshimę HEX!”, „Robinsona Crusoe”, „Nemesis” i „Wiedźmina: Stary Świat”. Ceny i stany magazynowe są przykładowe.

<a id="spis-tresci"></a>

## Spis treści

1. [Funkcje](#funkcje)
2. [Przykładowy katalog](#przykładowy-katalog)
3. [Technologie](#technologie)
4. [Układ projektu](#uklad-projektu)
5. [Uruchomienie lokalne](#uruchomienie-lokalne)
6. [Konfiguracja i bezpieczeństwo](#konfiguracja-i-bezpieczenstwo)
7. [English version](#english-version)

---

<a id="funkcje"></a>

## Funkcje

- przeglądanie, wyszukiwanie i filtrowanie gier według kategorii;
- konta użytkowników i logowanie;
- koszyk, lista życzeń i historia zamówień;
- oceny i opinie o produktach;
- panel administracyjny do zarządzania katalogiem i obsługi sklepu.

<a id="przykładowy-katalog"></a>

## Przykładowy katalog

W sklepie znajdują się polskie i wydane w Polsce tytuły, takie jak „Kolejka”, „Neuroshima HEX!”, „Robinson Crusoe: Przygoda na przeklętej wyspie”, „Nemesis” oraz „Wiedźmin: Stary Świat”.

<a id="technologie"></a>

## Technologie

<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=vue,vite,js,cs,dotnet,postgres,docker,git&perline=8" alt="Vue.js, Vite, JavaScript, C#, .NET, PostgreSQL, Docker i Git">
  </a>
</p>

<a id="uklad-projektu"></a>

## Układ projektu

```text
projekt_sklep/
├── backend/       # REST API, modele i migracje bazy danych
├── frontend/      # aplikacja sklepu
├── compose.yaml   # lokalna baza PostgreSQL
├── .env.example   # szablon konfiguracji bazy
└── README.md
```

<a id="uruchomienie-lokalne"></a>

## Uruchomienie lokalne

### Wymagania

- .NET SDK 10;
- Node.js i npm;
- Docker Desktop z Docker Compose.

### 1. Pobierz projekt i uruchom bazę danych

```powershell
git clone https://github.com/LukaszMatecki/projekt_sklep.git
cd projekt_sklep
Copy-Item .env.example .env
```

Otwórz plik `.env` i wpisz własne lokalne hasło w polu `POSTGRES_PASSWORD`, a następnie uruchom bazę:

```powershell
docker compose up -d
```

### 2. Skonfiguruj sekrety backendu

W poniższym przykładzie hasło z lokalnego pliku `.env` jest używane do zapisania connection stringa w .NET User Secrets. Generowany jest także lokalny klucz JWT. Te wartości nie trafiają do repozytorium.

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

Jeśli nie masz jeszcze narzędzia `dotnet-ef`, zainstaluj je:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.7
```

Następnie, w katalogu `backend`, wykonaj migracje i uruchom API:

```powershell
dotnet ef database update --context LocalAppDbContext
dotnet run --launch-profile http
```

API będzie dostępne pod adresem [http://localhost:5238](http://localhost:5238).

### 4. Uruchom frontend w drugim terminalu

W nowym terminalu PowerShell, z katalogu głównego projektu, wykonaj:

```powershell
cd frontend
Copy-Item .env.example .env
npm.cmd install
npm.cmd run dev
```

Frontend będzie dostępny pod adresem [http://localhost:5173](http://localhost:5173). W PowerShell polecenie `npm.cmd` pozwala ominąć blokadę skryptu `npm.ps1`.

Pozostaw backend i frontend uruchomione w osobnych terminalach. Każdy z nich zatrzymasz skrótem **Ctrl+C**.

<a id="konfiguracja-i-bezpieczenstwo"></a>

## Konfiguracja i bezpieczeństwo

Lokalne hasła i klucze zapisuj w pliku `.env` lub przez .NET User Secrets. Są pomijane przez Git. Do repozytorium dodawaj wyłącznie pliki przykładowe, takie jak `.env.example`; nie umieszczaj w nim prawdziwych haseł, kluczy ani connection stringów.

---

<a id="english-version"></a>

# Projekt_sklep — board game shop

## About

Projekt_sklep is an online shop for board games by Polish creators. Visitors can explore the catalogue, learn about each game, and follow the main shopping steps, from adding a game to the cart to placing an order.

This is an educational project. Its sample catalogue contains 10 real games, including *Kolejka*, *Neuroshima HEX!*, *Robinson Crusoe*, *Nemesis*, and *The Witcher: Old World*. Prices and stock levels are for demonstration purposes.

## Features

- browse, search, and filter games by category;
- user accounts and sign-in;
- shopping cart, wishlist, and order history;
- product ratings and reviews;
- an admin panel for managing the catalogue and shop.

## Technology badges

<p align="center">
  <a href="https://skillicons.dev">
    <img src="https://skillicons.dev/icons?i=vue,vite,js,cs,dotnet,postgres,docker,git&perline=8" alt="Vue.js, Vite, JavaScript, C#, .NET, PostgreSQL, Docker, and Git">
  </a>
</p>

## Project structure

```text
projekt_sklep/
├── backend/       # REST API, data models, and database migrations
├── frontend/      # shop application
├── compose.yaml   # local PostgreSQL database
├── .env.example   # database configuration template
└── README.md
```

## Run locally on Windows

### Requirements

- .NET SDK 10;
- Node.js and npm;
- Docker Desktop with Docker Compose.

### 1. Get the project and start the database

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

From the `backend` directory, use the PowerShell commands in the [Polish setup guide](#uruchomienie-lokalne) to save the connection string and a locally generated JWT key with .NET User Secrets.

### 3. Apply migrations and start the API

From `backend`, run:

```powershell
dotnet ef database update --context LocalAppDbContext
dotnet run --launch-profile http
```

The API will be available at [http://localhost:5238](http://localhost:5238).

### 4. Start the frontend in a second terminal

From the project root, run:

```powershell
cd frontend
Copy-Item .env.example .env
npm.cmd install
npm.cmd run dev
```

The frontend will be available at [http://localhost:5173](http://localhost:5173). Keep both terminals open while using the shop; press **Ctrl+C** in each one to stop the servers.

## Configuration and security

Keep local passwords and keys in `.env` or .NET User Secrets. These files are ignored by Git. Commit example configuration files only, and never add real passwords, keys, or connection strings to the repository.
