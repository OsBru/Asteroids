# Asteroids
Unity Learning Tool

Asteroids
Eina d'aprenentatge d'Unity. Ideal per a artistes i nous usuaris d'Unity. Més informació al nostre github.

Projecte de joc basat en l'Asteroids. Eina pensada per introduir les animacion en un entorn de joc Unity.

Joc Asteroids 2D amb Sistema de Reskin Modular
L'objectiu d'aquest projecte és crear un joc complet estil Asteroids 2D a Unity (Unity 6 / URP) dissenyat expressament per facilitar el reskin (canvi immediat de sprites, música, efectes de so, animacions d'explosió i paleta de colors).

Tot el disseny visual i sonor estarà desacoblat de la lògica del joc mitjançant ScriptableObjects (GameSkinData), permetent canviar de tema amb un sol clic o fins i tot en temps real durant la partida.

1. Arquitectura del Sistema de Reskin
Per crear un tema nou, només cal fer clic dret a Unity: Create -> Asteroids -> Game Skin, omplir els camps amb els nous assets (sprites, àudios, prefabs d'explosió) i afegir-lo a la llista de temes.

Inclourem 2 temes complets de mostra funcionals:
Retro Neon / Vector: Estil arcade clàssic, colors neó, asteroides poligonals i sons 8-bit.
Deep Space / Sci-Fi: Nau daurada, làsers de plasma, asteroides rocosos i explosions contundents.
2. Components del Joc
A. Sistema de Skins & Assets
GameSkinData.cs: ScriptableObject centralitzat que conté totes les referències visuals i d'àudio.
SkinManager.cs: Administra el tema actiu, permet canviar de tema (amb tecla T o menú d'opcions) i notifica tots els objectes en escena.
SkinApplier components (ShipSkinApplier, AsteroidSkinApplier, BackgroundSkinApplier): Apliquen automàticament els sprites, colors i efectes del tema actual.
Creador d'assets procedurals (per generar automàticament els sprites i sons inicials sense dependències externes).
B. Jugador & Controls
PlayerController.cs:
Moviment físic realista 2D (Rigidbody2D) amb inèrcia, rotació i acceleració (propulsió).
Efecte visual de propulsió (propulsor actiu en prémer endavant).
Suport d'entrada compatible amb el nou Input System de Unity i tecles clàssiques (W/A/D, Fletxes, Espai).
Vides, reaparició amb parpelleig d'invulnerabilitat temporal (3 segons).
PlayerShooting.cs:
Dispar de projectils des de la punta de la nau amb cadència de foc configurable.
Generació del projectil amb la velocitat de la nau + impuls cap endavant.
Projectile.cs:
Projectil 2D amb temps de vida limitat.
Detecció d'impacte amb asteroides.
C. Asteroides & Enemics
Asteroid.cs:
3 mides: Gran (20 punts), Mitjà (50 punts), Petit (100 punts).
Fragmentació:
Destruir un asteroide Gran en crea 2 de Mitjans amb angles divergents.
Destruir un asteroide Mitjà en crea 2 de Petits.
Destruir un asteroide Petit l'elimina definitivament.
Velocitat i rotació aleatòries.
Instanciació de l'efecte d'explosió corresponent al tema.
AsteroidSpawner.cs:
Generador d'asteroides als marges de la pantalla (fora del camp visual) dirigits cap a l'interior.
Sistema d'onades progressives (cada ronda augmenta la quantitat i velocitat).
D. Sistema de Pantalla Infinita (Screen Wrapping)
ScreenWrapper.cs:
Component reutilitzable per a la nau, bales i asteroides.
Quan un objecte surt per la vora esquerra, reapareix per la dreta (i dalt/baix) de manera suau utilitzant el viewport de la càmera.
E. Àudio, UI i Gestió de Partida
AudioManager.cs:
Canals dedicats per a música de fons (en bucle), so del motor (en bucle quan s'accelera), trets i explosions.
Ajusta els clips segons el GameSkinData actiu.
GameManager.cs:
Puntuació, rècord màxim (PlayerPrefs), comptador de vides i onades.
Flux de joc: Inici, Jugant, Mort temporal, Game Over i Reinici ràpid (tecla R).
UIManager.cs:
Panell HUD: Punts, Rècord, Vides restants (icones de naus), Onada actual.
Panell de Game Over amb botó de reinici.
Indicador de tema actual amb instrucció ("Tema: Retro Neon [Prem T per canviar de tema]").


Solució a possibles errors de configuració
A Unity, obre la teva escena (SampleScene)

A la Jerarquia (Hierarchy), selecciona el GameObject EventSystem

A l'Inspector, elimina el component Standalone Input Module (clic dret → Remove Component)

Fes clic a Add Component i afegeix Input System UI Input Module (de UnityEngine.InputSystem.UI)

Unity també pot mostrar-te automàticament un botó "Replace with InputSystemUIInputModule" quan seleccionis l'EventSystem — simplement fes-hi clic si apareix.

Com funciona
El sistema usa GameSkinData — un ScriptableObject que agrupa tots els sprites, colors i sons d'un tema visual.

Perfecte. Ja tens 2 skins existents i el sistema completament muntat. Aquí tens la guia completa:

Pas 1 — Crea un nou GameSkinData
Clic dret a la carpeta Assets/ScriptableObjects/Skins → Create → Asteroids → Game Skin
Posa-li un nom descriptiu, p.ex. Skin_Medieval o Skin_CyberPunk.

Pas 2 — Omple els camps a l'Inspector
Selecciona el nou asset i configura cada secció:

Secció	Camps	Descripció
General Info	skinName, themeAccentColor	Nom del tema i color dominant
Player Ship	shipSprite, shipColor	Sprite i tint de la nau
, >code class="whitespace-pre-wrap">thrusterParticlePrefab	Visual del propulsor
shootSfx, , >code class="whitespace-pre-wrap">shipExplosionSfx	Sons
Projectiles	projectileSprite, projectileColor, projectileScale	Aspecte dels làsers
Asteroids	largeAsteroidSprites[], mediumAsteroidSprites[], smallAsteroidSprites[]	Pots posar-ne varis — s'escullen a l'atzar
asteroidTint, asteroidExplosionPrefab	Color i efecte d'explosió
Music & Atmosphere	backgroundSprite, backgroundColor	Imatge/color de fons
backgroundMusic, gameOverSfx	Música i so de final
UI & HUD	lifeIconSprite, uiTextColor	Icona de vides i color dels textos
TIP

Per als asteroides pots afegir múltiples sprites a l'array. Cada asteroide triarà un aleatòriament, cosa que dóna varietat visual sense cap esforç addicional.

Pas 3 — Afegeix la skin al SkinManager
A la Hierarchy, selecciona el GameObject GameManagers
Al component SkinManager, localitza l'array Available Skins
Augmenta la mida i arrossega el nou asset al nou slot
Pas 4 — Canviar de skin en joc
Durant la partida, prem T per ciclar entre totes les skins registrades. El canvi és instantani i en calent — tots els elements visuals i sonors s'actualitzen al moment.

On posar els nous sprites
Importa les teves imatges a Assets/Sprites/ i assegura't que el Texture Type sigui Sprite (2D and UI) a l'Inspector de la textura.

NOTA

Si vols un reskin total i no vols mantenir les skins anteriors, simplement edita directament Skin_RetroNeon.asset o Skin_DeepSpace.asset substituint els sprites pels teus.

Com crear les animacions a Unity
Per a la nau
Selecciona el prefab Player → Window → Animation → Animation
Crea dos clips: Ship_Idle i Ship_Thrusting (amb els sprites corresponents)
Crea un Animator Controller (p.ex. ShipAnimator)
Al Animator, afegeix un paràmetre Bool anomenat exactament IsThrusting
Fes la transició: Ship_Idle → Ship_Thrusting quan IsThrusting = true, i viceversa
Per als asteroides
Crea clips d'animació per a cada mida (p.ex. Asteroid_Large_Spin)
Crea un Animator Controller per mida (o un de compartit si l'animació és la mateixa)
No cal cap paràmetre — pot ser un loop directe
Assignar a la Skin
Obre el teu GameSkinData i arrossega els controllers als nous camps:

▼ Animació · Nau
    Ship Animator      ← [ShipAnimator]
    Thrust Param Name  ← "IsThrusting"
▼ Animació · Asteroides  
    Large Asteroid Animator  ← [AsteroidLarge]
    Medium Asteroid Animator ← [AsteroidMedium]
    Small Asteroid Animator  ← [AsteroidSmall]
TIP

Barreja lliure: Pots tenir animació a la nau i sprites estàtics als asteroides, o qualsevol combinació. Si el camp de l'Animator és buit, el sistema cau automàticament a l'sprite estàtic, sense trencar res.

NOTA

Canvi de skin en calent: Si canvies de skin prement T durant la partida, el nou AnimatorController es carrega immediatament. Els asteroides ja generats s'actualitzen al moment gràcies a OnSkinChanged.

1. 💥 Explosió de la nau
L'explosió no és un sprite ni un Animator directe — és un Prefab amb un efecte de partícules o una animació que s'instancia en el moment de morir.

Com crear-lo
A la Hierarchy, crea un GameObject buit → afegeix-hi un component Particle System (o un Animator amb el clip d'explosió)
Configura l'efecte visual com vulguis
Afegeix el component AutoDestroyVFX perquè el prefab es destrueixi sol en acabar
Arrossega el GameObject a Assets/Prefabs/ per convertir-lo en Prefab
Obre el teu GameSkinData i arrossega el prefab al camp:
▼ Player Ship
    Ship Explosion Prefab  ← [el teu prefab d'explosió]
    Ship Explosion Sfx     ← [so opcional]
TIP

Si vols una animació de sprites (spritesheet) en lloc de partícules: crea un GameObject amb SpriteRenderer + Animator, fes el clip d'explosió frame a frame, afegeix AutoDestroyVFX i guarda'l com a Prefab.

2. 🌌 Imatge de fons del joc
El fons del joc s'aplica automàticament via el component BackgroundSkinApplier que hi ha a la càmera principal.

Com assignar-la
Obre el teu GameSkinData i omple la secció Music & Atmosphere:

▼ Music & Atmosphere
    Background Color   ← color sòlid de fons (si no hi ha sprite)
    Background Sprite  ← [la teva imatge de fons .png]
Importa la imatge a Assets/Sprites/ i assegura't que el Texture Type sigui Sprite (2D and UI).

NOTE

Si assignes un Background Sprite, es mostrarà la imatge. Si el deixes buit, s'usa el Background Color com a fons sòlid. El canvi és en calent — si premses T per canviar de skin, el fons canvia immediatament.

3. 🖼️ Imatge de fons de la pantalla d'inici
Aquesta va al MainMenuConfig (l'asset que vam crear a Assets/ScriptableObjects/Configs/):

▼ Fons
    Background Sprite  ← [la teva imatge de fons del menú]
    Background Color   ← color sòlid (si Sprite és buit)
    Background Image Type ← Simple / Sliced / Tiled
Passos
Importa la imatge a Assets/Sprites/
A l'Inspector de la textura, posa Texture Type = Sprite (2D and UI)
Selecciona Assets/ScriptableObjects/Configs/MainMenuConfig
Arrossega el sprite al camp Background Sprite
