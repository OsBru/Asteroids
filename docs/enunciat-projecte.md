# 🎮 Projecte: Reskin d'Asteroids

**Mòdul:** Disseny Gràfic per a Videojocs
**Durada estimada:** 3–4 setmanes
**Eines:** Adobe Illustrator / Photoshop · Unity 2D

---

## Descripció del projecte

A partir del joc clàssic **Asteroids**, dissenyaràs i implementaràs un **reskin visual complet** amb una estètica pròpia i coherent. El procés seguirà el flux de treball professional del disseny gràfic per a videojocs: des de la recerca visual fins al muntatge final en el motor de joc.

El joc ja és funcional. La teva tasca és redefinir-ne completament la identitat visual.

---

## Objectius

- Aplicar un procés de disseny professional: investigació → conceptualització → producció → implementació
- Crear assets gràfics originals (sprites i/o animacions) coherents amb un tema definit
- Integrar els assets dins d'un motor de joc (Unity) de manera autònoma
- Presentar el treball amb una argumentació visual i conceptual clara

---

## Fases del projecte

### Fase 1 — Recerca i Moodboard
**Durada:** 2–3 dies

Defineix el **tema visual** del teu reskin. Pot ser qualsevol univers estètic: medieval, underwater, cyberpunk, kawaii, western, horror, art déco...

**Lliurables:**
- **Moodboard** (mínim 15–20 referències visuals): paleta de colors, tipografies, textures, referents estètics
- **Definició del concepte** (5–10 línies): quin univers és, quina sensació ha de transmetre, com es traduirà al joc

**Format:** Document A3 o presentació digital (Figma, PDF, etc.)

---

### Fase 2 — Esbossos i definició d'assets
**Durada:** 3–4 dies

Identifica tots els elements visuals que cal dissenyar i fes esbossos a mà o digital.

**Elements obligatoris:**
| Element | Descripció |
|---------|------------|
| **Nau del jugador** | Forma, silueta, detalls |
| **Asteroide gran** | Mínimo 1 variant |
| **Asteroide mitjà** | Mínimo 1 variant |
| **Asteroide petit** | Mínimo 1 variant |
| **Làser / Projectil** | Forma i color |
| **Fons del joc** | Composició i color |
| **Pantalla d'inici** | Títol, botó Play, fons |

**Elements opcionals (+nota):**
- Animació de la nau (idle / propulsant)
- Animació dels asteroides (rotació, pulsació)
- Explosions personalitzades (partícules o spritesheet)
- Icona de vides al HUD

**Lliurables:**
- Full d'esbossos amb anotacions (proporcions, colors, comportament)
- Paleta de colors definitiva (HEX o RGB)

---

### Fase 3 — Producció de assets
**Durada:** 1–1,5 setmanes

Digitalitza i finalitza tots els assets en **alta qualitat**, amb fons transparent (PNG).

**Especificacions tècniques:**

| Asset | Mida recomanada | Format |
|-------|----------------|--------|
| Nau | 128×128 px | PNG |
| Asteroide gran | 128×128 px | PNG |
| Asteroide mitjà | 64×64 px | PNG |
| Asteroide petit | 32×32 px | PNG |
| Làser | 8×32 px | PNG |
| Icona de vida | 32×32 px | PNG |
| Fons del joc | 1920×1080 px | PNG/JPG |
| Fons menú principal | 1920×1080 px | PNG/JPG |

> Si fas animacions: exporta com a spritesheet o com a fotogrames individuals numerats.

**Criteris de qualitat:**
- Coherència estètica entre tots els elements
- Llegibilitat en pantalla (contrast, siluetes clares)
- Fons transparent en tots els sprites de joc

---

### Fase 4 — Implementació a Unity
**Durada:** 3–4 dies

Importa i configura tots els assets dins del projecte Unity proporcionat.

**Passos:**
1. Copia els sprites a `Assets/Sprites/NomDelTeuTema/`
2. Configura cada textura: **Texture Type → Sprite (2D and UI)**
3. Crea un nou `GameSkinData`: **Create → Asteroids → Game Skin**
4. Assigna tots els sprites als camps corresponents
5. Si tens animacions: crea els clips i Animator Controllers
6. Configura el `MainMenuConfig` amb el fons i l'estètica del menú
7. Afegeix la nova skin al `SkinManager`
8. Prova el joc i ajusta fins que tot sigui coherent

---

### Fase 5 — Presentació final
**Durada:** 1 dia

Presenta el teu reskin al grup explicant les decisions de disseny preses a cada fase.

**Contingut de la presentació (10–15 min):**
1. Moodboard i concepte inicial
2. Esbossos i evolució del disseny
3. Assets finals i argumentació de les decisions
4. Demo en viu del joc amb la nova skin
5. Autocrítica: què canviaries, què has après

**Format:** Presentació digital (PowerPoint, Figma, PDF)

---

## Criteris d'avaluació

| Criteri | Pes |
|---------|-----|
| Coherència estètica del conjunt | 25% |
| Qualitat tècnica dels assets (resolució, transparències, llegibilitat) | 20% |
| Procés de disseny documentat (moodboard, esbossos) | 20% |
| Implementació correcta a Unity | 20% |
| Presentació i argumentació de les decisions | 15% |

---

## Recursos proporcionats

- Projecte Unity funcional amb el joc Asteroids complet
- Documentació tècnica del sistema de skins: `docs/canviar-sprites.md`, `docs/animacions.md`, `docs/menu-principal.md`, `docs/explosions.md`
- Guia d'importació de textures a Unity

---

## Condicions

- Els assets han de ser **originals** (il·lustració pròpia o generació amb IA declarada i retocada)
- No es permet usar sprites existents d'altres jocs sense llicència
- Tots els fitxers finals han d'estar correctament nomenats i organitzats
- L'entrega inclou: arxius d'il·lustració originals + assets exportats + captura de pantalla del joc amb el reskin

---

*Qualsevol dubte tècnic sobre la implementació a Unity, consulta la documentació del projecte o pregunta al professor.*
