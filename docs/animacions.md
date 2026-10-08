# 🎬 Afegir Animacions

Guia per substituir els sprites estàtics de la nau i els asteroides per animacions 2D de Unity.

> **Nota:** Les animacions i els sprites estàtics **coexisteixen** al sistema de skins. Si un camp d'Animator és buit, el sistema usa l'sprite automàticament. Pots animar la nau i mantenir sprites estàtics als asteroides, o qualsevol combinació.

---

## Com funciona

El sistema usa `RuntimeAnimatorController` al `GameSkinData`. Quan s'aplica una skin:

- Si el camp `Ship Animator` té un controller → s'afegeix/activa el component `Animator` a la nau
- Si el camp és buit → s'usa l'sprite estàtic (`Ship Sprite`)

El mateix lògica s'aplica per a cada mida d'asteroide.

---

## Animació de la nau

### Pas 1 — Preparar el spritesheet

Importa el spritesheet a `Assets/Sprites/`:
- **Texture Type** = `Sprite (2D and UI)`
- **Sprite Mode** = `Multiple`
- Fes clic a **Sprite Editor** → **Slice** per tallar els fotogrames

### Pas 2 — Crear els clips d'animació

1. Selecciona el prefab **Player** a la Hierarchy
2. Obre **Window → Animation → Animation**
3. Fes clic a **Create** i guarda el clip a `Assets/Animations/`

**Clips recomanats per a la nau:**

| Clip | Descripció |
|------|------------|
| `Ship_Idle` | Nau en repòs (1 fotograma o animació suau) |
| `Ship_Thrusting` | Nau accelerant (motor encès, flama, etc.) |

Per a cada clip:
- Selecciona el clip al desplegable
- Arrossega els sprites corresponents a la línia de temps de l'Animation panel

### Pas 3 — Crear l'Animator Controller

1. Clic dret a `Assets/Animations/` → **Create → Animator Controller**
2. Anomena'l `ShipAnimator`
3. Obre-l'hi fent doble clic

**Configuració a l'Animator:**

```
Parameters:
  + Bool: "IsThrusting"

States:
  Ship_Idle  (estat per defecte)
  Ship_Thrusting

Transicions:
  Ship_Idle → Ship_Thrusting   [condició: IsThrusting = true]
  Ship_Thrusting → Ship_Idle   [condició: IsThrusting = false]
```

**Important:** Desmarca **"Has Exit Time"** a les transicions per a canvis immediats.

### Pas 4 — Assignar al GameSkinData

Obre el teu `GameSkinData` i a la secció **Animació · Nau**:

| Camp | Valor |
|------|-------|
| `Ship Animator` | Arrossega `ShipAnimator` aquí |
| `Thrust Param Name` | `IsThrusting` (ha de coincidir exactament) |

---

## Animació dels asteroides

Els asteroides generalment tenen una animació de **rotació o pulsació** en loop, sense condicions.

### Pas 1 — Crear el clip

1. Selecciona el prefab de l'asteroide (ex: `Asteroid_Large`)
2. **Window → Animation → Animation → Create**
3. Afegeix els fotogrames del spritesheet a la línia de temps
4. Activa el **loop** al clip (Inspector del clip → `Loop Time = true`)

### Pas 2 — Crear l'Animator Controller

1. **Create → Animator Controller** → ex: `AsteroidLargeAnimator`
2. A l'Animator, afegeix l'estat amb el clip en loop
3. Estableix-lo com a **estat per defecte** (clic dret → Set as Layer Default State)
4. No cal cap paràmetre — és un loop continu

### Pas 3 — Assignar al GameSkinData

A la secció **Animació · Asteroides**:

| Camp | Valor |
|------|-------|
| `Large Asteroid Animator` | AsteroidLargeAnimator |
| `Medium Asteroid Animator` | AsteroidMediumAnimator |
| `Small Asteroid Animator` | AsteroidSmallAnimator |

Pots reutilitzar el **mateix controller** per a totes les mides si vols la mateixa animació.

---

## Canvi de skin en calent

Quan el jugador prem **T** per canviar de skin durant la partida:
- Si la nova skin té `Ship Animator` → el controller es recarrega a l'instant
- Si la nova skin no té animator → el sistema desactiva l'Animator i usa l'sprite
- Els asteroides ja generats s'actualitzen immediatament

---

## Consells

- Usa **"Has Exit Time" = false** a les transicions per evitar retards
- Posa **"Transition Duration = 0"** per a transicions instantànies (idle/thrust)
- Per a asteroides, usa `Speed` a l'Animator State per controlar la velocitat de l'animació sense duplicar clips
- El component `Animator` s'afegeix i s'elimina dinàmicament — no cal afegir-lo manualment als prefabs
