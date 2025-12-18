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

    [HttpGet("{id}/{GebruikerID}")]
    public IActionResult GetFiltered(int id = 0, int GebruikerID = 0, bool IncludeBetalingen = false, bool IncludeGebruiker = false, bool IncludeAccommodatie = false)
    {
        var res = _dal.Boekingen.GetFiltered(id, GebruikerID, IncludeBetalingen, IncludeGebruiker, IncludeAccommodatie);
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
