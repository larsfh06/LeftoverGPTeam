public class Faciliteit
{
    public int FaciliteitID { get; set; }
    public string FaciliteitNaam { get; set; } = string.Empty;
    public string? Omschrijving { get; set; }
    public int? Capaciteit { get; set; }
    public DateTime? Openingstijd { get; set; }
    public DateTime? Sluitingstijd { get; set; }
}
