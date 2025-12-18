using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GebruikerController : ControllerBase
{
    private readonly DAL _dal;

    public GebruikerController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{naam}/{telefoon}")]
    public IActionResult GetGebruikers(int id = 0, string naam = "ALL", string telefoon = "ALL", int BoekingID = 0, bool IncludeBoekingen = false)
    {
        var user = _dal.Gebruikers.GetGebruikers(id, naam, telefoon,BoekingID, IncludeBoekingen);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Gebruiker gebruiker)
    {
        if (gebruiker == null) return BadRequest();

        var newGebruiker = _dal.Gebruikers.Create(gebruiker);
        return Ok(newGebruiker);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Gebruiker gebruiker)
    {
        if (gebruiker == null) return BadRequest();
        gebruiker.GebruikerID = id;

        var updated = _dal.Gebruikers.Update(gebruiker);
        if (!updated) return NotFound();

        return NoContent();
    }
}
