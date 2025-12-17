using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FaciliteitBlokkadeController : ControllerBase
{
    private readonly DAL _dal;

    public FaciliteitBlokkadeController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{status}/{reden}")]
    public IActionResult GetFaciliteitBlokkades(int id = 0, string status = "ALL", string reden = "ALL")
    {
        var blok = _dal.FaciliteitBlokkades.GetFaciliteitBlokkades(id, status, reden);
        if (blok == null) return NotFound();
        return Ok(blok);
    }


    [HttpPost]
    public IActionResult Create([FromBody] FaciliteitBlokkade faciliteitblokkade)
    {
        if (faciliteitblokkade == null) return BadRequest();

        var newFaciliteitBlokkade = _dal.FaciliteitBlokkades.Create(faciliteitblokkade);
        return Ok(newFaciliteitBlokkade);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] FaciliteitBlokkade faciliteitblokkade)
    {
        if (faciliteitblokkade == null) return BadRequest();
        faciliteitblokkade.BlokkadeID = id;

        var updated = _dal.FaciliteitBlokkades.Update(faciliteitblokkade);
        if (!updated) return NotFound();

        return NoContent();
    }
}
