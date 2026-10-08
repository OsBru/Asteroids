# 🚀 Asteroids — Guia de Personalització Visual

> Documentació completa per modificar l'aparença del joc: sprites, animacions, fons, menú principal i sistema de skins.

---

## 📋 Taula de continguts

1. [Estructura del projecte](#estructura-del-projecte)
2. [Sistema de Skins](#sistema-de-skins)
3. [Canviar sprites](#canviar-sprites)
4. [Afegir animacions](#afegir-animacions)
5. [Imatge de fons del joc](#imatge-de-fons-del-joc)
6. [Pàgina d'inici (Main Menu)](#pàgina-dinici-main-menu)
7. [Explosió de la nau](#explosió-de-la-nau)
8. [Flux de treball recomanat](#flux-de-treball-recomanat)

---

## Estructura del projecte

```
Assets/
├── Scripts/
│   ├── SkinSystem/
│   │   ├── GameSkinData.cs          ← ScriptableObject principal de cada tema
│   │   ├── SkinManager.cs           ← Gestor global de skins (singleton)
│   │   ├── ShipSkinApplier.cs       ← Aplica la skin a la nau
│   │   └── AsteroidSkinApplier.cs   ← Aplica la skin als asteroides
│   ├── UI/
│   │   ├── MainMenuConfig.cs        ← ScriptableObject de la pantalla d'inici
│   │   └── MainMenuManager.cs       ← Lògica del menú principal
│   └── Core/
│       └── GameManager.cs           ← Control central del joc
├── ScriptableObjects/
│   ├── Skins/
│   │   ├── Skin_RetroNeon.asset     ← Tema 1 (Retro Neon)
│   │   └── Skin_DeepSpace.asset     ← Tema 2 (Deep Space)
│   └── Configs/
│       └── MainMenuConfig.asset     ← Configuració del menú principal
├── Sprites/                         ← Totes les imatges del joc
├── Audio/                           ← Sons i música
└── Prefabs/                         ← Prefabs de nau, asteroides, explosions
```

---

## Sistema de Skins

El joc utilitza un sistema de **skins intercanviables en temps real**. Cada skin és un asset de tipus `GameSkinData` que agrupa tots els elements visuals i sonors d'un tema.

### Crear una nova skin

1. Clic dret a `Assets/ScriptableObjects/Skins/`
2. **Create → Asteroids → Game Skin**
3. Posa-li un nom descriptiu (ex: `Skin_Medieval`)
4. Configura els camps a l'Inspector (vegeu les seccions següents)
5. Afegeix la nova skin al **SkinManager** → array `Available Skins`

### Canviar de skin durant el joc

Prem **`T`** per ciclar entre totes les skins registrades. El canvi és **instantani i en calent**.

---

## Canviar sprites

Consulta la guia detallada: [docs/canviar-sprites.md](docs/canviar-sprites.md)

### Resum ràpid

| Element | Camp a `GameSkinData` | Secció |
|---------|----------------------|--------|
| Nau | `Ship Sprite` + `Ship Color` | Player Ship |
| Propulsor | `Thruster Sprite` | Player Ship |
| Làser/Projectil | `Projectile Sprite` + `Projectile Color` | Projectiles |
| Asteroide gran | `Large Asteroid Sprites[]` | Asteroids |
| Asteroide mitjà | `Medium Asteroid Sprites[]` | Asteroids |
| Asteroide petit | `Small Asteroid Sprites[]` | Asteroids |
| Icona de vida (HUD) | `Life Icon Sprite` | UI & HUD |

> **Nota:** Els arrays d'asteroides accepten **múltiples sprites** — el joc en tria un a l'atzar per a cada asteroide generat.

---

## Afegir animacions

Consulta la guia detallada: [docs/animacions.md](docs/animacions.md)

### Resum ràpid

Les animacions s'assignen com a **AnimatorController** al `GameSkinData`. Tenen prioritat sobre els sprites estàtics — si el camp de l'Animator és buit, el sistema usa l'sprite.

| Element | Camp a `GameSkinData` | Secció |
|---------|----------------------|--------|
| Nau (animada) | `Ship Animator` | Animació · Nau |
| Paràmetre de propulsió | `Thrust Param Name` (default: `IsThrusting`) | Animació · Nau |
| Asteroide gran | `Large Asteroid Animator` | Animació · Asteroides |
| Asteroide mitjà | `Medium Asteroid Animator` | Animació · Asteroides |
| Asteroide petit | `Small Asteroid Animator` | Animació · Asteroides |

---

## Imatge de fons del joc

Consulta la guia detallada: [docs/fons-del-joc.md](docs/fons-del-joc.md)

### Resum ràpid

A `GameSkinData`, secció **Music & Atmosphere**:

| Camp | Descripció |
|------|------------|
| `Background Sprite` | Imatge de fons (prioritat sobre el color) |
| `Background Color` | Color sòlid si no hi ha sprite |

---

## Pàgina d'inici (Main Menu)

Consulta la guia detallada: [docs/menu-principal.md](docs/menu-principal.md)

### Resum ràpid

Tot es configura a `Assets/ScriptableObjects/Configs/MainMenuConfig.asset`:

| Secció | Camps configurables |
|--------|---------------------|
| Títol | Text, color, mida de font |
| Subtítol | Text, color, mida de font |
| Fons | Sprite o color sòlid |
| Botó Play | Text, colors (normal/hover/premut), mida, sprite |
| Versió | Text, color, mida de font |

---

## Explosió de la nau

Consulta la guia detallada: [docs/explosions.md](docs/explosions.md)

### Resum ràpid

A `GameSkinData`, secció **Player Ship**:

- **`Ship Explosion Prefab`** → Prefab amb Particle System o Animator
- **`Ship Explosion Sfx`** → So de l'explosió (AudioClip)

El prefab necessita el component `AutoDestroyVFX` per destruir-se sol en acabar.

---

## Flux de treball recomanat

```
1. Importa els nous sprites/animacions a Assets/Sprites/
2. Assegura't que Texture Type = "Sprite (2D and UI)"
3. Obre el GameSkinData del tema que vols modificar
4. Assigna els nous assets als camps corresponents
5. Prem Play a Unity per veure el resultat
6. Prem T durant el joc per canviar de tema i comparar
```

---

## Requisits tècnics

- **Unity** 2022.3 LTS o superior
- **Input System** package (actiu)
- **TextMeshPro** package
- **Universal Render Pipeline (URP) 2D**
