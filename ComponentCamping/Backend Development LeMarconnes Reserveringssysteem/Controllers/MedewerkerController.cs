using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MedewerkerController : ControllerBase
{
    private readonly DAL _dal;

    public MedewerkerController(DAL dal)
    {
        _dal = dal;
    }


    [HttpGet("{id}/{naam}")]
    public IActionResult GetMedewerkers(int id = 0, string naam = "ALL")
    {
        var med = _dal.Medewerkers.GetMedewerkers(id, naam);
        if (med == null) return NotFound();
        return Ok(med);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Medewerker medewerker)
    {
        if (medewerker == null) return BadRequest();

        var newMedewerker = _dal.Medewerkers.Create(medewerker);
        return Ok(newMedewerker);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Medewerker medewerker)
    {
        if (medewerker == null) return BadRequest();
        medewerker.MedewerkerID = id;

        var updated = _dal.Medewerkers.Update(medewerker);
        if (!updated) return NotFound();

        return NoContent();
    }
}
