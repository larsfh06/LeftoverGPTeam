public class Accommodatie
{
    public int AccommodatieID { get; set; }
    public int CampingID { get; set; }
    public Camping? Camping { get; set; }
}
