using LexaCard.Core.Entities;
using LexaCard.Core.Interfaces;

namespace LexaCard.Core.Services;

public class SesiuneService : ISesiuneService
{
    private readonly ISesiuneRepository _repo;
    public SesiuneService(ISesiuneRepository repo) => _repo = repo;

    public async Task<int> IncepeSesuineAsync(int utilizatorId)
    {
        var s = await _repo.IncepeAsync(utilizatorId);
        return s.Id;
    }

    public async Task InchideSesiuneAsync(
        int sesiuneId, int nrVazute, int nrCorect, int nrGresit)
    {
        var s = await _repo.GetAsync(sesiuneId);
        if (s == null) return;
        s.NrCarduriVazute = nrVazute;
        s.NrCorect        = nrCorect;
        s.NrGresit        = nrGresit;
        await _repo.InchideAsync(sesiuneId);
    }

    public async Task<List<SesiuneStudiu>> GetIstoricAsync(int utilizatorId) =>
        await _repo.GetIstoricAsync(utilizatorId, 30);
}
