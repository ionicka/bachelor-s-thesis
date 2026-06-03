using FlashCards.Core.DTOs;
using FlashCards.Core.Entities;
using FlashCards.Core.Enums;
using FlashCards.Core.Interfaces;

namespace FlashCards.Core.Services;

public class SrsService : ISrsService
{
    private static readonly int[] IntervaleBaza = { 0, 1, 2, 4, 7, 14, 30, 90 };

    public RezultatSrs CalculeazaUrmatoareaRevizuire(
    ProgresCuvant progres, CalitatRaspuns calitate)
    {
        // Intervalele în ordinea nivelurilor: 1, 5, 10, 20, 40, 80, 160...
        // Nivel 1 = prima dată văzut (azi din nou sau mâine)
        // Nivel 2 = mâine (1 zi)
        // Nivel 3 = 5 zile
        // Nivel 4 = 10 zile
        // Nivel 5 = 20 zile
        // Nivel 6 = 40 zile
        // Nivel 7 = 80 zile (consolidat)

        bool esteCorect = calitate != CalitatRaspuns.Nu_Stiu;

        byte nivelNou;
        int intervalNou;

        if (!esteCorect)
        {
            // Orice greșeală → reset la nivel 1, revizuire mâine
            nivelNou = 1;
            intervalNou = 1;
        }
        else
        {
            // Corect → avansează un nivel
            nivelNou = (byte)Math.Min(7, progres.NivelCunoastere + 1);
            intervalNou = nivelNou switch
            {
                1 => 1,    // prima dată → mâine
                2 => 1,    // confirmat → mâine
                3 => 5,    // 5 zile
                4 => 10,   // 10 zile
                5 => 20,   // 20 zile
                6 => 40,   // 40 zile
                7 => 80,   // 80 zile — consolidat
                _ => 1
            };
        }

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
            7 => 0.9,
            _ => 0.3
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