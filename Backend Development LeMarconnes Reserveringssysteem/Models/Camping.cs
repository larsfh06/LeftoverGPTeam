public class Camping
{
    public int CampingID { get; set; }
    public string? Regels { get; set; }
    public decimal? Lengte { get; set; }
    public decimal? Breedte { get; set; }
    public decimal? Stroom { get; set; }
    public bool? Huisdieren { get; set; }
    public Accommodatie? Accommodatie { get; set; }
}
