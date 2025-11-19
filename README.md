# Fribergs Bilar Expansion

Detta projekt är en vidareutveckling av tidigare inlämningsuppgift, där **Fribergs Bilar** går från en enkel webbsida till en fullständig lösning med **Blazor WebAssembly (WASM)** som frontend och **Web API** som backend.

## Arkitektur

### Web

- **Blazor WASM** som hämtar all data **endast via REST API-anrop**.
- Ingen direkt åtkomst till databasen.
- Hanterar autentisering via JWT + refresh tokens.

### Api

- **Web API** som hanterar all data, CRUD-operationer och autentisering.
- **Repository pattern** används för att strukturera dataåtkomst och affärslogik.

### Shared
- **Shared Class Library** med **DTO:er (Data Transfer Objects)** används för att skicka data mellan frontend och backend.

### Autentisering & tokenhantering

1. Användaren loggar in och får en **JWT-token** samt en **refresh token**.
2. Varje gång frontend hämtar data, kontrolleras om JWT-token är giltig.
3. Om JWT-token har gått ut används refresh token för att hämta en ny JWT-token.
4. Den nya token används sedan för API-anrop, vilket ger en smidig och säker användarupplevelse.

### Administratörer
* I filen `FribergAPI/Controllers/AuthController.cs` finns en **statisk lista av kända admin-eposter**.

## Kom igång

```bash
git clone https://github.com/VincentBrodin/FribergItExpansion.git
cd FribergItExpansion
dotnet restore
dotnet run --project FribergApi
dotnet run --project FribergWeb
```

## Teknologier

* **Frontend:** Blazor WebAssembly
* **Backend:** Web API (ASP.NET Core)
* **Arkitektur:** Repository pattern, Shared Class Library med DTO
* **Autentisering:** JWT + Refresh Tokens
* **Kommunikation:** RESTful API

## Noteringar

- All dataflöde går genom Web API, frontend har ingen direkt databasåtkomst.
- Repository-pattern gör backend mer modulär och lätt att underhålla.
- DTO:er säkerställer tydlig och säker dataöverföring mellan frontend och backend.
- Tokenförnyelse via refresh tokens ger smidig och säker användarupplevelse.
