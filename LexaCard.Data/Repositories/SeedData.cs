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
                CaleImagini       = "perseverance1.webp|perseverance2.jpg",
                Pronuntie         = "/ˌpɜː.sɪˈvɪər.əns/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "resilient",
                Definitie         = "Able to recover quickly from difficult conditions or situations.",
                DefinitieRo       = "Capabil sa se recupereze rapid din conditii sau situatii dificile.",
                ExemplePropozitii = "She proved to be incredibly [TERMEN] after losing her job.|Children are often more [TERMEN] than adults give them credit for.|The [TERMEN] community rebuilt after the storm in record time.",
                CaleImagini       = "resilient1.jpg|resilient2.webp",
                Pronuntie         = "/rɪˈzɪl.i.ənt/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "ambiguous",
                Definitie         = "Having more than one possible meaning; not clear or decided.",
                DefinitieRo       = "Avand mai mult de un sens posibil; neclar sau nedecis.",
                ExemplePropozitii = "The manager gave an [TERMEN] answer that confused everyone.|His [TERMEN] smile left her wondering what he really meant.|The contract contained several [TERMEN] clauses that caused disputes.",
                CaleImagini       = "ambiguous.jpg",
                Pronuntie         = "/æmˈbɪɡ.ju.əs/",
                Nivel             = NivelCuvant.ElementarSuperior,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "eloquent",
                Definitie         = "Expressing ideas and opinions clearly and effectively, using language well.",
                DefinitieRo       = "Exprimand idei si opinii clar si eficient, folosind bine limbajul.",
                ExemplePropozitii = "The president gave an [TERMEN] speech that moved the entire audience.|She is [TERMEN] in three languages and often acts as interpreter.|His [TERMEN] defense of the proposal convinced the entire board.",
                CaleImagini       = "eloquent.jpg",
                Pronuntie         = "/ˈel.ə.kwənt/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "catalyst",
                Definitie         = "A person or thing that causes a change or makes something happen faster.",
                DefinitieRo       = "O persoana sau un lucru care cauzeaza o schimbare sau accelereaza un proces.",
                ExemplePropozitii = "The new law was a [TERMEN] for significant social change.|Her speech acted as a [TERMEN] for the protest movement.|Technological innovation has been a [TERMEN] for economic growth.",
                CaleImagini       = "catalyst.jpg",
                Pronuntie         = "/ˈkæt.ə.lɪst/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "meticulous",
                Definitie         = "Very careful and precise, paying great attention to every detail.",
                DefinitieRo       = "Foarte atent si precis, acordand mare atentie fiecarui detaliu.",
                ExemplePropozitii = "The surgeon was [TERMEN] in her preparation before every operation.|He kept [TERMEN] records of every transaction for decades.|Her [TERMEN] research left no room for error in the final report.",
                CaleImagini       = "meticulous.webp",
                Pronuntie         = "/məˈtɪk.jʊ.ləs/",
                Nivel             = NivelCuvant.Avansat,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "serendipity",
                Definitie         = "The occurrence of events by chance in a happy or beneficial way.",
                DefinitieRo       = "Aparitia evenimentelor din intamplare intr-un mod fericit sau benefic.",
                ExemplePropozitii = "Meeting my best friend on that train was pure [TERMEN].|The discovery of penicillin is often cited as a case of [TERMEN].|By a wonderful [TERMEN], they ended up at the same hotel.",
                CaleImagini       = "serendipity.jpg",
                Pronuntie         = "/ˌser.ənˈdɪp.ɪ.ti/",
                Nivel             = NivelCuvant.Avansat,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "innovative",
                Definitie         = "Introducing or using new ideas, methods, or inventions; creative and original.",
                DefinitieRo       = "Introducand sau folosind idei, metode sau inventii noi; creativ si original.",
                ExemplePropozitii = "The company is known for its [TERMEN] approach to problem solving.|She designed an [TERMEN] system that cut costs by half.|The [TERMEN] startup disrupted the entire industry within two years.",
                CaleImagini       = "innovative.jpg",
                Pronuntie         = "/ˈɪn.ə.veɪ.tɪv/",
                Nivel             = NivelCuvant.ElementarSuperior,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "empathy",
                Definitie         = "The ability to understand and share the feelings of another person.",
                DefinitieRo       = "Abilitatea de a intelege si de a impartasi sentimentele unei alte persoane.",
                ExemplePropozitii = "Good nurses show great [TERMEN] toward their patients every day.|[TERMEN] is essential for building strong and lasting relationships.|She listened with [TERMEN] as her friend described the difficult situation.",
                CaleImagini       = "empathy1.jpg|empathy2.jpg",
                Pronuntie         = "/ˈem.pə.θi/",
                Nivel             = NivelCuvant.ElementarSuperior,
                Etichete          = "substantiv"
            },
            new() {
                Termen            = "profound",
                Definitie         = "Very great or intense; having or showing great knowledge or insight.",
                DefinitieRo       = "Foarte mare sau intens; avand sau aratand cunostinte sau intelegere profunda.",
                ExemplePropozitii = "The loss of his father had a [TERMEN] effect on his outlook on life.|She made a [TERMEN] observation that changed how we see the problem.|Reading that book had a [TERMEN] impact on my thinking.",
                CaleImagini       = "profound1.jpg|profound2.webp",
                Pronuntie         = "/prəˈfaʊnd/",
                Nivel             = NivelCuvant.Intermediar,
                Etichete          = "adjectiv"
            },
            new() {
                Termen            = "tenacious",
                Definitie         = "Holding firmly to a purpose or belief; not giving up easily.",
                DefinitieRo       = "Care se tine ferm de un scop sau o convingere; care nu renunta usor.",
                ExemplePropozitii = "She was [TERMEN] in her pursuit of justice for her community.|The [TERMEN] negotiator refused to accept the first offer.|His [TERMEN] grip on the project ensured it was completed on time.",
                CaleImagini       = "tenacious1.jpg|tenacious2.webp",
                Pronuntie = "/təˈneɪ.ʃəs/",
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "diligent",
                Definitie         = "Showing careful and persistent effort in work or duties.",
                DefinitieRo       = "Care depune efort atent si persistent in munca sau indatoriri.",
                ExemplePropozitii = "A [TERMEN] student reviews notes every day after class.|She was [TERMEN] in checking every detail before submitting the report.|His [TERMEN] work ethic earned him a promotion within six months.",
                CaleImagini       = "diligent1.jpg|diligent2.jpg",
                Pronuntie = "/ˈdɪl.ɪ.dʒənt/", 
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "integrity",
                Definitie         = "The quality of being honest and having strong moral principles.",
                DefinitieRo       = "Calitatea de a fi sincer si de a avea principii morale solide.",
                ExemplePropozitii = "A good leader must act with honesty and [TERMEN] at all times.|She demonstrated her [TERMEN] by refusing the bribe.|The company built its reputation on a foundation of [TERMEN].",
                CaleImagini       = "integrity1.jpg|integrity2.webp",
                Pronuntie = "/ɪnˈteɡ.rɪ.ti/", 
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "substantiv"
            },
            new() {
                Termen            = "curiosity",
                Definitie         = "A strong desire to know or learn something.",
                DefinitieRo       = "O dorinta puternica de a sti sau de a afla ceva.",
                ExemplePropozitii = "Her [TERMEN] about science led her to become a researcher.|[TERMEN] is the engine that drives all great discoveries.|The child's [TERMEN] led him to take apart every toy he owned.",
                CaleImagini       = "curiosity1.jpg|curiosity2.webp",
                Pronuntie = "/ˌkjʊər.iˈɒs.ɪ.ti/", 
                Nivel = NivelCuvant.Incepator,
                Etichete = "substantiv"
            },
            new() {
                Termen            = "versatile",
                Definitie         = "Able to adapt or be adapted to many different functions or activities.",
                DefinitieRo       = "Capabil sa se adapteze la multe functii sau activitati diferite.",
                ExemplePropozitii = "She is a [TERMEN] musician who can play five instruments.|A [TERMEN] employee is an asset to any organisation.|The new software is [TERMEN] enough to handle all our needs.",
                CaleImagini       = "versatile1.jpg|versatile2.webp",
                Pronuntie = "/ˈvɜː.sə.taɪl/",
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "empowerment",
                Definitie         = "The process of giving someone more confidence, freedom, or power.",
                DefinitieRo       = "Procesul de a oferi cuiva mai multa incredere, libertate sau putere.",
                ExemplePropozitii = "Education is the greatest tool for [TERMEN] in any society.|The programme focused on the [TERMEN] of women in rural areas.|Financial [TERMEN] allows people to make their own life choices.",
                CaleImagini       = "empowerment1.webp|empowerment2.jpg",
                Pronuntie = "/ɪmˈpaʊ.ər.mənt/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "substantiv"
            },
            new() {
                Termen            = "coherent",
                Definitie         = "Logical and consistent; easy to understand.",
                DefinitieRo       = "Logic si consistent; usor de inteles.",
                ExemplePropozitii = "The essay needs a more [TERMEN] structure to make sense.|She gave a [TERMEN] explanation of the complex situation.|His argument was not [TERMEN] enough to convince the committee.",
                CaleImagini       = "coherent1.jpg|coherent2.jpg",
                Pronuntie = "/kəʊˈhɪər.ənt/",
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "nostalgia",
                Definitie         = "A sentimental longing for the past, typically for a period with happy associations.",
                DefinitieRo       = "Dor sentimental pentru trecut, de obicei pentru o perioada cu amintiri fericite.",
                ExemplePropozitii = "Smelling fresh bread filled her with [TERMEN] for her grandmother's kitchen.|The old song brought waves of [TERMEN] flooding back.|He felt a deep [TERMEN] for his carefree childhood summers.",
                CaleImagini       = "nostalgia1.webp|nostalgia2.webp",
                Pronuntie = "/nɒˈstæl.dʒə/", 
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "substantiv"
            },
            new() {
                Termen            = "sustainable",
                Definitie         = "Able to be maintained over the long term without depleting resources.",
                DefinitieRo       = "Capabil sa fie mentinut pe termen lung fara a epuiza resursele.",
                ExemplePropozitii = "We need to develop [TERMEN] energy sources for the future.|The farm uses [TERMEN] methods that protect the environment.|Building a [TERMEN] business requires long-term thinking.",
                CaleImagini       = "sustainable1.webp|sustainable2.jpg",
                Pronuntie = "/səˈsteɪ.nə.bəl/",
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "transparent",
                Definitie         = "Open and honest; not hiding information or motives.",
                DefinitieRo       = "Deschis si sincer; care nu ascunde informatii sau motive.",
                ExemplePropozitii = "The government needs to be [TERMEN] about how it spends public money.|She was completely [TERMEN] about her reasons for leaving.|A [TERMEN] hiring process helps build trust within the organisation.",
                CaleImagini       = "transparent1.jpg|transparent.webp",
                Pronuntie = "/trænsˈpær.ənt/",
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "initiative",
                Definitie         = "The ability to assess and act independently without being told what to do.",
                DefinitieRo       = "Capacitatea de a evalua si a actiona independent fara a fi instruit.",
                ExemplePropozitii = "She took the [TERMEN] to organise the fundraiser herself.|Employers value workers who show [TERMEN] and creativity.|He launched the community garden as a personal [TERMEN].",
                CaleImagini       = "initiative1.webp|initiative2.jpg",
                Pronuntie = "/ɪˈnɪʃ.ə.tɪv/",
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "substantiv"
            },
            new() {
                Termen            = "compassion",
                Definitie         = "Sympathetic concern for the suffering of others, combined with a desire to help.",
                DefinitieRo       = "Grija simpatica pentru suferinta altora, insotita de dorinta de a ajuta.",
                ExemplePropozitii = "A great doctor treats patients with [TERMEN] and respect.|She showed great [TERMEN] toward the homeless people in her city.|[TERMEN] for others is at the heart of every caring profession.",
                CaleImagini       = "compassion1.jpg|compassion2.jpg",
                Pronuntie = "/kəmˈpæʃ.ən/",
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "substantiv"
            },
            new() {
                Termen            = "deliberate",
                Definitie         = "Done consciously and intentionally; carefully considered.",
                DefinitieRo       = "Facut constient si intentionat; bine gandit.",
                ExemplePropozitii = "The mistake was [TERMEN], not an accident.|She made a [TERMEN] choice to leave her high-paying job.|His [TERMEN] pace showed he was in full control of the situation.",
                CaleImagini       = "deliberate1.jpg|deliberate2.jpg",
                Pronuntie = "/dɪˈlɪb.ər.ət/",
                Nivel = NivelCuvant.Intermediar,
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "insight",
                Definitie         = "A deep and accurate understanding of a person or situation.",
                DefinitieRo       = "O intelegere profunda si exacta a unei persoane sau situatii.",
                ExemplePropozitii = "The book offered valuable [TERMEN] into human psychology.|Her [TERMEN] into the problem helped the team find a solution quickly.|Years of experience gave him remarkable [TERMEN] into people's behaviour.",
                CaleImagini       = "insight1.jpg|insight2.jpg",
                Pronuntie = "/ˈɪn.saɪt/", 
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "substantiv"
            },
            new() {
                Termen            = "momentum",
                Definitie         = "The force or speed of movement; the impetus gained by a moving object.",
                DefinitieRo       = "Forta sau viteza miscarii; impulsul castigat de un obiect in miscare.",
                ExemplePropozitii = "The team gained [TERMEN] after scoring the first goal.|Once you start exercising regularly, [TERMEN] keeps you going.|The political movement lost [TERMEN] after its leader resigned.",
                CaleImagini       = "momentum1.jpg|momentum2.webp",
                Pronuntie = "/məˈmen.təm/", 
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "substantiv"
            },
            new() {
                Termen            = "anticipate",
                Definitie         = "To expect or predict something and prepare for it in advance.",
                DefinitieRo       = "A se astepta la ceva si a se pregati pentru aceasta in avans.",
                ExemplePropozitii = "We did not [TERMEN] such a large turnout at the event.|Good managers [TERMEN] problems before they arise.|She failed to [TERMEN] how difficult the transition would be.",
                CaleImagini       = "anticipate1.jpg",
                Pronuntie = "/ænˈtɪs.ɪ.peɪt/", 
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "verb"
            },
            new() {
                Termen            = "collaborate",
                Definitie         = "To work jointly with others on a project or activity.",
                DefinitieRo       = "A lucra impreuna cu altii la un proiect sau activitate.",
                ExemplePropozitii = "The two companies decided to [TERMEN] on the new project.|Artists often [TERMEN] across disciplines to produce original work.|Students who [TERMEN] tend to achieve better academic outcomes.",
                CaleImagini       = "collaborate1.webp|collaborate2.webp",
                Pronuntie = "/kəˈlæb.ə.reɪt/", 
                Nivel = NivelCuvant.ElementarSuperior,
                Etichete = "verb"
            },
            new() {
                Termen            = "authentic",
                Definitie         = "Real or genuine; not a copy or imitation.",
                DefinitieRo       = "Real sau autentic; nu o copie sau imitatie.",
                ExemplePropozitii = "The museum displayed [TERMEN] artefacts from ancient Egypt.|Being [TERMEN] in your relationships builds deeper trust.|She gave an [TERMEN] performance that moved the whole audience.",
                CaleImagini       = "authentic1.jpg",
                Pronuntie = "/ɔːˈθen.tɪk/", 
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "inevitable",
                Definitie         = "Certain to happen and unable to be avoided.",
                DefinitieRo       = "Cert ca se va intampla si imposibil de evitat.",
                ExemplePropozitii = "Change is [TERMEN]; we must learn to adapt to it.|Conflict seemed [TERMEN] given the tension between the two sides.|It was [TERMEN] that the old factory would eventually close.",
                CaleImagini       = "inevitable1.webp",
                Pronuntie = "/ɪnˈev.ɪ.tə.bəl/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "skeptical",
                Definitie         = "Not easily convinced; having doubts about something.",
                DefinitieRo       = "Care nu se convinge usor; care are indoieli despre ceva.",
                ExemplePropozitii = "She was [TERMEN] about the new treatment's effectiveness.|Many scientists remain [TERMEN] of the findings.|He was [TERMEN] that the project could be completed on time.",
                CaleImagini       = "skeptical1.jpg|skeptical2.jpg",
                Pronuntie = "/ˈskep.tɪ.kəl/", 
                Nivel = NivelCuvant.Intermediar,
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "dynamic",
                Definitie         = "Characterised by constant change, activity, and progress.",
                DefinitieRo       = "Caracterizat de schimbare, activitate si progres constant.",
                ExemplePropozitii = "She is a [TERMEN] leader who brings energy to every meeting.|The [TERMEN] market requires businesses to adapt quickly.|Their [TERMEN] performance captivated the entire audience.",
                CaleImagini       = "dynamic1.jpg|dynamic2.webp",
                Pronuntie = "/daɪˈnæm.ɪk/", 
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "facilitate",
                Definitie         = "To make an action or process easier or more achievable.",
                DefinitieRo       = "A face o actiune sau un proces mai usor sau mai realizabil.",
                ExemplePropozitii = "Good communication can [TERMEN] teamwork in any organisation.|The new software will [TERMEN] faster data processing.|Her role was to [TERMEN] discussions between the two parties.",
                CaleImagini       = "facilitate1.jpg",
                Pronuntie = "/fəˈsɪl.ɪ.teɪt/",
                Nivel = NivelCuvant.Intermediar,
                Etichete = "verb"
            },
            new() {
                Termen            = "pragmatic",
                Definitie         = "Dealing with problems in a sensible and realistic way rather than theoretically.",
                DefinitieRo       = "Care rezolva problemele intr-un mod sensibil si realist, nu teoretic.",
                ExemplePropozitii = "We need a [TERMEN] solution, not just theoretical ideas.|She took a [TERMEN] approach to managing the budget cuts.|A [TERMEN] leader focuses on what can actually be achieved.",
                CaleImagini       = "pragmatic1.webp",
                Pronuntie = "/præɡˈmæt.ɪk/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "spontaneous",
                Definitie         = "Done or occurring suddenly and without planning.",
                DefinitieRo       = "Facut sau care apare brusc si fara planificare.",
                ExemplePropozitii = "The [TERMEN] trip to Paris turned out to be the best vacation ever.|Her [TERMEN] laughter filled the room with joy.|He made a [TERMEN] decision to quit his job and travel the world.",
                CaleImagini       = "spontaneous1.jpg|spontaneous2.webp", 
                Pronuntie = "/spɒnˈteɪ.ni.əs/",
                Nivel = NivelCuvant.Intermediar,
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "assertive",
                Definitie         = "Confident and direct in expressing one's opinions and needs.",
                DefinitieRo       = "Increzator si direct in exprimarea opiniilor si nevoilor proprii.",
                ExemplePropozitii = "Being [TERMEN] at work helped him get the promotion he deserved.|She learned to be more [TERMEN] when dealing with difficult clients.|An [TERMEN] communication style prevents misunderstandings.",
                CaleImagini       = "assertive1.jpg|assertive2.jpg",
                Pronuntie = "/əˈsɜː.tɪv/", 
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "vulnerable",
                Definitie         = "Exposed to the possibility of being attacked or harmed.",
                DefinitieRo       = "Expus posibilitatii de a fi atacat sau ranit.",
                ExemplePropozitii = "Young children are particularly [TERMEN] to infections in winter.|She felt [TERMEN] sharing her personal story with strangers.|The country's economy was [TERMEN] to changes in oil prices.",
                CaleImagini       = "vulnerable1.webp",
                Pronuntie = "/ˈvʌl.nər.ə.bəl/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "concise",
                Definitie         = "Giving a lot of information clearly and in few words.",
                DefinitieRo       = "Care ofera multa informatie clar si in putine cuvinte.",
                ExemplePropozitii = "Please write a [TERMEN] summary of the main points.|Her [TERMEN] explanation made the concept easy to grasp.|A [TERMEN] email is more likely to be read and acted upon.",
                CaleImagini       = "concise1.webp",
                Pronuntie = "/kənˈsaɪs/",
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "scrutiny",
                Definitie         = "Critical observation or examination of something.",
                DefinitieRo       = "Observatie sau examinare critica a ceva.",
                ExemplePropozitii = "The politician's decisions came under intense public [TERMEN].|Every detail of the contract was subjected to careful [TERMEN].|Under [TERMEN], the company's accounts revealed serious problems.",
                CaleImagini       = "scrutiny1.jpg", 
                Pronuntie = "/ˈskruː.tɪ.ni/", 
                Nivel = NivelCuvant.Avansat,
                Etichete = "substantiv"
            },
            new() {
                Termen            = "dedicate",
                Definitie         = "To devote time and effort to a particular purpose or person.",
                DefinitieRo       = "A dedica timp si efort unui scop sau unei persoane.",
                ExemplePropozitii = "She decided to [TERMEN] her life to helping others.|He chose to [TERMEN] the book to his late mother.|The foundation was created to [TERMEN] resources to cancer research.",
                CaleImagini       = "dedicate1.jpg",
                Pronuntie = "/ˈded.ɪ.keɪt/", 
                Nivel = NivelCuvant.Incepator, 
                Etichete = "verb"
            },
            new() {
                Termen            = "implication",
                Definitie         = "A conclusion that can be drawn from something, though not directly stated.",
                DefinitieRo       = "O concluzie ce poate fi trasa din ceva, desi nu este spusa direct.",
                ExemplePropozitii = "The [TERMEN] of his words was clear even though he said nothing directly.|We must consider the long-term [TERMEN]s of this decision.|The study has serious [TERMEN]s for public health policy.",
                CaleImagini       = "implication1.jpg",
                Pronuntie = "/ˌɪm.plɪˈkeɪ.ʃən/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "substantiv"
            },
            new() {
                Termen            = "endeavour",
                Definitie         = "To try hard to achieve something; a serious attempt.",
                DefinitieRo       = "A incerca din greu sa realizezi ceva; o tentativa serioasa.",
                ExemplePropozitii = "She will [TERMEN] to complete the project before the deadline.|Scientific [TERMEN] has improved the quality of human life enormously.|Despite every [TERMEN], the rescue mission was unsuccessful.",
                CaleImagini       = "endeavour1.webp",
                Pronuntie = "/ɪnˈdev.ər/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "verb"
            },
            new() {
                Termen            = "empathise",
                Definitie         = "To understand and share the feelings of another person.",
                DefinitieRo       = "A intelege si a impartasi sentimentele unei alte persoane.",
                ExemplePropozitii = "It is hard to [TERMEN] with someone whose experience is very different.|A good therapist learns to [TERMEN] without judgment.|She could [TERMEN] with his frustration because she had felt the same.",
                CaleImagini       = "empathise1.jpg",
                Pronuntie = "/ˈem.pə.θaɪz/",
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "verb"
            },
            new() {
                Termen            = "resilience",
                Definitie         = "The capacity to recover quickly from difficulties; mental toughness.",
                DefinitieRo       = "Capacitatea de a se recupera rapid din dificultati; rezistenta mentala.",
                ExemplePropozitii = "Her [TERMEN] in the face of adversity inspired everyone around her.|Building [TERMEN] in children helps them cope with life's challenges.|The team showed remarkable [TERMEN] after losing three matches in a row.",
                CaleImagini       = "resilience1.jpg",
                Pronuntie = "/rɪˈzɪl.i.əns/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "substantiv"
            },
            new() {
                Termen            = "eloquence",
                Definitie         = "Fluent or persuasive speaking or writing.",
                DefinitieRo       = "Vorbire sau scriere fluenta sau persuasiva.",
                ExemplePropozitii = "The lawyer's [TERMEN] in the courtroom won over the jury.|Her [TERMEN] made even complex topics easy to understand.|He was admired for the [TERMEN] of his writing and speeches.",
                CaleImagini       = "eloquence1.jpg",
                Pronuntie = "/ˈel.ə.kwəns/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "substantiv"
            },
            new() {
                Termen            = "subtle",
                Definitie         = "So delicate or precise as to be difficult to analyse or describe.",
                DefinitieRo       = "Atat de delicat sau precis incat este greu de analizat sau descris.",
                ExemplePropozitii = "There was a [TERMEN] difference in tone between the two speeches.|She made a [TERMEN] change to the design that improved it greatly.|His [TERMEN] humour was lost on most of the audience.",
                CaleImagini       = "subtle1.jpg",
                Pronuntie = "/ˈsʌt.əl/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "adjectiv"
            },
            new() {
                Termen            = "comprehend",
                Definitie         = "To understand something fully.",
                DefinitieRo       = "A intelege ceva pe deplin.",
                ExemplePropozitii = "It is difficult to [TERMEN] the scale of the disaster.|Young children cannot fully [TERMEN] the consequences of their actions.|She struggled to [TERMEN] why he had made such a poor decision.",
                CaleImagini       = "comprehend1.jpg",
                Pronuntie = "/ˌkɒm.prɪˈhend/",
                Nivel = NivelCuvant.ElementarSuperior, 
                Etichete = "verb"
            },
            new() {
                Termen            = "paradigm",
                Definitie         = "A typical example or pattern of something; a model or framework.",
                DefinitieRo       = "Un exemplu sau model tipic al ceva; un cadru de referinta.",
                ExemplePropozitii = "The internet created a new [TERMEN] for global communication.|This discovery challenges the existing scientific [TERMEN].|The company's success became a [TERMEN] for the entire industry.",
                CaleImagini       = "paradigm1.jpg",
                Pronuntie = "/ˈpær.ə.daɪm/",
                Nivel = NivelCuvant.Avansat, 
                Etichete = "substantiv"
            },
            new() {
                Termen            = "fluctuate",
                Definitie         = "To rise and fall irregularly in number or amount.",
                DefinitieRo       = "A creste si scadea neregulat ca numar sau cantitate.",
                ExemplePropozitii = "Stock prices tend to [TERMEN] throughout the trading day.|Her mood seemed to [TERMEN] depending on the weather.|Energy costs [TERMEN] significantly between summer and winter.",
                CaleImagini       = "fluctuate1.jpg|fluctuate2.jpg",
                Pronuntie = "/ˈflʌk.tʃu.eɪt/", 
                Nivel = NivelCuvant.Intermediar, 
                Etichete = "verb"
            },
        };

        ctx.Cuvinte.AddRange(cuvinte);
        ctx.SaveChanges();
    }
}