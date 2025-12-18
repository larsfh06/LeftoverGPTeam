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
    public IActionResult GetFeedback(int id = 0, int gebruikerID = 0, bool IncludeGebruiker = false, bool IncludeBoeking = false)
    {
        var fb = _dal.Feedbacks.GetFeedback(id, gebruikerID, IncludeGebruiker, IncludeBoeking);
        if (fb == null) return NotFound();
        return Ok(fb);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Feedback feedback)
    {
        if (feedback == null) return BadRequest();

        var newFeedback = _dal.Feedbacks.Create(feedback);
        return Ok(newFeedback);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Feedback feedback)
    {
        if (feedback == null) return BadRequest();
        feedback.FeedbackID = id;

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
}
