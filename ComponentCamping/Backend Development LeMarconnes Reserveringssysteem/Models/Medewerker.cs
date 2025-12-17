public class Medewerker
{
    public int MedewerkerID { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Emailadres { get; set; } = string.Empty;
    public string HashedWachtwoord { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string? Telefoon { get; set; }
    public string? Taal { get; set; }
    public string Accounttype { get; set; } = string.Empty;
}
