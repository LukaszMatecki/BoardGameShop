# Projekt_sklep — sklep z planszówkami

Lokalny sklep internetowy z planszówkami polskich twórców. Projekt łączy frontend w Vue 3, REST API w ASP.NET Core oraz bazę PostgreSQL. Zawiera przykładowy katalog 10 istniejących gier, m.in. „Kolejkę”, „Neuroshimę HEX!”, „Robinsona Crusoe”, „Nemesis” i „Wiedźmina: Stary Świat”.

To projekt edukacyjny. Ceny i stany magazynowe w katalogu są przykładowe.

## Funkcje

- przeglądanie, wyszukiwanie i filtrowanie gier według kategorii;
- konta użytkowników i logowanie;
- koszyk, lista życzeń i historia zamówień;
- oceny i opinie o produktach;
- panel administracyjny do zarządzania katalogiem i obsługi sklepu;
- lokalna baza PostgreSQL przygotowywana przez migracje Entity Framework Core.

## Technologie

- **Frontend:** Vue 3, JavaScript, Vite, Axios
- **Backend:** ASP.NET Core Web API, .NET 10, Entity Framework Core
- **Baza danych:** PostgreSQL
- **Uruchamianie bazy:** Docker Compose

## Układ projektu

- **backend/** — API, modele danych i migracje bazy;
- **frontend/** — aplikacja Vue;
- **compose.yaml** — lokalna baza PostgreSQL;
- **.env.example** — wzór lokalnej konfiguracji bazy.

## Wymagania

- .NET SDK 10;
- Node.js i npm;
- Docker Desktop z Docker Compose.

## Uruchomienie lokalne w Windows / PowerShell

### 1. Pobierz projekt i ustaw hasło bazy

```powershell
git clone https://github.com/LukaszMatecki/projekt_sklep.git
cd projekt_sklep
Copy-Item .env.example .env
```

Otwórz plik .env i wpisz własne lokalne hasło w polu POSTGRES_PASSWORD. Potem uruchom PostgreSQL:

```powershell
docker compose up -d
```

### 2. Skonfiguruj lokalne sekrety i bazę

Poniższe polecenia wczytają hasło z lokalnego pliku .env, zapiszą connection string w .NET User Secrets i wygenerują lokalny klucz JWT. Sekrety nie trafiają do repozytorium.

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

### 3. Zastosuj migracje i uruchom API

Jeśli polecenie dotnet ef nie jest dostępne, doinstaluj narzędzie:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.7
```

Następnie w terminalu backendu wykonaj migracje i uruchom serwer:

```powershell
dotnet ef database update --context LocalAppDbContext
dotnet run --launch-profile http
```

API będzie działać pod adresem **http://localhost:5238**.

### 4. Uruchom frontend w drugim terminalu

```powershell
cd frontend
Copy-Item .env.example .env
npm.cmd install
npm.cmd run dev
```

Użycie npm.cmd w PowerShell omija blokadę skryptu npm.ps1. Frontend będzie dostępny pod adresem **http://localhost:5173**.

Oba serwery pozostaw uruchomione w osobnych terminalach. Zatrzymasz je skrótem **Ctrl+C**.

## Konfiguracja i bezpieczeństwo

Lokalne hasła i klucze zapisuj w plikach .env lub przez .NET User Secrets. Są pomijane przez Git. Do publicznego repozytorium dodawaj wyłącznie pliki przykładowe, takie jak .env.example; nie umieszczaj w nim prawdziwych haseł, kluczy ani connection stringów.