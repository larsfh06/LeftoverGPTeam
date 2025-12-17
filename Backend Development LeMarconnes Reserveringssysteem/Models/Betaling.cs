public class Betaling
{
    public int BetalingID { get; set; }
    public int BoekingID { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Bedrag { get; set; }
    public string? Methode { get; set; }
    public string? Status { get; set; }
    public decimal? Korting { get; set; }
    public DateTime? DatumOrigine { get; set; }
    public DateTime? DatumBetaald { get; set; }
}
