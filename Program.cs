/*
 Program.cs = receptionen. Den visar menyn och tar emot vad användaren skriver.
 Själva jobbet gör JobManager.

 Så fungerar det:
   while-loop visar menyn  ->  switch väljer vad som händer  ->  val 0 avslutar.
*/

using JobTracker;

// Program.cs = "receptionen". Den pratar med användaren och ber JobManager göra jobbet.
// Här finns INGEN affärslogik, bara menyer och inmatning.

// Så att emojis och å/ä/ö visas rätt i konsolen.
Console.OutputEncoding = System.Text.Encoding.UTF8;

// Skapar EN JobManager. Det är vår "pärm".
var manager = new JobManager();

// Valfritt: testdata så du slipper skriva in allt varje gång du testar.
// Ta bort de här raderna när du vill börja med en tom lista.
AddSampleData(manager);

// running = true betyder att menyn visas igen. Vid val 0 blir den false.
bool running = true;

// while-loop: menyn visas om och om igen tills användaren väljer Avsluta.
while (running)
{
    ShowMenu();
    string? choice = Console.ReadLine();
    Console.WriteLine();

    // switch väljer vad som ska hända beroende på vad användaren skrev.
    switch (choice)
    {
        case "1":
            AddNewApplication();
            break;
        case "2":
            manager.ShowAll();
            break;
        case "3":
            FilterByStatus();
            break;
        case "4":
            manager.ShowSortedByDate();
            break;
        case "5":
            manager.ShowStatistics();
            break;
        case "6":
            UpdateStatus();
            break;
        case "7":
            RemoveApplication();
            break;
        case "8":
            Console.WriteLine("Visa ansökningar utan svar äldre än 14 dagar:");
            manager.ShowOldWithoutResponse();
            break;
        case "0":
            running = false;
            Console.WriteLine("Hejdå! 👋");
            break;
        default:
            // Felhantering (VG): okänt val ger ett vänligt meddelande, ingen krasch.
            Console.WriteLine("Ogiltigt val, försök igen.");
            break;
    }

    if (running)
    {
        Console.WriteLine("\nTryck på valfri tangent för att fortsätta...");
        Console.ReadKey();
        Console.Clear();
    }
}

// =====================================================================
// LOKALA METODER: små funktioner som delar upp koden.
// Varför? Hade allt legat inuti switch hade filen blivit en lång oläslig vägg.
// =====================================================================

void ShowMenu()
{
    Console.WriteLine("===== JOB APPLICATION TRACKER =====");
    Console.WriteLine("1. ➕ Lägg till ny ansökan");
    Console.WriteLine("2. 📋 Visa alla ansökningar");
    Console.WriteLine("3. 🔍 Filtrera efter status");
    Console.WriteLine("4. 📅 Sortera efter datum");
    Console.WriteLine("5. 📊 Visa statistik");
    Console.WriteLine("6. ✏️  Uppdatera status");
    Console.WriteLine("7. 🗑️  Ta bort en ansökan");
    Console.WriteLine("8. ⏰ Utan svar äldre än 14 dagar");
    Console.WriteLine("0. 💾 Avsluta");
    Console.Write("\nDitt val: ");
}

void AddNewApplication()
{
    string company = ReadText("Företagsnamn: ");
    string position = ReadText("Tjänstetitel: ");
    DateTime date = ReadDate("Datum du skickade ansökan (yyyy-mm-dd, Enter = idag): ");
    int salary = ReadInt("Önskad lön i kronor: ");

    // Skapar objektet och ger det till managern.
    manager.AddJob(new JobApplication(company, position, date, salary));
    Console.WriteLine("✅ Ansökan tillagd!");
}

void FilterByStatus()
{
    Status? status = ReadStatus();
    if (status != null)
    {
        manager.ShowByStatus(status.Value);
    }
}

void UpdateStatus()
{
    manager.ShowAll();
    if (manager.Applications.Count == 0) return;

    int number = ReadInt("\nNummer på ansökan att uppdatera: ");
    Status? newStatus = ReadStatus();
    if (newStatus == null) return;

    // number - 1 eftersom användaren ser 1,2,3 men listan börjar på 0.
    bool ok = manager.UpdateStatus(number - 1, newStatus.Value);
    Console.WriteLine(ok ? "✅ Status uppdaterad!" : "❌ Det finns ingen ansökan med det numret.");
}

void RemoveApplication()
{
    manager.ShowAll();
    if (manager.Applications.Count == 0) return;

    int number = ReadInt("\nNummer på ansökan att ta bort: ");
    bool ok = manager.RemoveJob(number - 1);
    Console.WriteLine(ok ? "🗑️ Ansökan borttagen!" : "❌ Det finns ingen ansökan med det numret.");
}

// ---------------------------------------------------------------------
// INMATNINGSHJÄLPARE (felhantering, VG)
// Principen: "lita aldrig på användaren". Vi frågar om och om igen tills
// svaret är giltigt. Använder TryParse istället för Parse eftersom
// Parse KRASCHAR vid felaktig text och TryParse bara svarar true/false.
// ---------------------------------------------------------------------

string ReadText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        // IsNullOrWhiteSpace fångar både tomt och bara mellanslag.
        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }
        Console.WriteLine("❌ Fältet får inte vara tomt.");
    }
}

int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        // "out int result" = TryParse lägger svaret i variabeln result om det lyckas.
        if (int.TryParse(Console.ReadLine(), out int result) && result >= 0)
        {
            return result;
        }
        Console.WriteLine("❌ Skriv ett heltal (0 eller större).");
    }
}

DateTime ReadDate(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        // Bara Enter = använd dagens datum, smidigt i vardagen.
        if (string.IsNullOrWhiteSpace(input))
        {
            return DateTime.Now;
        }
        if (DateTime.TryParse(input, out DateTime result))
        {
            return result;
        }
        Console.WriteLine("❌ Fel format. Skriv t.ex. 2026-10-19.");
    }
}

// Returnerar Status? (kan vara null om användaren valde fel).
Status? ReadStatus()
{
    Console.WriteLine("Välj status: 1=Applied, 2=Interview, 3=Offer, 4=Rejected");
    Console.Write("Val: ");
    string? input = Console.ReadLine();

    switch (input)
    {
        case "1": return Status.Applied;
        case "2": return Status.Interview;
        case "3": return Status.Offer;
        case "4": return Status.Rejected;
        default:
            Console.WriteLine("❌ Ogiltigt statusval.");
            return null;
    }
}

void AddSampleData(JobManager m)
{
    m.AddJob(new JobApplication("Volvo", "Junior .NET-utvecklare", DateTime.Now.AddDays(-30), 35000));
    m.AddJob(new JobApplication("Spotify", "Backend-utvecklare", DateTime.Now.AddDays(-20), 42000));
    m.AddJob(new JobApplication("Ericsson", "Systemutvecklare", DateTime.Now.AddDays(-5), 38000));
    m.UpdateStatus(0, Status.Interview);
    m.UpdateStatus(1, Status.Rejected);
}
