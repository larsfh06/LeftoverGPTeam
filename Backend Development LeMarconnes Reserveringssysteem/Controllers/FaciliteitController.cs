using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FaciliteitController : ControllerBase
{
    private readonly DAL _dal;

    public FaciliteitController(DAL dal)
    {
        _dal = dal;
    }


    [HttpGet("{id}")]
    public IActionResult GetFaciliteiten(int id = 0)
    {
        var fac = _dal.Faciliteiten.GetFaciliteiten(id);
        if (fac == null) return NotFound();
        return Ok(fac);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Faciliteit faciliteit)
    {
        if (faciliteit == null) return BadRequest();

        var newFaciliteit = _dal.Faciliteiten.Create(faciliteit);
        return Ok(newFaciliteit);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Faciliteit faciliteit)
    {
        if (faciliteit == null) return BadRequest();
        faciliteit.FaciliteitID = id;

        var updated = _dal.Faciliteiten.Update(faciliteit);
        if (!updated) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var deleted = _dal.Faciliteiten.Delete(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
