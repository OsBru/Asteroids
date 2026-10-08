# 🖥️ Pàgina d'Inici (Main Menu)

Guia per personalitzar la pantalla d'inici: títol, subtítol, botó Play, fons i versió.

---

## On es configura

Tot el menú principal es controla des d'un únic ScriptableObject:

**Ruta:** `Assets/ScriptableObjects/Configs/MainMenuConfig.asset`

Selecciona'l al **Project panel** de Unity per veure tots els camps a l'Inspector.

---

## Generar el menú automàticament

Si el menú no està creat a l'escena, executa:

> **Tools → Asteroids → 6. Configurar Menú Principal**

Això genera automàticament tota la jerarquia UI i la connecta amb el `MainMenuConfig`.

---

## Camps configurables

### 📝 Títol del joc

| Camp | Descripció | Valor per defecte |
|------|------------|-------------------|
| `Title Text` | Text del títol | `ASTEROIDS` |
| `Title Color` | Color del text | Blanc |
| `Title Font Size` | Mida de la font (pts) | `96` |
| `Title Bottom Margin` | Separació entre títol i botó (px) | `40` |

---

### 💬 Subtítol / Eslògan

| Camp | Descripció | Valor per defecte |
|------|------------|-------------------|
| `Subtitle Text` | Text del subtítol (buit = ocult) | `SURVIVE THE VOID` |
| `Subtitle Color` | Color del text | Blau clar |
| `Subtitle Font Size` | Mida de la font (pts) | `28` |

---

### 🖼️ Fons

| Camp | Descripció |
|------|------------|
| `Background Sprite` | Imatge de fons (prioritat sobre el color) |
| `Background Color` | Color sòlid si no hi ha sprite |
| `Background Image Type` | `Simple` / `Sliced` / `Tiled` |

**Com assignar una imatge de fons:**
1. Importa la imatge a `Assets/Sprites/`
2. **Texture Type** = `Sprite (2D and UI)`
3. Arrossega l'sprite al camp `Background Sprite`

---

### ▶️ Botó Play

| Camp | Descripció | Valor per defecte |
|------|------------|-------------------|
| `Play Button Text` | Text del botó | `JUGAR` |
| `Button Normal Color` | Color en repòs | Blau clar |
| `Button Hover Color` | Color en passar el ratolí | Blau brillant |
| `Button Pressed Color` | Color en prémer | Blau fosc |
| `Button Text Color` | Color del text del botó | Quasi negre |
| `Button Font Size` | Mida de la font (pts) | `36` |
| `Button Width` | Amplada en píxels | `280` |
| `Button Height` | Alçada en píxels | `80` |
| `Button Corner Radius` | Radi de cantonades (0 = quadrat) | `12` |
| `Button Sprite` | Sprite personalitzat (opcional) | Buit |

**Per usar un sprite personalitzat per al botó:**
- Importa el sprite amb **Texture Type** = `Sprite (2D and UI)` i **Mesh Type** = `Full Rect`
- Assigna'l al camp `Button Sprite`
- Els colors seguiran aplicant-se com a tint

---

### 🔢 Versió / Crèdits

| Camp | Descripció | Valor per defecte |
|------|------------|-------------------|
| `Version Text` | Text de versió (buit = ocult) | `v1.0` |
| `Version Color` | Color del text | Gris |
| `Version Font Size` | Mida de la font (pts) | `18` |

---

## Previsualització en temps real

Tots els canvis al `MainMenuConfig` es veuen **immediatament a l'Editor** sense entrar en Play Mode, gràcies al mètode `OnValidate()` del `MainMenuManager`.

---

## Estructura de la jerarquia UI

```
Canvas
 └── MainMenu                     ← MainMenuManager + MainMenuConfig
      ├── Background (Image)       ← Fons configurable
      ├── Title (TextMeshPro)      ← Títol del joc
      ├── Subtitle (TextMeshPro)   ← Subtítol/eslògan
      ├── PlayButton (Button)      ← Botó de Play
      │    └── PlayLabel (TMP)     ← Text del botó
      └── Version (TextMeshPro)    ← Text de versió (cantonada inf. dreta)
```
