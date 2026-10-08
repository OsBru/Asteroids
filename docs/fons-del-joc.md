# 🌌 Imatge de Fons del Joc

Guia per personalitzar el fons de l'escena de joc.

---

## Com funciona

El fons del joc s'aplica automàticament pel component `BackgroundSkinApplier`, que es troba al **GameObject de la càmera principal**. Quan canvia la skin activa, el fons s'actualitza automàticament.

Hi ha dues opcions:
- **Color sòlid** → s'aplica com a color de fons de la càmera
- **Sprite/Imatge** → es renderitza com a capa de fons darrere de tot

---

## Opció A — Color sòlid

1. Selecciona el `GameSkinData` del teu tema
2. Secció **Music & Atmosphere**
3. Configura el camp **`Background Color`** amb el color desitjat
4. Deixa **`Background Sprite`** buit

El color s'aplica directament a `Camera.backgroundColor`.

---

## Opció B — Imatge de fons

### Pas 1 — Importar la imatge

1. Copia la imatge a `Assets/Sprites/`
2. A l'Inspector de la textura:
   - **Texture Type** = `Sprite (2D and UI)`
   - **Max Size** = `2048` o `4096` (per a imatges d'alta resolució)
3. Fes clic a **Apply**

**Mides recomanades:** 1920×1080 px (Full HD) o 2560×1440 px (2K)

### Pas 2 — Assignar al GameSkinData

1. Selecciona el `GameSkinData` del teu tema
2. Secció **Music & Atmosphere**
3. Arrossega la imatge al camp **`Background Sprite`**

Quan hi ha un `Background Sprite`, té **prioritat sobre el `Background Color`**.

---

## Consells

- Usa imatges amb **fons fosc** per contrastar bé amb els asteroides i la nau
- Per a un efecte de **paral·laxi** (fons amb profunditat), caldria afegir un script addicional
- Pots usar la mateixa imatge per al fons del joc i per al menú principal, o una diferent per a cada skin
- Si el joc té diverses skins, cada una pot tenir un fons completament diferent
