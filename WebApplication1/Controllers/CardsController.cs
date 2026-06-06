using FlashCards.Core.DTOs;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ICardService _cards;

    public CardsController(ICardService cards)
    {
        _cards = cards;
    }

    [HttpGet("sesiune/{userId}")]
    public async Task<ActionResult<List<CardDto>>> GetSesiune(
        int userId, int cuvinteNoi = 10, int maxRevizuiri = 100)
    {
        var result = await _cards.GetSesiuneAsync(userId, cuvinteNoi, maxRevizuiri);
        return Ok(result);
    }

    [HttpPost("raspuns/{userId}")]
    public async Task<ActionResult<RezultatRaspunsDto>> ProceseazaRaspuns(
        int userId, RaspunsCardDto dto)
    {
        var result = await _cards.ProceseazaRaspunsAsync(userId, dto);
        return Ok(result);
    }

    [HttpGet("statistici/{userId}")]
    public async Task<ActionResult<StatisticiDto>> GetStatistici(int userId)
    {
        var result = await _cards.GetStatisticiAsync(userId);
        return Ok(result);
    }

    [HttpGet("disponibilitate/{userId}")]
    public async Task<ActionResult<DisponibilitateSesiuneDto>> GetDisponibilitate(
        int userId,
        [FromQuery] List<NivelCuvant> niveluri,
        [FromQuery] List<DomeniuCuvant> domenii)
    {
        var result = await _cards.GetDisponibilitateAsync(userId, niveluri, domenii);
        return Ok(result);
    }
    [HttpPost("ignora/{userId}/{cuvantId}")]
    public async Task<ActionResult> IgnoraCuvant(int userId, int cuvantId)
    {
        await _cards.IgnoraCuvantAsync(userId, cuvantId);
        return Ok();
    }

    [HttpPost("scoate-ignorare/{userId}/{cuvantId}")]
    public async Task<ActionResult> ScoateIgnorare(int userId, int cuvantId)
    {
        await _cards.ScoateIgnorareAsync(userId, cuvantId);
        return Ok();
    }
    [HttpGet("toate/{userId}")]
    public async Task<ActionResult<List<CardDto>>> GetToateCuvintele(int userId)
    {
        var result = await _cards.GetToateCuvinteleAsync(userId);
        return Ok(result);
    }
}