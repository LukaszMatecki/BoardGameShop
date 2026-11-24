# Projekt sklepu

Projekt jest podzielony na frontend Vue/Vite i backend ASP.NET Core API. Można go uruchamiać lokalnie z terminala; Visual Studio nie jest wymagane.

## Wymagania

- .NET SDK 10
- Node.js z npm
- Docker Desktop (do lokalnej bazy PostgreSQL)

## Pierwsze uruchomienie

1. Skopiuj .env.example do .env, ustaw lokalne hasło i uruchom bazę przez Docker Compose:

   ```powershell
   docker compose up -d
   ```

2. Skonfiguruj connection string w .NET User Secrets (dane zostaną zapisane poza repozytorium):

   ```powershell
   cd backend
   dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5432;Database=projektskle;Username=projektskle;Password=WPISZ_HASLO_Z_PLIKU_ENV"
   ```

3. W pierwszym terminalu uruchom API:

   ```powershell
   cd backend
   # Uruchom raz; jeśli dotnet-ef jest już zainstalowany, pomiń tę linię.
   dotnet tool install --global dotnet-ef --version 10.0.7
   dotnet ef database update --context LocalAppDbContext
   dotnet run --launch-profile http
   ```

   API będzie dostępne pod `http://localhost:5238`.

4. W drugim terminalu przygotuj frontend i uruchom Vite:

   ```powershell
   cd frontend
   Copy-Item .env.example .env
   npm.cmd install
   npm.cmd run dev
   ```

   Strona będzie dostępna pod `http://localhost:5173`.

## Ustawienia lokalne

Hasła, connection stringi i konfigurację SMTP ustawiaj przez `dotnet user-secrets`, a nie w plikach śledzonych przez Git. Przykład SMTP (opcjonalny):

```powershell
cd backend
dotnet user-secrets set "Smtp:Host" "smtp.gmail.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:User" "twoj-adres@example.com"
dotnet user-secrets set "Smtp:Password" "WPISZ_LOKALNE_HASLO_APLIKACJI"
```

Przed wdrożeniem publicznym ustaw własne sekrety i konfigurację dla środowiska produkcyjnego.
