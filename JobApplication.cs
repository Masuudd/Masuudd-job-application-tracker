namespace JobTracker;

// En KLASS är en ritning. Analogi: ritningen till ett "ansökningskort".
// Varje gång vi skriver "new JobApplication(...)" skapas ett OBJEKT = ett riktigt kort.
// Därför har vi en klass: alla ansökningar har samma fält, så vi gör ritningen EN gång.
public class JobApplication
{
    // --- Attribut (egenskaper) ---
    // { get; set; } = vi får både läsa och ändra värdet utifrån.
    // "= string.Empty" ger ett startvärde så vi slipper null-varningar.
    public string CompanyName { get; set; } = string.Empty;
    public string PositionTitle { get; set; } = string.Empty;

    // Egenskapen heter Status och typen heter Status. Det är tillåtet i C#.
    public Status Status { get; set; }

    public DateTime ApplicationDate { get; set; }

    // Frågetecknet (DateTime?) betyder "kan vara tomt (null)".
    // Varför? Man får inte alltid svar. Då finns det inget datum att spara.
    public DateTime? ResponseDate { get; set; }

    public int SalaryExpectation { get; set; }

    // --- Konstruktor ---
    // Körs automatiskt vid "new JobApplication(...)".
    // Varför? Då tvingas man ange det viktigaste direkt, så inga kort blir halvtomma.
    // Status sätts alltid till Applied, för en ny ansökan har ju precis skickats.
    public JobApplication(string companyName, string positionTitle,
                          DateTime applicationDate, int salaryExpectation)
    {
        CompanyName = companyName;
        PositionTitle = positionTitle;
        ApplicationDate = applicationDate;
        SalaryExpectation = salaryExpectation;
        Status = Status.Applied;
        ResponseDate = null; // inget svar ännu
    }

    // --- Metod 1: antal dagar sedan ansökan skickades ---
    public int GetDaysSinceApplied()
    {
        // DateTime minus DateTime ger en TimeSpan (en tidslängd).
        // .Days plockar ut hela dagar som ett heltal.
        return (DateTime.Now - ApplicationDate).Days;
    }

    // --- Metod 2: kort sammanfattning som text ---
    public string GetSummary()
    {
        // $"..." = string interpolation: variabler i { } stoppas in i texten.
        // ?. och ?? hanterar att ResponseDate kan vara null:
        // "om datum finns, formatera det, annars skriv 'Inget svar'".
        string response = ResponseDate?.ToString("yyyy-MM-dd") ?? "Inget svar";

        return $"{CompanyName} - {PositionTitle} | Status: {Status} | " +
               $"Skickad: {ApplicationDate:yyyy-MM-dd} ({GetDaysSinceApplied()} dagar sedan) | " +
               $"Svar: {response} | Önskad lön: {SalaryExpectation} kr";
    }
}
