using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BetalingController : ControllerBase
{
    private readonly DAL _dal;

    public BetalingController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{Status}/{BoekingID}")]
    public IActionResult GetAllByStatus(int id = 0, string Status = "ALL", int BoekingID = 0, bool IncludeBoeking = false)
    {
        var betaling = _dal.Betalingen.GetBetalingen(id, Status, BoekingID, IncludeBoeking);
        if (betaling == null) return NotFound();
        return Ok(betaling);
    }

    [HttpPost]
    public IActionResult Create([FromBody] BetalingRequest request)
    {
        if (request == null) return BadRequest();

        var betaling = new Betaling
        {
            BoekingID = request.BoekingID,
            Type = request.Type,
            Bedrag = request.Bedrag,
            Methode = request.Methode,
            Status = request.Status,
            Korting = request.Korting,
            DatumOrigine = request.DatumOrigine,
            DatumBetaald = request.DatumBetaald
        };

        var newBetaling = _dal.Betalingen.Create(betaling);
        return Ok(newBetaling);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] BetalingRequest request)
    {
        if (request == null) return BadRequest();

        var betaling = new Betaling
        {
            BetalingID = id,
            BoekingID = request.BoekingID,
            Type = request.Type,
            Bedrag = request.Bedrag,
            Methode = request.Methode,
            Status = request.Status,
            Korting = request.Korting,
            DatumOrigine = request.DatumOrigine,
            DatumBetaald = request.DatumBetaald
        };

        var updated = _dal.Betalingen.Update(betaling);
        if (!updated) return NotFound();

        return NoContent();
    }

    public class BetalingRequest
    {
        public int BoekingID { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Bedrag { get; set; }
        public string? Methode { get; set; }
        public string? Status { get; set; }
        public decimal? Korting { get; set; }
        public DateTime? DatumOrigine { get; set; }
        public DateTime? DatumBetaald { get; set; }
    }
}
