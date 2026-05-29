-- ================================================================
-- LexaCard Database Init Script
-- Rulează: psql -U postgres -d lexacard -f init.sql
-- sau copiaza in pgAdmin Query Tool si ruleaza
-- ================================================================

-- Sterge tot inclusiv constraints si secvente
DROP TABLE IF EXISTS "RaspunsuriDetaliate" CASCADE;
DROP TABLE IF EXISTS "ProgresCuvinte"      CASCADE;
DROP TABLE IF EXISTS "SesiuniStudiu"       CASCADE;
DROP TABLE IF EXISTS "Cuvinte"             CASCADE;
DROP TABLE IF EXISTS "Utilizatori"         CASCADE;

-- Sterge secvente ramase
DROP SEQUENCE IF EXISTS "Utilizatori_Id_seq"        CASCADE;
DROP SEQUENCE IF EXISTS "Cuvinte_Id_seq"             CASCADE;
DROP SEQUENCE IF EXISTS "ProgresCuvinte_Id_seq"      CASCADE;
DROP SEQUENCE IF EXISTS "SesiuniStudiu_Id_seq"       CASCADE;
DROP SEQUENCE IF EXISTS "RaspunsuriDetaliate_Id_seq" CASCADE;

-- Sterge constraint orfan daca exista
DO $$ BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'uq_progres_utilizator_cuvant'
    ) THEN
        EXECUTE 'ALTER TABLE IF EXISTS "ProgresCuvinte" DROP CONSTRAINT IF EXISTS "uq_progres_utilizator_cuvant"';
    END IF;
END $$;

