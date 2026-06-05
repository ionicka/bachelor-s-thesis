using FlashCards.Core.DTOs;
using FlashCards.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginDto dto)
    {
        var (succes, utilizator, eroare) = await _auth.LoginAsync(dto);
        if (!succes)
            return BadRequest(new { eroare });
        return Ok(utilizator);
    }

    [HttpPost("inregistrare")]
    public async Task<ActionResult> Inregistrare(InregistrareDto dto)
    {
        var (succes, eroare) = await _auth.InregistreazaAsync(dto);
        if (!succes)
            return BadRequest(new { eroare });
        return Ok();
    }

    [HttpPut("setari/{userId}")]
    public async Task<ActionResult> ActualizeazaSetari(
        int userId, [FromQuery] int carduriNoi, [FromQuery] int maxCarduri)
    {
        await _auth.ActualizeazaSetariAsync(userId, carduriNoi, maxCarduri);
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UtilizatorDto>> GetUtilizator(int userId)
    {
        var u = await _auth.GetUtilizatorCurentAsync(userId);
        if (u == null) return NotFound();
        return Ok(u);
    }
}