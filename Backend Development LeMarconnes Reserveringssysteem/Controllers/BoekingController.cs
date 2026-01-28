using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BoekingController : ControllerBase
{
    private readonly DAL _dal;

    public BoekingController(DAL dal)
    {
        _dal = dal;
    }

    [HttpGet("{id}/{GebruikerID}/{AccommodatieID}")]
    public IActionResult GetBoekingen(int id = 0, int GebruikerID = 0, int AccommodatieID = 0, int BetalingID = 0, DateTime? Datum = null, bool IncludeGebruiker = false, bool IncludeAccommodatie = false, bool IncludeBetalingen = false)
    {
        var res = _dal.Boekingen.GetBoekingen(id, GebruikerID, AccommodatieID, BetalingID, Datum, IncludeGebruiker, IncludeAccommodatie, IncludeBetalingen);
        if (res == null) return NotFound();
        return Ok(res);
    }

    [HttpPost]
    public IActionResult Create([FromBody] BoekingRequest request)
    {
        if (request == null) return BadRequest();

        var boeking = new Boeking
        {
            GebruikerID = request.GebruikerID,
            Datum = request.Datum,
            AccommodatieID = request.AccommodatieID,
            CheckInDatum = request.CheckInDatum,
            CheckOutDatum = request.CheckOutDatum,
            AantalVolwassenen = request.AantalVolwassenen,
            AantalJongeKinderen = request.AantalJongeKinderen,
            AantalOudereKinderen = request.AantalOudereKinderen,
            Opmerking = request.Opmerking,
            Cancelled = false
        };

        var newBoeking = _dal.Boekingen.Create(boeking);
        return Ok(newBoeking);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] BoekingRequest request)
    {
        if (request == null) return BadRequest();

        var boeking = new Boeking
        {
            BoekingID = id,
            GebruikerID = request.GebruikerID,
            Datum = request.Datum,
            AccommodatieID = request.AccommodatieID,
            CheckInDatum = request.CheckInDatum,
            CheckOutDatum = request.CheckOutDatum,
            AantalVolwassenen = request.AantalVolwassenen,
            AantalJongeKinderen = request.AantalJongeKinderen,
            AantalOudereKinderen = request.AantalOudereKinderen,
            Opmerking = request.Opmerking,
            Cancelled = request.Cancelled
        };

        var updated = _dal.Boekingen.Update(boeking);
        if (!updated) return NotFound();

        return NoContent();
    }

    public class BoekingRequest
    {
        public int GebruikerID { get; set; }
        public DateTime? Datum { get; set; }
        public int AccommodatieID { get; set; }
        public DateTime CheckInDatum { get; set; }
        public DateTime CheckOutDatum { get; set; }
        public byte? AantalVolwassenen { get; set; }
        public byte? AantalJongeKinderen { get; set; }
        public byte? AantalOudereKinderen { get; set; }
        public string? Opmerking { get; set; }
        public bool? Cancelled { get; set; }
    }
}