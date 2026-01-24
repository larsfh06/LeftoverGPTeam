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
    public IActionResult Create([FromBody] FaciliteitBlokkadeRequest request)
    {
        if (request == null) return BadRequest();

        var faciliteitBlokkade = new FaciliteitBlokkade
        {
            FaciliteitID = request.FaciliteitID,
            BlokkadeType = request.BlokkadeType,
            BeginDatum = request.BeginDatum,
            EindDatum = request.EindDatum,
            BlokkadeReden = request.BlokkadeReden,
            Status = request.Status
        };

        var newFaciliteitBlokkade = _dal.FaciliteitBlokkades.Create(faciliteitBlokkade);
        return Ok(newFaciliteitBlokkade);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] FaciliteitBlokkadeRequest request)
    {
        if (request == null) return BadRequest();

        var faciliteitBlokkade = new FaciliteitBlokkade
        {
            BlokkadeID = id,
            FaciliteitID = request.FaciliteitID,
            BlokkadeType = request.BlokkadeType,
            BeginDatum = request.BeginDatum,
            EindDatum = request.EindDatum,
            BlokkadeReden = request.BlokkadeReden,
            Status = request.Status
        };

        var updated = _dal.FaciliteitBlokkades.Update(faciliteitBlokkade);
        if (!updated) return NotFound();

        return NoContent();
    }

    public class FaciliteitBlokkadeRequest
    {
        public int FaciliteitID { get; set; }
        public string? BlokkadeType { get; set; }
        public DateTime BeginDatum { get; set; }
        public DateTime EindDatum { get; set; }
        public string? BlokkadeReden { get; set; }
        public string? Status { get; set; }
    }
}
