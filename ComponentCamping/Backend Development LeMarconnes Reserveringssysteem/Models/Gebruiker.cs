public class Gebruiker
{
    public int GebruikerID { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Emailadres { get; set; } = string.Empty;
    public string HashedWachtwoord { get; set; } = string.Empty;
    public string Salt { get; set; } = string.Empty;
    public string? Telefoon { get; set; }
    public string? Autokenteken { get; set; }
    public string? Taal { get; set; }
}
