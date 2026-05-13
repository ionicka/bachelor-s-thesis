using LexaCard.Core.DTOs;
using LexaCard.Core.Entities;
using LexaCard.Core.Enums;
using LexaCard.Core.Interfaces;

namespace LexaCard.Core.Services;

public class SrsService : ISrsService
{
    private static readonly int[] IntervaleBaza = { 0, 1, 2, 4, 7, 14, 30, 90 };

    public RezultatSrs CalculeazaUrmatoareaRevizuire(
        ProgresCuvant progres, CalitatRaspuns calitate)
    {
        byte nivelNou = progres.NivelCunoastere;
        double factor;

        switch (calitate)
        {
            case CalitatRaspuns.Nu_Stiu:
                nivelNou = (byte)Math.Max(1, progres.NivelCunoastere - 2);
                factor = 0.5;
                break;
            case CalitatRaspuns.Stiu_Ezitare:
                factor = 1.2;
                break;
            case CalitatRaspuns.Stiu_Sigur:
                nivelNou = (byte)Math.Min(7, progres.NivelCunoastere + 1);
                factor = 2.5;
                break;
            case CalitatRaspuns.Tastat_Corect:
                nivelNou = (byte)Math.Min(7, progres.NivelCunoastere + 2);
                factor = 3.0;
                break;
            default:
                factor = 1.0;
                break;
        }

        int intervalBaza = IntervaleBaza[nivelNou];
        int intervalNou = (int)Math.Round(
            Math.Max(intervalBaza, progres.IntervalCurentZile * factor));

        double variatie = 0.9 + Random.Shared.NextDouble() * 0.2;
        intervalNou = Math.Max(1, (int)Math.Round(intervalNou * variatie));

        return new RezultatSrs(
            nivelNou,
            intervalNou,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(intervalNou)));
    }

    public TipRaspuns DeterminaTipRaspuns(ProgresCuvant progres)
    {
        if (progres.NrRaspunsuriCorecte < 3)
            return TipRaspuns.Recunoastere;

        double prob = progres.NivelCunoastere switch
        {
            1 or 2 => 0.2,
            3 or 4 => 0.5,
            5 or 6 => 0.7,
            7      => 0.9,
            _      => 0.3
        };

        return Random.Shared.NextDouble() < prob
            ? TipRaspuns.RemintireActiva
            : TipRaspuns.Recunoastere;
    }

    public bool VerificaTextTastat(string textTastat, string terminCorect)
    {
        if (string.IsNullOrWhiteSpace(textTastat)) return false;
        var t = textTastat.Trim().ToLowerInvariant();
        var c = terminCorect.Trim().ToLowerInvariant();
        if (t == c) return true;
        if (Math.Abs(t.Length - c.Length) <= 1)
            return Levenshtein(t, c) <= 1;
        return false;
    }

    private static int Levenshtein(string a, string b)
    {
        int[,] dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
        for (int j = 1; j <= b.Length; j++)
        {
            int cost = a[i - 1] == b[j - 1] ? 0 : 1;
            dp[i, j] = Math.Min(
                Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                dp[i - 1, j - 1] + cost);
        }
        return dp[a.Length, b.Length];
    }
}
