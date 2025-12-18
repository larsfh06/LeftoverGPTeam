using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BetalingController : ControllerBase
{
    private readonly DAL _dal;

    public BetalingController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{Status}/{BoekingID}")]
    public IActionResult GetAllByStatus(int id = 0, string Status = "ALL",int BoekingID = 0, bool IncludeBoeking = false)
    {
        var betaling = _dal.Betalingen.GetBetalingen(id, Status, BoekingID, IncludeBoeking);
        if (betaling == null) return NotFound();
        return Ok(betaling);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Betaling betaling)
    {
        if (betaling == null) return BadRequest();

        var newBetaling = _dal.Betalingen.Create(betaling);
        return Ok(newBetaling);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Betaling betaling)
    {
        if (betaling == null) return BadRequest();
        betaling.BetalingID = id;

        var updated = _dal.Betalingen.Update(betaling);
        if (!updated) return NotFound();

        return NoContent();
    }
}
