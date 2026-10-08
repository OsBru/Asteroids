# 💥 Explosió de la Nau

Guia per personalitzar l'efecte visual i sonor de l'explosió quan la nau del jugador mor.

---

## Com funciona

Quan la nau mor, el `ShipSkinApplier` instancia el **Prefab d'explosió** definit al `GameSkinData`. El prefab s'instancia a la posició de la nau i es destrueix sol en acabar, gràcies al component `AutoDestroyVFX`.

---

## Opció A — Explosió amb Particle System (recomanada)

### Pas 1 — Crear el prefab

1. A la Hierarchy, **crea un GameObject buit** → anomena'l `ShipExplosion`
2. Afegeix-hi el component **Particle System**
3. Configura les partícules:
   - **Duration**: 0.5–1.0 s
   - **Loop**: ❌ Desactivat
   - **Start Speed**: 2–8
   - **Start Lifetime**: 0.3–0.8
   - **Emission → Burst**: Ex: 20–50 partícules al moment 0
   - **Renderer → Sprite**: Pots usar un sprite de l'explosió
4. Afegeix el component **`AutoDestroyVFX`** (necessari per destruir-se sol)
5. Arrossega el GameObject a `Assets/Prefabs/` per convertir-lo en Prefab
6. Esborra el GameObject de la Hierarchy

### Pas 2 — Assignar al GameSkinData

1. Obre el `GameSkinData` del teu tema
2. Secció **Player Ship**:
   - `Ship Explosion Prefab` → arrossega el prefab `ShipExplosion`
   - `Ship Explosion Sfx` → arrossega el clip d'àudio de l'explosió (opcional)

---

## Opció B — Explosió amb Spritesheet animat

### Pas 1 — Importar el spritesheet

1. Copia el spritesheet a `Assets/Sprites/`
2. A l'Inspector:
   - **Texture Type** = `Sprite (2D and UI)`
   - **Sprite Mode** = `Multiple`
3. **Sprite Editor → Slice** per tallar els fotogrames

### Pas 2 — Crear el clip d'animació

1. Crea un GameObject buit amb `SpriteRenderer` + `Animator`
2. **Window → Animation → Animation → Create** → guarda com `Explosion_Ship.anim`
3. Arrossega tots els fotogrames al timeline (ordre correcte)
4. A l'Inspector del clip: **Loop Time = ❌ false**

### Pas 3 — Configurar l'Animator Controller

1. **Create → Animator Controller** → `ShipExplosionAnimator`
2. Afegeix l'estat `Explosion_Ship` com a estat per defecte
3. Assigna el controller al component `Animator` del GameObject

### Pas 4 — Afegir AutoDestroyVFX

1. Selecciona el GameObject de l'explosió
2. **Add Component → AutoDestroyVFX**
3. Configura el temps de destrucció perquè coincideixi amb la durada de l'animació

### Pas 5 — Guardar com a Prefab i assignar

Igual que l'Opció A (Pas 5 i 6).

---

## Component AutoDestroyVFX

Aquest component ja existeix al projecte (`Assets/Scripts/Utils/AutoDestroyVFX.cs`). Destrueix el GameObject automàticament en acabar l'efecte.

```
GameObject ShipExplosion
 ├── Particle System  (o Animator + SpriteRenderer)
 └── AutoDestroyVFX   ← imprescindible!
```

---

## Explosió dels asteroides

El mateix sistema s'aplica als asteroides. Al `GameSkinData`, secció **Asteroids**:

| Camp | Descripció |
|------|------------|
| `Asteroid Explosion Prefab` | Prefab d'explosió comú per a totes les mides |
| `Asteroid Explosion Sfx` | So en destruir un asteroide |
| `Asteroid Hit Sfx` | So en impactar (sense destruir) |

---

## Consells

- Les explosions **no necessiten col·liders** — es creen com a efecte purament visual
- Pots crear **efectes d'explosió diferents** per a cada skin
- Per a millor rendiment, evita Particle Systems amb més de 100 partícules simultànies
- El color de les partícules pot llegir el `Asteroid Tint` o `Ship Color` per mantenir coherència visual (requereix codi addicional)
