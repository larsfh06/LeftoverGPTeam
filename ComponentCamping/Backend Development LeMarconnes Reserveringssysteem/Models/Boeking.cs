public class Boeking
{
    public int BoekingID { get; set; }
    public int GebruikerID { get; set; }
    public DateTime? Datum { get; set; }
    public int AccommodatieID { get; set; }
    public DateTime CheckInDatum { get; set; }
    public DateTime CheckOutDatum { get; set; }
    public byte? AantalVolwassenen { get; set; }
    public byte? AantalJongeKinderen { get; set; }
    public byte? AantalOudereKinderen { get; set; }
    public string? Opmerking { get; set; }
    public bool? Cancelled { get; set; }
}
