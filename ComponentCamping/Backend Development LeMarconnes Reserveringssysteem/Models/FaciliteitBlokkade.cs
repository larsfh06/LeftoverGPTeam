public class FaciliteitBlokkade
{
    public int BlokkadeID { get; set; }
    public int FaciliteitID { get; set; }
    public string? BlokkadeType { get; set; }
    public DateTime BeginDatum { get; set; }
    public DateTime EindDatum { get; set; }
    public string? BlokkadeReden { get; set; }
    public string? Status { get; set; }
}
