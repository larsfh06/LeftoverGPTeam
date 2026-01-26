using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BoekingController : ControllerBase
{
    private readonly DAL _dal;

    public BoekingController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{GebruikerID}/{AccommodatieID}")]
    public IActionResult GetBoekingen(int id = 0, int GebruikerID = 0, int AccommodatieID = 0, int BetalingID = 0, DateTime? Datum = null, bool IncludeGebruiker = false, bool IncludeAccommodatie = false, bool IncludeBetalingen = false)
    {
        var res = _dal.Boekingen.GetBoekingen(id, GebruikerID, AccommodatieID, BetalingID, Datum, IncludeGebruiker, IncludeAccommodatie, IncludeBetalingen);
        if (res == null) return NotFound();
        return Ok(res);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Boeking boeking)
    {
        if (boeking == null) return BadRequest();

        var newBoeking = _dal.Boekingen.Create(boeking);
        return Ok(newBoeking);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Boeking boeking)
    {
        if (boeking == null) return BadRequest();
        boeking.BoekingID = id;

        var updated = _dal.Boekingen.Update(boeking);
        if (!updated) return NotFound();

        return NoContent();
    }
}
