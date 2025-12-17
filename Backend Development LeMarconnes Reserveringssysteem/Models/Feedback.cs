public class Feedback
{
    public int FeedbackID { get; set; }
    public int GebruikerID { get; set; }
    public int BoekingID { get; set; }
    public int FeedbackScore { get; set; }
    public string? FeedbackTekst { get; set; }
    public DateTime FeedbackDatum { get; set; }
}
