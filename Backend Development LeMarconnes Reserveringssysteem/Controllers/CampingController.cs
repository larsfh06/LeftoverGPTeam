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
    public IActionResult GetCampings(int id = 0, int stroom = 0, bool huisdieren = false, int AccommodatieID = 0, bool IncludeAccommodatie = false)
    {
        var camping = _dal.Campings.GetCampings(id, stroom, huisdieren, AccommodatieID, IncludeAccommodatie);
        if (camping == null) return NotFound();
        return Ok(camping);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CampingRequest request)
    {
        if (request == null) return BadRequest();

        var camping = new Camping
        {
            Regels = request.Regels,
            Lengte = request.Lengte,
            Breedte = request.Breedte,
            Stroom = request.Stroom,
            Huisdieren = request.Huisdieren
        };

        var newCamping = _dal.Campings.Create(camping);
        return Ok(newCamping);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] CampingRequest request)
    {
        if (request == null) return BadRequest();

        var camping = new Camping
        {
            CampingID = id,
            Regels = request.Regels,
            Lengte = request.Lengte,
            Breedte = request.Breedte,
            Stroom = request.Stroom,
            Huisdieren = request.Huisdieren
        };

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

    public class CampingRequest
    {
        public string? Regels { get; set; }
        public decimal? Lengte { get; set; }
        public decimal? Breedte { get; set; }
        public decimal? Stroom { get; set; }
        public bool? Huisdieren { get; set; }
    }
}
