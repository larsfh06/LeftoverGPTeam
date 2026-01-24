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
    public IActionResult Create([FromBody] FaciliteitRequest request)
    {
        if (request == null) return BadRequest();

        var faciliteit = new Faciliteit
        {
            FaciliteitNaam = request.FaciliteitNaam,
            Omschrijving = request.Omschrijving,
            Capaciteit = request.Capaciteit,
            Openingstijd = request.Openingstijd,
            Sluitingstijd = request.Sluitingstijd
        };

        var newFaciliteit = _dal.Faciliteiten.Create(faciliteit);
        return Ok(newFaciliteit);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] FaciliteitRequest request)
    {
        if (request == null) return BadRequest();

        var faciliteit = new Faciliteit
        {
            FaciliteitID = id,
            FaciliteitNaam = request.FaciliteitNaam,
            Omschrijving = request.Omschrijving,
            Capaciteit = request.Capaciteit,
            Openingstijd = request.Openingstijd,
            Sluitingstijd = request.Sluitingstijd
        };

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

    public class FaciliteitRequest
    {
        public string FaciliteitNaam { get; set; } = string.Empty;
        public string? Omschrijving { get; set; }
        public int? Capaciteit { get; set; }
        public DateTime? Openingstijd { get; set; }
        public DateTime? Sluitingstijd { get; set; }
    }
}
