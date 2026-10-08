# 🎨 Canviar Sprites

Guia pas a pas per substituir qualsevol sprite del joc sense tocar cap línia de codi.

---

## Prerequisits

Abans d'assignar qualsevol imatge, cal importar-la correctament a Unity:

1. Copia el fitxer `.png` (o `.jpg`) a la carpeta `Assets/Sprites/`
2. Selecciona la imatge al **Project panel** de Unity
3. A l'**Inspector**, comprova que:
   - **Texture Type** = `Sprite (2D and UI)`
   - **Pixels Per Unit** = `100` (o el valor consistent amb la resta del joc)
   - **Max Size** = `2048` o `4096` per a imatges grans
4. Fes clic a **Apply**

---

## On assignar cada sprite

Tots els sprites es gestionen des del `GameSkinData` del tema actiu.

**Ruta:** `Assets/ScriptableObjects/Skins/Skin_NomDelTema.asset`

---

### 🚀 Nau del jugador

**Secció: Player Ship**

| Camp | Descripció |
|------|------------|
| `Ship Sprite` | Sprite principal de la nau |
| `Ship Color` | Tint de color aplicat sobre el sprite (blanc = sense tint) |
| `Thruster Sprite` | Sprite del propulsor (visible quan s'accelera) |

**Mides recomanades:** 64×64 px o 128×128 px, fons transparent (PNG).

---

### 💥 Projectils / Làsers

**Secció: Projectiles / Lasers**

| Camp | Descripció |
|------|------------|
| `Projectile Sprite` | Forma del làser o bala |
| `Projectile Color` | Tint del projectil |
| `Projectile Scale` | Escala X/Y (per fer-lo més llarg, estret, etc.) |

**Mides recomanades:** 8×32 px (vertical), fons transparent.

---

### ☄️ Asteroides

**Secció: Asteroids**

Cada mida té el seu propi array de sprites. Pots assignar-ne **múltiples** — el joc en tria un aleatòriament per a cada asteroide generat.

| Camp | Descripció |
|------|------------|
| `Large Asteroid Sprites[]` | Sprites per a asteroides grans |
| `Medium Asteroid Sprites[]` | Sprites per a asteroides mitjans |
| `Small Asteroid Sprites[]` | Sprites per a asteroides petits |
| `Asteroid Tint` | Color aplicat sobre tots els sprites d'asteroide |

**Mides recomanades:**
- Gran: 128×128 px
- Mitjà: 64×64 px
- Petit: 32×32 px

**Com afegir múltiples sprites a l'array:**
1. Fes clic al botó `+` al costat de l'array
2. Arrossega el sprite al nou slot
3. Repeteix per a cada variant

---

### ❤️ Icona de vida (HUD)

**Secció: UI & HUD**

| Camp | Descripció |
|------|------------|
| `Life Icon Sprite` | Icona que representa cada vida al HUD |

Si es deixa buit, s'usa el `Ship Sprite` com a icona de vida.

**Mides recomanades:** 24×24 px o 32×32 px.

---

## Exemple pràctic: Reskin complet

```
1. Crea una carpeta Assets/Sprites/MeuTema/
2. Importa tots els sprites nous
3. Clic dret a Assets/ScriptableObjects/Skins/ → Create → Asteroids → Game Skin
4. Anomena'l Skin_MeuTema
5. Assigna:
   - Ship Sprite        → nau_meutema.png
   - Large Asteroid Sprites[0] → asteroide_gran.png
   - Large Asteroid Sprites[1] → asteroide_gran2.png  (variant)
   - Medium Asteroid Sprites[0] → asteroide_mitja.png
   - Small Asteroid Sprites[0]  → asteroide_petit.png
   - Life Icon Sprite   → icona_vida.png
6. Al SkinManager (GameObject "GameManagers"), afegeix la nova skin a "Available Skins"
7. Prem Play i T per veure el resultat
```

---

## Consells

- Usa sempre **fons transparent (PNG)** per als sprites de joc
- Mantén la **mateixa proporció** que els sprites originals per no distorsionar les col·lisions
- El sistema de col·lisions usa **Collider2D** i **no** depèn de la mida del sprite — però visualment quedará millor si els sprites i el collider coincideixen
