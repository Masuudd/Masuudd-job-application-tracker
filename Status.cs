namespace JobTracker;

// ENUM = en "fast lista med tillåtna värden".
// Analogi: som en rullgardinsmeny. Man kan BARA välja något av dessa fyra.
// Varför enum och inte string? Med en string kan man skriva fel ("Intervju", "intervew").
// Med enum hittar kompilatorn felet åt oss innan programmet ens körs.
public enum Status
{
    Applied,    // = 0, ansökan skickad
    Interview,  // = 1, kallad till intervju
    Offer,      // = 2, fått erbjudande
    Rejected    // = 3, fått nej
}
