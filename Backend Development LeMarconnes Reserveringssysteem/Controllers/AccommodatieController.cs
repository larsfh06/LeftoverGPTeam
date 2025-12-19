using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccommodatieController : ControllerBase
{
    private readonly DAL _dal;

    public AccommodatieController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}")]
    public IActionResult GetAccommodaties(int id = 0, int CampingID = 0, bool IncludeCamping = false, int BoekingID = 0, bool IncludeBoeking = false)
    {
        var acc = _dal.Accommodaties.GetAccommodaties(id, CampingID, IncludeCamping, BoekingID, IncludeBoeking);
        if (acc == null) return NotFound();
        return Ok(acc);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Accommodatie accommodatie)
    {
        if (accommodatie == null) return BadRequest();

        var newAccommodatie = _dal.Accommodaties.Create(accommodatie);
        return Ok(newAccommodatie);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Accommodatie accommodatie)
    {
        if (accommodatie == null) return BadRequest();
        accommodatie.AccommodatieID = id;

        var updated = _dal.Accommodaties.Update(accommodatie);
        if (!updated) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _dal.Accommodaties.Delete(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
