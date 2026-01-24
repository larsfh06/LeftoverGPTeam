using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly DAL _dal;

    public FeedbackController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{gebruikerID}")]
    public IActionResult GetFeedback(int id = 0, int gebruikerID = 0, bool IncludeGebruiker = false, int BoekingID = 0, bool IncludeBoeking = false)
    {
        var fb = _dal.Feedbacks.GetFeedback(id, gebruikerID, IncludeGebruiker, BoekingID, IncludeBoeking);
        if (fb == null) return NotFound();
        return Ok(fb);
    }

    [HttpPost]
    public IActionResult Create([FromBody] FeedbackRequest request)
    {
        if (request == null) return BadRequest();

        var feedback = new Feedback
        {
            GebruikerID = request.GebruikerID,
            BoekingID = request.BoekingID,
            FeedbackScore = request.FeedbackScore,
            FeedbackTekst = request.FeedbackTekst,
            FeedbackDatum = request.FeedbackDatum
        };

        var newFeedback = _dal.Feedbacks.Create(feedback);
        return Ok(newFeedback);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] FeedbackRequest request)
    {
        if (request == null) return BadRequest();

        var feedback = new Feedback
        {
            FeedbackID = id,
            GebruikerID = request.GebruikerID,
            BoekingID = request.BoekingID,
            FeedbackScore = request.FeedbackScore,
            FeedbackTekst = request.FeedbackTekst,
            FeedbackDatum = request.FeedbackDatum
        };

        var updated = _dal.Feedbacks.Update(feedback);
        if (!updated) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _dal.Feedbacks.Delete(id);
        if (!deleted) return NotFound();

        return NoContent();
    }

    public class FeedbackRequest
    {
        public int GebruikerID { get; set; }
        public int BoekingID { get; set; }
        public int FeedbackScore { get; set; }
        public string? FeedbackTekst { get; set; }
        public DateTime FeedbackDatum { get; set; }
    }
}
