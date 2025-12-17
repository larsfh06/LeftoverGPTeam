using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CampingController : ControllerBase
{
    private readonly DAL _dal;

    public CampingController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{stroom}/{huisdieren}")]
    public IActionResult GetCampings(int id = 0, int stroom = 0, bool huisdieren = false)
    {
        var camping = _dal.Campings.GetCampings(id, stroom, huisdieren);
        if (camping == null) return NotFound();
        return Ok(camping);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Camping camping)
    {
        if (camping == null) return BadRequest();

        var newCamping = _dal.Campings.Create(camping);
        return Ok(newCamping);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Camping camping)
    {
        if (camping == null) return BadRequest();
        camping.CampingID = id;

        var updated = _dal.Campings.Update(camping);
        if (!updated) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _dal.Campings.Delete(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
