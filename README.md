# Job Application Tracker

**Namn:** <Masuud Ali>
**Datum:** <2026-10-21>

## Projektbeskrivning
Ett konsolprogram i C# som hjälper mig hålla koll på mina jobbansökningar.
Man kan lägga till, visa, filtrera (LINQ), sortera, uppdatera status på och ta bort ansökningar,
samt se statistik som antal per status och genomsnittlig svarstid.

## Hur man kör programmet
1. Installera [.NET 8 SDK](https://dotnet.microsoft.com/download)
2. Klona repot: `git clone <https://github.com/Masuudd/Masuudd-job-application-tracker.git>`
3. Gå in i mappen: `cd Masuudd-job-application-tracker`
4. Kör: `dotnet run`

## Struktur
| Fil | Ansvar |
|---|---|
| `Status.cs` | Enum med tillåtna statusar |
| `JobApplication.cs` | Representerar en ansökan |
| `JobManager.cs` | Lista, logik och LINQ |
| `Program.cs` | Meny och inmatning |

## Reflektion
Se `Reflektion.txt`.
