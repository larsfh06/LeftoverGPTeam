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
        var user = _dal.Gebruikers.GetGebruikers(id, naam, telefoon, BoekingID, IncludeBoekingen);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Create([FromBody] GebruikerRequest request)
    {
        if (request == null) return BadRequest();

        var gebruiker = new Gebruiker
        {
            Naam = request.Naam,
            Emailadres = request.Emailadres,
            HashedWachtwoord = request.HashedWachtwoord,
            Salt = request.Salt,
            Telefoon = request.Telefoon,
            Autokenteken = request.Autokenteken,
            Taal = request.Taal
        };

        var newGebruiker = _dal.Gebruikers.Create(gebruiker);
        return Ok(newGebruiker);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] GebruikerRequest request)
    {
        if (request == null) return BadRequest();

        var gebruiker = new Gebruiker
        {
            GebruikerID = id,
            Naam = request.Naam,
            Emailadres = request.Emailadres,
            HashedWachtwoord = request.HashedWachtwoord,
            Salt = request.Salt,
            Telefoon = request.Telefoon,
            Autokenteken = request.Autokenteken,
            Taal = request.Taal
        };

        var updated = _dal.Gebruikers.Update(gebruiker);
        if (!updated) return NotFound();

        return NoContent();
    }

    public class GebruikerRequest
    {
        public string Naam { get; set; } = string.Empty;
        public string Emailadres { get; set; } = string.Empty;
        public string HashedWachtwoord { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public string? Telefoon { get; set; }
        public string? Autokenteken { get; set; }
        public string? Taal { get; set; }
    }
}
