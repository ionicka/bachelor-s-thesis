using LexaCard.Core.Entities;
using LexaCard.Core.Enums;

namespace LexaCard.Data.Context;

public static class SeedData
{
    public static void Populeaza(LexaDbContext ctx)
    {
        var cuvinte = new List<Cuvant>
        {
            new() {
                Termen            = "perseverance",
                Definitie         = "The quality of continuing to try to achieve a particular aim despite difficulties.",
                DefinitieRo       = "Calitatea de a continua sa incerci sa atingi un scop in ciuda dificultatilor.",
                ExemplePropozitii = "Her [TERMEN] in learning the language finally paid off after two years.|Despite many failures, his [TERMEN] eventually led to success.|The athlete's [TERMEN] through injury inspired the whole team.",
                CaleImagini       = null,
                Pronuntie         = "/ˌpɜː.sɪˈvɪər.əns/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "resilient",
                Definitie         = "Able to recover quickly from difficult conditions or situations.",
                DefinitieRo       = "Capabil sa se recupereze rapid din conditii sau situatii dificile.",
                ExemplePropozitii = "She proved to be incredibly [TERMEN] after losing her job.|Children are often more [TERMEN] than adults give them credit for.|The [TERMEN] community rebuilt after the storm in record time.",
                CaleImagini       = null,
                Pronuntie         = "/rɪˈzɪl.i.ənt/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "ambiguous",
                Definitie         = "Having more than one possible meaning; not clear or decided.",
                DefinitieRo       = "Avand mai mult de un sens posibil; neclar sau nedecis.",
                ExemplePropozitii = "The manager gave an [TERMEN] answer that confused everyone.|His [TERMEN] smile left her wondering what he really meant.|The contract contained several [TERMEN] clauses that caused disputes.",
                CaleImagini       = null,
                Pronuntie         = "/æmˈbɪɡ.ju.əs/",
                Nivel             = NivelCuvant.ElementarSuperior,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "eloquent",
                Definitie         = "Expressing ideas and opinions clearly and effectively, using language well.",
                DefinitieRo       = "Exprimand idei si opinii clar si eficient, folosind bine limbajul.",
                ExemplePropozitii = "The president gave an [TERMEN] speech that moved the entire audience.|She is [TERMEN] in three languages and often acts as interpreter.|His [TERMEN] defense of the proposal convinced the entire board.",
                CaleImagini       = null,
                Pronuntie         = "/ˈel.ə.kwənt/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "catalyst",
                Definitie         = "A person or thing that causes a change or makes something happen faster.",
                DefinitieRo       = "O persoana sau un lucru care cauzeaza o schimbare sau accelereaza un proces.",
                ExemplePropozitii = "The new law was a [TERMEN] for significant social change.|Her speech acted as a [TERMEN] for the protest movement.|Technological innovation has been a [TERMEN] for economic growth.",
                CaleImagini       = null,
                Pronuntie         = "/ˈkæt.ə.lɪst/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "meticulous",
                Definitie         = "Very careful and precise, paying great attention to every detail.",
                DefinitieRo       = "Foarte atent si precis, acordand mare atentie fiecarui detaliu.",
                ExemplePropozitii = "The surgeon was [TERMEN] in her preparation before every operation.|He kept [TERMEN] records of every transaction for decades.|Her [TERMEN] research left no room for error in the final report.",
                CaleImagini       = null,
                Pronuntie         = "/məˈtɪk.jʊ.ləs/",
                Nivel             = NivelCuvant.Avansat,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "serendipity",
                Definitie         = "The occurrence of events by chance in a happy or beneficial way.",
                DefinitieRo       = "Aparitia evenimentelor din intamplare intr-un mod fericit sau benefic.",
                ExemplePropozitii = "Meeting my best friend on that train was pure [TERMEN].|The discovery of penicillin is often cited as a case of [TERMEN].|By a wonderful [TERMEN], they ended up at the same hotel.",
                CaleImagini       = null,
                Pronuntie         = "/ˌser.ənˈdɪp.ɪ.ti/",
                Nivel             = NivelCuvant.Avansat,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "innovative",
                Definitie         = "Introducing or using new ideas, methods, or inventions; creative and original.",
                DefinitieRo       = "Introducand sau folosind idei, metode sau inventii noi; creativ si original.",
                ExemplePropozitii = "The company is known for its [TERMEN] approach to problem solving.|She designed an [TERMEN] system that cut costs by half.|The [TERMEN] startup disrupted the entire industry within two years.",
                CaleImagini       = null,
                Pronuntie         = "/ˈɪn.ə.veɪ.tɪv/",
                Nivel             = NivelCuvant.ElementarSuperior,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "empathy",
                Definitie         = "The ability to understand and share the feelings of another person.",
                DefinitieRo       = "Abilitatea de a intelege si de a impartasi sentimentele unei alte persoane.",
                ExemplePropozitii = "Good nurses show great [TERMEN] toward their patients every day.|[TERMEN] is essential for building strong and lasting relationships.|She listened with [TERMEN] as her friend described the difficult situation.",
                CaleImagini       = null,
                Pronuntie         = "/ˈem.pə.θi/",
                Nivel             = NivelCuvant.ElementarSuperior,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "profound",
                Definitie         = "Very great or intense; having or showing great knowledge or insight.",
                DefinitieRo       = "Foarte mare sau intens; avand sau aratand cunostinte sau intelegere profunda.",
                ExemplePropozitii = "The loss of his father had a [TERMEN] effect on his outlook on life.|She made a [TERMEN] observation that changed how we see the problem.|Reading that book had a [TERMEN] impact on my thinking.",
                CaleImagini       = null,
                Pronuntie         = "/prəˈfaʊnd/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "adjectiv"
            }
        };

        ctx.Cuvinte.AddRange(cuvinte);
        ctx.SaveChanges();
    }
}