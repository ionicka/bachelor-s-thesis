using FlashCards.Core.DTOs;
using FlashCards.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlashCards.Api.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _admin;
    public AdminController(IAdminService admin) => _admin = admin;

    [HttpGet("cuvinte")]
    public async Task<ActionResult<List<CuvantListaDto>>> GetCuvinte() =>
        Ok(await _admin.GetCuvinteAsync());

    [HttpPost("cuvinte/filtru")]
    public async Task<ActionResult<List<CuvantListaDto>>> GetCuvinteFiltrate(FiltruCuvinteDto filtru) =>
        Ok(await _admin.GetCuvinteAsync(filtru));

    [HttpGet("cuvinte/{id}")]
    public async Task<ActionResult<CuvantEditDto?>> GetCuvant(int id) =>
        Ok(await _admin.GetCuvantPentruEditAsync(id));

    [HttpPost("cuvinte")]
    public async Task<ActionResult<RezultatOperatieDto>> CreeazaCuvant(CuvantEditDto dto) =>
        Ok(await _admin.CreeazaCuvantAsync(dto));

    [HttpPut("cuvinte/{id}")]
    public async Task<ActionResult<RezultatOperatieDto>> ActualizeazaCuvant(int id, CuvantEditDto dto)
    {
        dto.Id = id;
        return Ok(await _admin.ActualizeazaCuvantAsync(dto));
    }

    [HttpDelete("cuvinte/{id}")]
    public async Task<ActionResult<RezultatOperatieDto>> StergeCuvant(int id) =>
        Ok(await _admin.StergeCuvantAsync(id));

    [HttpGet("cuvinte/total")]
    public async Task<ActionResult<int>> GetTotal() =>
        Ok(await _admin.GetNrTotalCuvinteAsync());

    [HttpGet("cuvinte/domenii")]
    public async Task<ActionResult> GetDomenii() =>
        Ok(await _admin.GetNrPeDomeniiAsync());
}