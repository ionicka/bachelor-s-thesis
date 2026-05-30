# FlashCards — Setup Baza de Date

## Prima instalare (calculator nou)

### 1. Instalează PostgreSQL
- Descarcă de la https://www.postgresql.org/download/
- User: `postgres`, Password: `postgres`, Port: `5432`

### 2. Creează baza de date
```sql
CREATE DATABASE FlashCards;
```

### 3. Rulează scriptul init
În pgAdmin → Query Tool pe baza `FlashCards`:
- Deschide `Database/init.sql`
- Apasă **F5** (Run)

### 4. Adaugă imaginile
Copiază folderul `images/` în:
```
FlashCards/Resources/Images/images/
```

### 5. Rulează aplicația
Visual Studio → F5

---

## Structura imaginilor
```
FlashCards/Resources/Images/images/
  perseverance_1.jpg
  perseverance_2.jpg
  resilient_1.jpg
  resilient_2.jpg
  ambiguous_1.jpg
  ambiguous_2.jpg
  eloquent_1.jpg
  eloquent_2.jpg
  catalyst_1.jpg
  catalyst_2.jpg
  meticulous_1.jpg
  meticulous_2.jpg
  serendipity_1.jpg
  serendipity_2.jpg
  innovative_1.jpg
  innovative_2.jpg
  empathy_1.jpg
  empathy_2.jpg
  profound_1.jpg
  profound_2.jpg
```

## .gitignore recomandat
```
# Nu include parola in git — schimba in LexaDbContext.cs
*.user
.vs/
bin/
obj/
```

## Connection String
In `FlashCards.Data/Context/LexaDbContext.cs`:
```csharp
public const string ConnectionString =
    "Host=localhost;Port=5432;Database=FlashCards;Username=postgres;Password=postgres";
```
