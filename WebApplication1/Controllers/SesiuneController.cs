using FlashCards.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SesiuneController : ControllerBase
{
    private readonly ISesiuneService _sesiune;

    public SesiuneController(ISesiuneService sesiune)
    {
        _sesiune = sesiune;
    }

    [HttpPost("incepe/{userId}")]
    public async Task<ActionResult<int>> IncepeSesiune(int userId)
    {
        var id = await _sesiune.IncepeSesiuneAsync(userId);
        return Ok(id);
    }

    [HttpPut("inchide/{sesiuneId}")]
    public async Task<ActionResult> InchideSesiune(
        int sesiuneId, int nrVazute, int nrCorect, int nrGresit)
    {
        await _sesiune.InchideSesiuneAsync(sesiuneId, nrVazute, nrCorect, nrGresit);
        return Ok();
    }
}