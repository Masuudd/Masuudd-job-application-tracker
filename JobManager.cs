namespace JobTracker;

// JobManager = "chefen över högen med ansökningskort".
// Analogi: JobApplication är ETT kort, JobManager är pärmen som håller alla kort.
// Varför två klasser? Single Responsibility: varje klass har ETT ansvar.
// Då blir koden lättare att hitta i och ändra.
public class JobManager
{
    // Listan med alla ansökningar. "private set" = andra klasser kan läsa
    // listan men inte byta ut den. Det skyddar mot misstag.
    public List<JobApplication> Applications { get; private set; } = new List<JobApplication>();

    // ---------------------------------------------------------
    // ÄNDRA DATA
    // ---------------------------------------------------------

    // Lägger till en ansökan.
    // Vi tar emot ett färdigt objekt som parameter. Frågorna till användaren ställs
    // i Program.cs, så JobManager bara sköter logik och data.
    public void AddJob(JobApplication job)
    {
        Applications.Add(job);
    }

    // Ändrar status. Returnerar true/false så att Program.cs kan veta om det gick bra.
    // "index" är positionen i listan (0 = första kortet).
    public bool UpdateStatus(int index, Status newStatus)
    {
        // Skyddsvillkor (guard clause): avbryt direkt om index är ogiltigt.
        // Utan detta kraschar programmet med ArgumentOutOfRangeException.
        if (index < 0 || index >= Applications.Count)
        {
            return false;
        }

        JobApplication job = Applications[index];
        job.Status = newStatus;

        // Om vi går från Applied till något annat har företaget svarat, så vi sparar datumet.
        // Är status Applied igen finns inget svar, så datumet nollställs.
        job.ResponseDate = newStatus == Status.Applied ? null : DateTime.Now;

        return true;
    }

    // Tar bort en ansökan, med samma typ av skydd som ovan.
    public bool RemoveJob(int index)
    {
        if (index < 0 || index >= Applications.Count)
        {
            return false;
        }

        Applications.RemoveAt(index);
        return true;
    }

    // ---------------------------------------------------------
    // VISA DATA
    // ---------------------------------------------------------

    // Visar ALLA ansökningar.
    public void ShowAll()
    {
        PrintList(Applications);
    }

    // LINQ #1: Where (filtrera). (VG)
    // Analogi: ett kaffefilter. Allt släpps in, bara det som matchar rinner igenom.
    // "a => a.Status == status" läses: "för varje ansökan a, behåll den om statusen matchar".
    public void ShowByStatus(Status status)
    {
        var filtered = Applications.Where(a => a.Status == status);
        PrintList(filtered);
    }

    // LINQ #2: OrderBy (sortera). (VG)
    // Äldst först. Vill man ha nyast först använder man OrderByDescending.
    // OrderBy ändrar INTE originallistan, den ger en ny sorterad vy.
    public void ShowSortedByDate()
    {
        var sorted = Applications.OrderBy(a => a.ApplicationDate);
        PrintList(sorted);
    }

    // Extra filter (VG): ansökningar utan svar som är äldre än 14 dagar.
    // Två villkor med && ("och"): båda måste stämma.
    public void ShowOldWithoutResponse()
    {
        var old = Applications.Where(a => a.ResponseDate == null
                                       && a.GetDaysSinceApplied() > 14);
        PrintList(old);
    }

    // Statistik. (VG)
    public void ShowStatistics()
    {
        // Count() är LINQ #3: räknar element. Applications.Count (utan parenteser)
        // är en egenskap på List, men Count() fungerar på alla samlingar.
        Console.WriteLine($"Totalt antal ansökningar: {Applications.Count()}");

        // Antal per status. Vi går igenom ALLA värden i enum:en, så även
        // status med 0 ansökningar visas.
        Console.WriteLine("\nAntal per status:");
        foreach (Status status in Enum.GetValues<Status>())
        {
            int count = Applications.Count(a => a.Status == status);
            Console.WriteLine($"  {status}: {count}");
        }

        // Genomsnittlig svarstid.
        // Steg 1: behåll bara ansökningar som har fått svar (Where).
        var answered = Applications.Where(a => a.ResponseDate != null);

        // Steg 2: Average() på en TOM lista kraschar, så vi kollar med Any() först.
        // Any() = "finns det minst ett element?"
        if (answered.Any())
        {
            // "!" betyder: "jag lovar att ResponseDate inte är null här" (vi filtrerade ju bort null).
            // ".Value" plockar ut själva datumet ur DateTime?.
            double averageDays = answered.Average(a => (a.ResponseDate!.Value - a.ApplicationDate).TotalDays);
            Console.WriteLine($"\nGenomsnittlig svarstid: {averageDays:F1} dagar");
        }
        else
        {
            Console.WriteLine("\nGenomsnittlig svarstid: inga svar ännu");
        }

        // Bonus: genomsnittlig önskad lön.
        if (Applications.Any())
        {
            double avgSalary = Applications.Average(a => a.SalaryExpectation);
            Console.WriteLine($"Genomsnittlig önskad lön: {avgSalary:F0} kr");
        }
    }

    // ---------------------------------------------------------
    // HJÄLPMETODER (private = bara JobManager själv använder dem)
    // ---------------------------------------------------------

    // Skriver ut en lista. Den tar IEnumerable<JobApplication>, vilket är "vilken
    // samling som helst av ansökningar". Därför kan EN metod skriva ut både hela listan
    // och LINQ-resultat. Det är DRY: Don't Repeat Yourself.
    private void PrintList(IEnumerable<JobApplication> jobs)
    {
        if (!jobs.Any())
        {
            Console.WriteLine("Inga ansökningar att visa.");
            return;
        }

        foreach (var job in jobs)
        {
            // Numret är positionen i HUVUDLISTAN + 1. Då är numret alltid detsamma
            // för samma ansökan, även efter filtrering eller sortering.
            int number = Applications.IndexOf(job) + 1;

            Console.ForegroundColor = GetColor(job.Status);
            Console.WriteLine($"{number}. {job.GetSummary()}");
            Console.ResetColor(); // Viktigt! Annars blir ALL kommande text färgad.
        }
    }

    // Väljer färg beroende på status (VG).
    // "switch expression": kortare form av switch som returnerar ett värde.
    private ConsoleColor GetColor(Status status)
    {
        return status switch
        {
            Status.Offer => ConsoleColor.Green,
            Status.Rejected => ConsoleColor.Red,
            Status.Interview => ConsoleColor.Yellow,
            _ => ConsoleColor.White // _ = "allt annat" (här: Applied)
        };
    }
}