CREATE TABLE "Utilizatori" (
    "Id"                  SERIAL PRIMARY KEY,
    "NumeUtilizator"      VARCHAR(100) NOT NULL UNIQUE,
    "Email"               VARCHAR(200) NOT NULL UNIQUE,
    "ParolaHash"          TEXT NOT NULL,
    "DataInregistrarii"   TIMESTAMP NOT NULL DEFAULT NOW(),
    "UltimaAutentificare" TIMESTAMP,
    "CarduriNoiPerZi"     INTEGER NOT NULL DEFAULT 10,
    "MaxCarduriPerZi"     INTEGER NOT NULL DEFAULT 50,
    "AvatarUrl"           VARCHAR(500),
    "Monede"              INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE "Cuvinte" (
    "Id"                SERIAL PRIMARY KEY,
    "Termen"            VARCHAR(200) NOT NULL,
    "Definitie"         TEXT NOT NULL,
    "DefinitieRo"       TEXT,
    "ExemplePropozitii" TEXT NOT NULL,
    "CaleImagini"       TEXT,
    "Pronuntie"         VARCHAR(200),
    "Limba"             VARCHAR(50)  NOT NULL DEFAULT 'engleza',
    "Nivel"             INTEGER      NOT NULL DEFAULT 1,
    "Etichete"          VARCHAR(500),
    "DataAdaugarii"     TIMESTAMP    NOT NULL DEFAULT NOW()
);

CREATE TABLE "ProgresCuvinte" (
    "Id"                      SERIAL PRIMARY KEY,
    "UtilizatorId"            INTEGER  NOT NULL REFERENCES "Utilizatori"("Id") ON DELETE CASCADE,
    "CuvantId"                INTEGER  NOT NULL REFERENCES "Cuvinte"("Id")     ON DELETE CASCADE,
    "NivelCunoastere"         SMALLINT NOT NULL DEFAULT 0,
    "IntervalCurentZile"      INTEGER  NOT NULL DEFAULT 0,
    "DataUrmatoareiRevizuiri" DATE,
    "DataUltimeiRevizuiri"    TIMESTAMP,
    "NrRaspunsuriCorecte"     INTEGER  NOT NULL DEFAULT 0,
    "NrRaspunsuriGresite"     INTEGER  NOT NULL DEFAULT 0,
    CONSTRAINT "uq_progres_util_cuv" UNIQUE ("UtilizatorId", "CuvantId")
);

DROP INDEX IF EXISTS "ix_progres_azi";
CREATE INDEX "ix_progres_azi" ON "ProgresCuvinte" ("UtilizatorId", "DataUrmatoareiRevizuiri");

CREATE TABLE "SesiuniStudiu" (
    "Id"              SERIAL PRIMARY KEY,
    "UtilizatorId"    INTEGER   NOT NULL REFERENCES "Utilizatori"("Id") ON DELETE CASCADE,
    "DataSesiunii"    TIMESTAMP NOT NULL DEFAULT NOW(),
    "DataSfarsitului" TIMESTAMP,
    "DurataSec"       INTEGER NOT NULL DEFAULT 0,
    "NrCarduriVazute" INTEGER NOT NULL DEFAULT 0,
    "NrCorect"        INTEGER NOT NULL DEFAULT 0,
    "NrGresit"        INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE "RaspunsuriDetaliate" (
    "Id"              SERIAL PRIMARY KEY,
    "ProgresCuvantId" INTEGER   NOT NULL REFERENCES "ProgresCuvinte"("Id") ON DELETE CASCADE,
    "SesiuneId"       INTEGER   REFERENCES "SesiuniStudiu"("Id") ON DELETE SET NULL,
    "TipRaspuns"      INTEGER   NOT NULL DEFAULT 0,
    "Calitate"        INTEGER   NOT NULL DEFAULT 0,
    "EsteCorect"      BOOLEAN   NOT NULL DEFAULT FALSE,
    "TimpRaspunsSec"  INTEGER   NOT NULL DEFAULT 0,
    "TextTastat"      TEXT,
    "Timestamp"       TIMESTAMP NOT NULL DEFAULT NOW()
);

INSERT INTO "Cuvinte" ("Id","Termen","Definitie","DefinitieRo","ExemplePropozitii","CaleImagini","Pronuntie","Nivel","Etichete") VALUES
(1,'perseverance','The quality of continuing to try to achieve a particular aim despite difficulties.','Calitatea de a continua sa incerci sa atingi un scop in ciuda dificultatilor.','Her [TERMEN] in learning the language finally paid off after two years.|Despite many failures, his [TERMEN] eventually led to success.|The athlete''s [TERMEN] through injury inspired the whole team.',NULL,'/ˌpɜː.sɪˈvɪər.əns/',4,'substantiv'),
(2,'resilient','Able to recover quickly from difficult conditions or situations.','Capabil sa se recupereze rapid din conditii sau situatii dificile.','She proved to be incredibly [TERMEN] after losing her job.|Children are often more [TERMEN] than adults give them credit for.|The [TERMEN] community rebuilt after the storm in record time.',NULL,'/rɪˈzɪl.i.ənt/',4,'adjectiv'),
(3,'ambiguous','Having more than one possible meaning; not clear or decided.','Avand mai mult de un sens posibil; neclar sau nedecis.','The manager gave an [TERMEN] answer that confused everyone.|His [TERMEN] smile left her wondering what he really meant.|The contract contained several [TERMEN] clauses that caused disputes.',NULL,'/æmˈbɪɡ.ju.əs/',3,'adjectiv'),
(4,'eloquent','Expressing ideas and opinions clearly and effectively, using language well.','Exprimand idei si opinii clar si eficient, folosind bine limbajul.','The president gave an [TERMEN] speech that moved the entire audience.|She is [TERMEN] in three languages and often acts as interpreter.|His [TERMEN] defense of the proposal convinced the entire board.',NULL,'/ˈel.ə.kwənt/',4,'adjectiv'),
(5,'catalyst','A person or thing that causes a change or makes something happen faster.','O persoana sau un lucru care cauzeaza o schimbare sau accelereaza un proces.','The new law was a [TERMEN] for significant social change.|Her speech acted as a [TERMEN] for the protest movement.|Technological innovation has been a [TERMEN] for economic growth.',NULL,'/ˈkæt.ə.lɪst/',4,'substantiv'),
(6,'meticulous','Very careful and precise, paying great attention to every detail.','Foarte atent si precis, acordand mare atentie fiecarui detaliu.','The surgeon was [TERMEN] in her preparation before every operation.|He kept [TERMEN] records of every transaction for decades.|Her [TERMEN] research left no room for error in the final report.',NULL,'/məˈtɪk.jʊ.ləs/',5,'adjectiv'),
(7,'serendipity','The occurrence of events by chance in a happy or beneficial way.','Aparitia evenimentelor din intamplare intr-un mod fericit sau benefic.','Meeting my best friend on that train was pure [TERMEN].|The discovery of penicillin is often cited as a case of [TERMEN].|By a wonderful [TERMEN], they ended up at the same hotel.',NULL,'/ˌser.ənˈdɪp.ɪ.ti/',5,'substantiv'),
(8,'innovative','Introducing or using new ideas, methods, or inventions; creative and original.','Introducand sau folosind idei, metode sau inventii noi; creativ si original.','The company is known for its [TERMEN] approach to problem solving.|She designed an [TERMEN] system that cut costs by half.|The [TERMEN] startup disrupted the entire industry within two years.',NULL,'/ˈɪn.ə.veɪ.tɪv/',3,'adjectiv'),
(9,'empathy','The ability to understand and share the feelings of another person.','Abilitatea de a intelege si de a impartasi sentimentele unei alte persoane.','Good nurses show great [TERMEN] toward their patients every day.|[TERMEN] is essential for building strong and lasting relationships.|She listened with [TERMEN] as her friend described the difficult situation.',NULL,'/ˈem.pə.θi/',3,'substantiv'),
(10,'profound','Very great or intense; having or showing great knowledge or insight.','Foarte mare sau intens; avand sau aratand cunostinte sau intelegere profunda.','The loss of his father had a [TERMEN] effect on his outlook on life.|She made a [TERMEN] observation that changed how we see the problem.|Reading that book had a [TERMEN] impact on my thinking.',NULL,'/prəˈfaʊnd/',4,'adjectiv');

SELECT setval('"Cuvinte_Id_seq"', 10);