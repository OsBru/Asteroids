using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Asteroids.Core;
using Asteroids.Player;
using Asteroids.Asteroids;
using Asteroids.SkinSystem;
using Asteroids.Audio;
using Asteroids.UI;
using Asteroids.Utils;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Asteroids.EditorTools
{
    public static class AsteroidsAssetSetup
    {
        private const string SPRITES_PATH = "Assets/Sprites";
        private const string AUDIO_PATH = "Assets/Audio";
        private const string PREFABS_PATH = "Assets/Prefabs";
        private const string SKINS_PATH = "Assets/ScriptableObjects/Skins";

        [InitializeOnLoadMethod]
        private static void OnEditorLoaded()
        {
            EditorApplication.delayCall += () =>
            {
                if (!File.Exists($"{SPRITES_PATH}/neon_ship.png") || !File.Exists($"{SKINS_PATH}/Skin_RetroNeon.asset"))
                {
                    Debug.Log("<color=yellow>[Asteroids] Detectada configuració inicial pendent. Generant tots els recursos...</color>");
                    RunFullSetup(false);
                }
            };
        }

        [MenuItem("Tools/Asteroids/🌟 Configuració Completa (Executar Tot)", false, 0)]
        public static void RunFullSetupManual()
        {
            RunFullSetup(true);
        }

        public static void RunFullSetup(bool showDialog = false)
        {
            EnsureDirectories();
            GenerateAllTextures();
            GenerateAllAudio();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            ConfigureTextureImporters();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            CreateVFXPrefabs();
            CreateGamePrefabs();
            CreateSkinDataAssets();
            SetupSampleScene();
            SetupMainMenu();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (showDialog)
            {
                EditorUtility.DisplayDialog("Asteroids Setup", "S'ha completat la generació de recursos, prefabs, skins i configuració de l'escena amb èxit!", "Genial!");
            }
            Debug.Log("<color=cyan><b>[Asteroids]</b> Tot el projecte s'ha configurat correctament!</color>");
        }

        private static void EnsureDirectories()
        {
            if (!Directory.Exists(SPRITES_PATH)) Directory.CreateDirectory(SPRITES_PATH);
            if (!Directory.Exists(AUDIO_PATH)) Directory.CreateDirectory(AUDIO_PATH);
            if (!Directory.Exists(PREFABS_PATH)) Directory.CreateDirectory(PREFABS_PATH);
            if (!Directory.Exists(SKINS_PATH)) Directory.CreateDirectory(SKINS_PATH);
            AssetDatabase.Refresh();
        }

        #region 1. TEXTURE & SPRITE GENERATION

        [MenuItem("Tools/Asteroids/1. Generar Textures i Sprites", false, 10)]
        public static void GenerateAllTextures()
        {
            EnsureDirectories();

            // TEMA 1: RETRO NEON
            SavePNG(CreateVectorShip(new Color(0.1f, 1f, 1f, 1f), new Color(0f, 0.4f, 0.4f, 0.6f)), $"{SPRITES_PATH}/neon_ship.png");
            SavePNG(CreateThrusterFlame(new Color(1f, 0.2f, 0.8f, 1f), new Color(1f, 0.8f, 0.1f, 1f)), $"{SPRITES_PATH}/neon_thruster.png");
            SavePNG(CreateLaserBeam(new Color(1f, 0.2f, 0.4f, 1f), new Color(1f, 0.8f, 0.9f, 1f)), $"{SPRITES_PATH}/neon_laser.png");
            SavePNG(CreatePolygonAsteroid(96, new Color(0.2f, 0.9f, 1f, 1f), new Color(0.05f, 0.15f, 0.25f, 0.8f), 1337), $"{SPRITES_PATH}/neon_asteroid_large.png");
            SavePNG(CreatePolygonAsteroid(56, new Color(0.2f, 0.9f, 1f, 1f), new Color(0.05f, 0.15f, 0.25f, 0.8f), 2448), $"{SPRITES_PATH}/neon_asteroid_med.png");
            SavePNG(CreatePolygonAsteroid(32, new Color(0.2f, 0.9f, 1f, 1f), new Color(0.05f, 0.15f, 0.25f, 0.8f), 3559), $"{SPRITES_PATH}/neon_asteroid_small.png");
            SavePNG(CreateMiniShipIcon(new Color(0.1f, 1f, 1f, 1f)), $"{SPRITES_PATH}/neon_life.png");

            // TEMA 2: DEEP SPACE (Fighter metàl·lic i asteroides rocosos)
            SavePNG(CreateSciFiShip(new Color(1f, 0.75f, 0.2f, 1f), new Color(0.85f, 0.25f, 0.15f, 1f)), $"{SPRITES_PATH}/space_ship.png");
            SavePNG(CreateThrusterFlame(new Color(0.2f, 0.7f, 1f, 1f), Color.white), $"{SPRITES_PATH}/space_thruster.png");
            SavePNG(CreateLaserBeam(new Color(0.2f, 1f, 0.4f, 1f), Color.white), $"{SPRITES_PATH}/space_laser.png");
            SavePNG(CreateRockyAsteroid(96, new Color(0.6f, 0.55f, 0.5f, 1f), new Color(0.35f, 0.3f, 0.28f, 1f), 411), $"{SPRITES_PATH}/space_asteroid_large.png");
            SavePNG(CreateRockyAsteroid(56, new Color(0.6f, 0.55f, 0.5f, 1f), new Color(0.35f, 0.3f, 0.28f, 1f), 522), $"{SPRITES_PATH}/space_asteroid_med.png");
            SavePNG(CreateRockyAsteroid(32, new Color(0.6f, 0.55f, 0.5f, 1f), new Color(0.35f, 0.3f, 0.28f, 1f), 633), $"{SPRITES_PATH}/space_asteroid_small.png");
            SavePNG(CreateMiniShipIcon(new Color(1f, 0.75f, 0.2f, 1f)), $"{SPRITES_PATH}/space_life.png");

            Debug.Log("[Asteroids] Sprites generats a Assets/Sprites.");
        }

        private static void ConfigureTextureImporters()
        {
            string[] files = Directory.GetFiles(SPRITES_PATH, "*.png");
            foreach (string file in files)
            {
                string assetPath = file.Replace("\\", "/");
                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 64;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.mipmapEnabled = false;
                    importer.SaveAndReimport();
                }
            }
        }

        private static void SavePNG(Texture2D tex, string path)
        {
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(tex);
        }

        private static Texture2D CreateVectorShip(Color outline, Color fill)
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            ClearTexture(tex);

            Vector2 top = new Vector2(32, 56);
            Vector2 left = new Vector2(12, 10);
            Vector2 notch = new Vector2(32, 20);
            Vector2 right = new Vector2(52, 10);

            // Omplir triangles
            DrawTriangle(tex, top, left, notch, fill);
            DrawTriangle(tex, top, notch, right, fill);

            // Contorns gruixuts
            DrawThickLine(tex, top, left, outline, 2);
            DrawThickLine(tex, left, notch, outline, 2);
            DrawThickLine(tex, notch, right, outline, 2);
            DrawThickLine(tex, right, top, outline, 2);

            tex.Apply();
            return tex;
        }

        private static Texture2D CreateSciFiShip(Color hull, Color wings)
        {
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            ClearTexture(tex);

            Vector2 nose = new Vector2(32, 58);
            Vector2 leftFin = new Vector2(8, 12);
            Vector2 leftBase = new Vector2(26, 16);
            Vector2 rightBase = new Vector2(38, 16);
            Vector2 rightFin = new Vector2(56, 12);
            Vector2 bottomNotch = new Vector2(32, 12);

            // Ales
            DrawTriangle(tex, nose, leftFin, leftBase, wings);
            DrawTriangle(tex, nose, rightBase, rightFin, wings);
            // Fuselatge central
            DrawTriangle(tex, nose, leftBase, bottomNotch, hull);
            DrawTriangle(tex, nose, bottomNotch, rightBase, hull);

            // Cabina
            Vector2 cTop = new Vector2(32, 44);
            Vector2 cLeft = new Vector2(29, 32);
            Vector2 cRight = new Vector2(35, 32);
            DrawTriangle(tex, cTop, cLeft, cRight, new Color(0.2f, 0.9f, 1f, 1f));

            // Perímetre
            DrawThickLine(tex, nose, leftFin, Color.white, 1);
            DrawThickLine(tex, leftFin, leftBase, Color.white, 1);
            DrawThickLine(tex, leftBase, bottomNotch, Color.white, 1);
            DrawThickLine(tex, bottomNotch, rightBase, Color.white, 1);
            DrawThickLine(tex, rightBase, rightFin, Color.white, 1);
            DrawThickLine(tex, rightFin, nose, Color.white, 1);

            tex.Apply();
            return tex;
        }

        private static Texture2D CreateThrusterFlame(Color outer, Color inner)
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            ClearTexture(tex);

            Vector2 top = new Vector2(16, 28);
            Vector2 left = new Vector2(8, 22);
            Vector2 right = new Vector2(24, 22);
            Vector2 tip = new Vector2(16, 4);

            DrawTriangle(tex, top, left, tip, outer);
            DrawTriangle(tex, top, tip, right, outer);

            Vector2 inTip = new Vector2(16, 12);
            DrawTriangle(tex, new Vector2(16, 26), new Vector2(12, 22), inTip, inner);
            DrawTriangle(tex, new Vector2(16, 26), inTip, new Vector2(20, 22), inner);

            tex.Apply();
            return tex;
        }

        private static Texture2D CreateLaserBeam(Color edge, Color core)
        {
            int w = 16, h = 32;
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            ClearTexture(tex);

            for (int y = 4; y < h - 4; y++)
            {
                for (int x = 4; x < w - 4; x++)
                {
                    bool isCore = (x >= 6 && x <= 9);
                    tex.SetPixel(x, y, isCore ? core : edge);
                }
            }
            tex.Apply();
            return tex;
        }

        private static Texture2D CreatePolygonAsteroid(int size, Color outline, Color fill, int seed)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            ClearTexture(tex);

            Random.InitState(seed);
            int verts = 10;
            Vector2[] points = new Vector2[verts];
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float baseRadius = size * 0.42f;

            for (int i = 0; i < verts; i++)
            {
                float angle = i * (Mathf.PI * 2f / verts);
                float r = baseRadius * Random.Range(0.72f, 1.15f);
                points[i] = center + new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
            }

            for (int i = 0; i < verts; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % verts];
                DrawTriangle(tex, center, p1, p2, fill);
            }

            for (int i = 0; i < verts; i++)
            {
                Vector2 p1 = points[i];
                Vector2 p2 = points[(i + 1) % verts];
                DrawThickLine(tex, p1, p2, outline, 2);
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D CreateRockyAsteroid(int size, Color rock, Color shadow, int seed)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            ClearTexture(tex);

            Random.InitState(seed);
            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size * 0.44f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float d = Vector2.Distance(pos, center);
                    // Deformació irregular
                    float angle = Mathf.Atan2(pos.y - center.y, pos.x - center.x);
                    float noise = Mathf.Sin(angle * 5f) * (radius * 0.12f) + Mathf.Cos(angle * 3f) * (radius * 0.08f);

                    if (d <= radius + noise)
                    {
                        // Ombrejat direccional (llum des de dalt-esquerra)
                        float lightFactor = Vector2.Dot((pos - center).normalized, new Vector2(-0.6f, 0.8f));
                        Color c = Color.Lerp(shadow, rock, (lightFactor + 1f) * 0.5f);

                        // Cràters aleatoris
                        float craterNoise = Mathf.PerlinNoise(x * 0.15f, y * 0.15f);
                        if (craterNoise > 0.65f)
                        {
                            c *= 0.75f;
                        }

                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        private static Texture2D CreateMiniShipIcon(Color c)
        {
            int size = 32;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            ClearTexture(tex);
            DrawTriangle(tex, new Vector2(16, 28), new Vector2(6, 6), new Vector2(16, 12), c);
            DrawTriangle(tex, new Vector2(16, 28), new Vector2(16, 12), new Vector2(26, 6), c);
            tex.Apply();
            return tex;
        }

        private static void ClearTexture(Texture2D tex)
        {
            Color[] colors = new Color[tex.width * tex.height];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;
            tex.SetPixels(colors);
        }

        private static void DrawThickLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col, int thickness)
        {
            int steps = Mathf.CeilToInt(Vector2.Distance(p1, p2) * 2f);
            for (int i = 0; i <= steps; i++)
            {
                float t = steps == 0 ? 0 : (float)i / steps;
                Vector2 p = Vector2.Lerp(p1, p2, t);
                int px = Mathf.RoundToInt(p.x);
                int py = Mathf.RoundToInt(p.y);

                for (int tx = -thickness / 2; tx <= thickness / 2; tx++)
                {
                    for (int ty = -thickness / 2; ty <= thickness / 2; ty++)
                    {
                        int cx = px + tx;
                        int cy = py + ty;
                        if (cx >= 0 && cx < tex.width && cy >= 0 && cy < tex.height)
                        {
                            tex.SetPixel(cx, cy, col);
                        }
                    }
                }
            }
        }

        private static void DrawTriangle(Texture2D tex, Vector2 v1, Vector2 v2, Vector2 v3, Color col)
        {
            int minX = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(v1.x, Mathf.Min(v2.x, v3.x))), 0, tex.width - 1);
            int maxX = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(v1.x, Mathf.Max(v2.x, v3.x))), 0, tex.width - 1);
            int minY = Mathf.Clamp(Mathf.FloorToInt(Mathf.Min(v1.y, Mathf.Min(v2.y, v3.y))), 0, tex.height - 1);
            int maxY = Mathf.Clamp(Mathf.CeilToInt(Mathf.Max(v1.y, Mathf.Max(v2.y, v3.y))), 0, tex.height - 1);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                    if (PointInTriangle(p, v1, v2, v3))
                    {
                        tex.SetPixel(x, y, col);
                    }
                }
            }
        }

        private static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1 = Sign(pt, v1, v2);
            float d2 = Sign(pt, v2, v3);
            float d3 = Sign(pt, v3, v1);
            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
            return !(hasNeg && hasPos);
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        #endregion

        #region 2. AUDIO GENERATION (SYNTHESIZED WAVS)

        [MenuItem("Tools/Asteroids/2. Generar Efectes de So i Música WAV", false, 11)]
        public static void GenerateAllAudio()
        {
            EnsureDirectories();

            // Retro Neon Sounds
            SaveWav($"{AUDIO_PATH}/sfx_laser_neon.wav", SynthTone(0.12f, 950f, 180f, 0.7f, ToneType.Sine));
            SaveWav($"{AUDIO_PATH}/sfx_thrust_neon.wav", SynthNoiseRumble(0.8f, 65f, 0.45f));
            SaveWav($"{AUDIO_PATH}/sfx_explosion_ship_neon.wav", SynthExplosion(0.75f, 0.9f, true));
            SaveWav($"{AUDIO_PATH}/sfx_explosion_asteroid_neon.wav", SynthExplosion(0.35f, 0.7f, false));
            SaveWav($"{AUDIO_PATH}/bgm_neon_loop.wav", SynthBassPulseLoop(3.84f, 125f));

            // Deep Space Sounds
            SaveWav($"{AUDIO_PATH}/sfx_laser_space.wav", SynthTone(0.16f, 1200f, 120f, 0.8f, ToneType.Saw));
            SaveWav($"{AUDIO_PATH}/sfx_thrust_space.wav", SynthNoiseRumble(0.8f, 48f, 0.55f));
            SaveWav($"{AUDIO_PATH}/sfx_explosion_ship_space.wav", SynthExplosion(1.1f, 1.0f, true));
            SaveWav($"{AUDIO_PATH}/sfx_explosion_asteroid_space.wav", SynthExplosion(0.45f, 0.85f, false));
            SaveWav($"{AUDIO_PATH}/bgm_space_loop.wav", SynthSpaceDroneLoop(6.0f));

            // Shared GameOver
            SaveWav($"{AUDIO_PATH}/sfx_gameover.wav", SynthArpeggioDown(1.2f));

            Debug.Log("[Asteroids] Àudios WAV sintetitzats a Assets/Audio.");
        }

        private enum ToneType { Sine, Saw, Square }

        private static float[] SynthTone(float duration, float startFreq, float endFreq, float volume, ToneType type)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(duration * sampleRate);
            float[] data = new float[samples];
            float phase = 0f;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float freq = Mathf.Lerp(startFreq, endFreq, t);
                phase += 2f * Mathf.PI * freq / sampleRate;

                float rawSample = 0f;
                switch (type)
                {
                    case ToneType.Sine:
                        rawSample = Mathf.Sin(phase);
                        break;
                    case ToneType.Saw:
                        rawSample = (float)(2.0 * (phase / (2f * Mathf.PI) - Mathf.Floor(phase / (2f * Mathf.PI) + 0.5f)));
                        break;
                    case ToneType.Square:
                        rawSample = Mathf.Sin(phase) >= 0 ? 1f : -1f;
                        break;
                }

                // Envolupant d'amplitud ràpida (AD)
                float env = 1f - t;
                data[i] = rawSample * env * volume;
            }
            return data;
        }

        private static float[] SynthExplosion(float duration, float volume, bool heavyBass)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(duration * sampleRate);
            float[] data = new float[samples];
            System.Random rnd = new System.Random(42);
            float bassPhase = 0f;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / samples;
                float noise = (float)(rnd.NextDouble() * 2.0 - 1.0);
                float decay = Mathf.Exp(-t * (heavyBass ? 4.5f : 8f));

                float bass = 0f;
                if (heavyBass)
                {
                    float bassFreq = Mathf.Lerp(120f, 30f, t);
                    bassPhase += 2f * Mathf.PI * bassFreq / sampleRate;
                    bass = Mathf.Sin(bassPhase) * 0.8f;
                }

                data[i] = (noise * 0.6f + bass * 0.4f) * decay * volume;
            }
            return data;
        }

        private static float[] SynthNoiseRumble(float duration, float baseFreq, float volume)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(duration * sampleRate);
            float[] data = new float[samples];
            float phase = 0f;
            System.Random rnd = new System.Random(77);

            for (int i = 0; i < samples; i++)
            {
                phase += 2f * Mathf.PI * baseFreq / sampleRate;
                float sine = Mathf.Sin(phase);
                float noise = (float)(rnd.NextDouble() * 2.0 - 1.0) * 0.35f;
                data[i] = (sine * 0.65f + noise) * volume;
            }
            return data;
        }

        private static float[] SynthBassPulseLoop(float duration, float bpm)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(duration * sampleRate);
            float[] data = new float[samples];
            float beatDur = 60f / bpm;
            float phase = 0f;

            float[] notes = new float[] { 110f, 110f, 130.81f, 98f }; // A2, A2, C3, G2

            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                int currentBeat = Mathf.FloorToInt(time / beatDur) % notes.Length;
                float beatT = (time % beatDur) / beatDur;

                float freq = notes[currentBeat];
                phase += 2f * Mathf.PI * freq / sampleRate;

                float wave = Mathf.Sin(phase) + 0.3f * Mathf.Sin(phase * 2f);
                float env = Mathf.Exp(-beatT * 5f);
                data[i] = wave * env * 0.35f;
            }
            return data;
        }

        private static float[] SynthSpaceDroneLoop(float duration)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(duration * sampleRate);
            float[] data = new float[samples];
            float p1 = 0f, p2 = 0f, p3 = 0f;

            for (int i = 0; i < samples; i++)
            {
                p1 += 2f * Mathf.PI * 65.4f / sampleRate;  // C2
                p2 += 2f * Mathf.PI * 98.0f / sampleRate;  // G2
                p3 += 2f * Mathf.PI * 130.8f / sampleRate; // C3

                float wave = (Mathf.Sin(p1) * 0.5f + Mathf.Sin(p2) * 0.3f + Mathf.Sin(p3) * 0.2f);
                data[i] = wave * 0.3f;
            }
            return data;
        }

        private static float[] SynthArpeggioDown(float duration)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(duration * sampleRate);
            float[] data = new float[samples];
            float[] freqs = new float[] { 329.63f, 261.63f, 220f, 164.81f }; // E4, C4, A3, E3
            float noteDur = duration / freqs.Length;
            float phase = 0f;

            for (int i = 0; i < samples; i++)
            {
                float time = (float)i / sampleRate;
                int idx = Mathf.Clamp(Mathf.FloorToInt(time / noteDur), 0, freqs.Length - 1);
                float noteT = (time % noteDur) / noteDur;

                phase += 2f * Mathf.PI * freqs[idx] / sampleRate;
                float wave = Mathf.Sin(phase);
                float env = 1f - noteT;
                data[i] = wave * env * 0.45f;
            }
            return data;
        }

        private static void SaveWav(string filePath, float[] samples)
        {
            int sampleRate = 44100;
            short channels = 1;
            short bitsPerSample = 16;
            int subChunk2Size = samples.Length * channels * (bitsPerSample / 8);
            int chunkSize = 36 + subChunk2Size;

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                // RIFF header
                bw.Write(new char[] { 'R', 'I', 'F', 'F' });
                bw.Write(chunkSize);
                bw.Write(new char[] { 'W', 'A', 'V', 'E' });

                // fmt chunk
                bw.Write(new char[] { 'f', 'm', 't', ' ' });
                bw.Write(16); // Subchunk1Size
                bw.Write((short)1); // AudioFormat = PCM
                bw.Write(channels);
                bw.Write(sampleRate);
                bw.Write(sampleRate * channels * (bitsPerSample / 8)); // ByteRate
                bw.Write((short)(channels * (bitsPerSample / 8))); // BlockAlign
                bw.Write(bitsPerSample);

                // data chunk
                bw.Write(new char[] { 'd', 'a', 't', 'a' });
                bw.Write(subChunk2Size);

                for (int i = 0; i < samples.Length; i++)
                {
                    short val = (short)Mathf.Clamp(samples[i] * 32767f, -32768f, 32767f);
                    bw.Write(val);
                }
            }
        }

        #endregion

        #region 3. PREFAB & VFX CREATION

        [MenuItem("Tools/Asteroids/3. Crear Prefabs i VFX", false, 12)]
        public static void CreateVFXAndPrefabs()
        {
            CreateVFXPrefabs();
            CreateGamePrefabs();
        }

        private static void CreateVFXPrefabs()
        {
            EnsureDirectories();

            // VFX Explosion Neon
            GameObject neonFxObj = new GameObject("VFX_Explosion_Neon");
            var psNeon = neonFxObj.AddComponent<ParticleSystem>();
            neonFxObj.AddComponent<AutoDestroyVFX>();
            var mainNeon = psNeon.main;
            mainNeon.duration = 0.5f;
            mainNeon.loop = false;
            mainNeon.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.6f);
            mainNeon.startSpeed = new ParticleSystem.MinMaxCurve(4f, 8f);
            mainNeon.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
            mainNeon.startColor = new Color(0.2f, 1f, 1f, 1f);

            var emissionNeon = psNeon.emission;
            emissionNeon.rateOverTime = 0;
            emissionNeon.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 25) });

            var shapeNeon = psNeon.shape;
            shapeNeon.shapeType = ParticleSystemShapeType.Circle;
            shapeNeon.radius = 0.2f;

            PrefabUtility.SaveAsPrefabAsset(neonFxObj, $"{PREFABS_PATH}/VFX_Explosion_Neon.prefab");
            Object.DestroyImmediate(neonFxObj);

            // VFX Explosion Space
            GameObject spaceFxObj = new GameObject("VFX_Explosion_Space");
            var psSpace = spaceFxObj.AddComponent<ParticleSystem>();
            spaceFxObj.AddComponent<AutoDestroyVFX>();
            var mainSpace = psSpace.main;
            mainSpace.duration = 0.6f;
            mainSpace.loop = false;
            mainSpace.startLifetime = new ParticleSystem.MinMaxCurve(0.4f, 0.8f);
            mainSpace.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f);
            mainSpace.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.45f);
            mainSpace.startColor = new Color(1f, 0.6f, 0.2f, 1f);

            var emissionSpace = psSpace.emission;
            emissionSpace.rateOverTime = 0;
            emissionSpace.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 35) });

            var shapeSpace = psSpace.shape;
            shapeSpace.shapeType = ParticleSystemShapeType.Circle;
            shapeSpace.radius = 0.3f;

            PrefabUtility.SaveAsPrefabAsset(spaceFxObj, $"{PREFABS_PATH}/VFX_Explosion_Space.prefab");
            Object.DestroyImmediate(spaceFxObj);

            Debug.Log("[Asteroids] VFX Prefabs creats.");
        }

        private static void CreateGamePrefabs()
        {
            EnsureDirectories();

            Sprite neonLaser = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_laser.png");
            Sprite neonShip = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_ship.png");
            Sprite neonThruster = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_thruster.png");
            Sprite astLarge = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_asteroid_large.png");
            Sprite astMed = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_asteroid_med.png");
            Sprite astSmall = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_asteroid_small.png");

            // 1. PROJECTILE PREFAB
            GameObject projObj = new GameObject("Projectile");
            var projSr = projObj.AddComponent<SpriteRenderer>();
            projSr.sprite = neonLaser;
            projSr.sortingOrder = 5;
            var projCol = projObj.AddComponent<CircleCollider2D>();
            projCol.isTrigger = true;
            projCol.radius = 0.2f;
            var projRb = projObj.AddComponent<Rigidbody2D>();
            projRb.gravityScale = 0f;
            projRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            projObj.AddComponent<Projectile>();
            GameObject projPrefab = PrefabUtility.SaveAsPrefabAsset(projObj, $"{PREFABS_PATH}/Projectile.prefab");
            Object.DestroyImmediate(projObj);

            // 2. ASTEROID SMALL PREFAB
            GameObject astSmallObj = new GameObject("Asteroid_Small");
            var srS = astSmallObj.AddComponent<SpriteRenderer>();
            srS.sprite = astSmall;
            srS.sortingOrder = 2;
            var colS = astSmallObj.AddComponent<CircleCollider2D>();
            colS.radius = 0.25f;
            var rbS = astSmallObj.AddComponent<Rigidbody2D>();
            rbS.gravityScale = 0f;
            astSmallObj.AddComponent<ScreenWrapper>();
            var astCompS = astSmallObj.AddComponent<Asteroid>();
            SetField(astCompS, "tier", AsteroidTier.Small);
            SetField(astCompS, "scoreValue", 100);
            SetField(astCompS, "fragmentCount", 0);
            astSmallObj.AddComponent<AsteroidSkinApplier>().SetTier(AsteroidTier.Small);
            GameObject smallPrefab = PrefabUtility.SaveAsPrefabAsset(astSmallObj, $"{PREFABS_PATH}/Asteroid_Small.prefab");
            Object.DestroyImmediate(astSmallObj);

            // 3. ASTEROID MEDIUM PREFAB
            GameObject astMedObj = new GameObject("Asteroid_Medium");
            var srM = astMedObj.AddComponent<SpriteRenderer>();
            srM.sprite = astMed;
            srM.sortingOrder = 2;
            var colM = astMedObj.AddComponent<CircleCollider2D>();
            colM.radius = 0.45f;
            var rbM = astMedObj.AddComponent<Rigidbody2D>();
            rbM.gravityScale = 0f;
            astMedObj.AddComponent<ScreenWrapper>();
            var astCompM = astMedObj.AddComponent<Asteroid>();
            SetField(astCompM, "tier", AsteroidTier.Medium);
            SetField(astCompM, "scoreValue", 50);
            SetField(astCompM, "smallerAsteroidPrefab", smallPrefab);
            SetField(astCompM, "fragmentCount", 2);
            astMedObj.AddComponent<AsteroidSkinApplier>().SetTier(AsteroidTier.Medium);
            GameObject medPrefab = PrefabUtility.SaveAsPrefabAsset(astMedObj, $"{PREFABS_PATH}/Asteroid_Medium.prefab");
            Object.DestroyImmediate(astMedObj);

            // 4. ASTEROID LARGE PREFAB
            GameObject astLargeObj = new GameObject("Asteroid_Large");
            var srL = astLargeObj.AddComponent<SpriteRenderer>();
            srL.sprite = astLarge;
            srL.sortingOrder = 2;
            var colL = astLargeObj.AddComponent<CircleCollider2D>();
            colL.radius = 0.75f;
            var rbL = astLargeObj.AddComponent<Rigidbody2D>();
            rbL.gravityScale = 0f;
            astLargeObj.AddComponent<ScreenWrapper>();
            var astCompL = astLargeObj.AddComponent<Asteroid>();
            SetField(astCompL, "tier", AsteroidTier.Large);
            SetField(astCompL, "scoreValue", 20);
            SetField(astCompL, "smallerAsteroidPrefab", medPrefab);
            SetField(astCompL, "fragmentCount", 2);
            astLargeObj.AddComponent<AsteroidSkinApplier>().SetTier(AsteroidTier.Large);
            GameObject largePrefab = PrefabUtility.SaveAsPrefabAsset(astLargeObj, $"{PREFABS_PATH}/Asteroid_Large.prefab");
            Object.DestroyImmediate(astLargeObj);

            // 5. PLAYER PREFAB
            GameObject playerObj = new GameObject("Player");
            var pSr = playerObj.AddComponent<SpriteRenderer>();
            pSr.sprite = neonShip;
            pSr.sortingOrder = 10;
            var pCol = playerObj.AddComponent<PolygonCollider2D>();
            var pRb = playerObj.AddComponent<Rigidbody2D>();
            pRb.gravityScale = 0f;
            pRb.linearDamping = 0.6f;
            pRb.angularDamping = 2.0f;
            playerObj.AddComponent<ScreenWrapper>();

            // Thruster child
            GameObject thrusterChild = new GameObject("Thruster");
            thrusterChild.transform.SetParent(playerObj.transform);
            thrusterChild.transform.localPosition = new Vector3(0, -0.32f, 0);
            var thrusterSr = thrusterChild.AddComponent<SpriteRenderer>();
            thrusterSr.sprite = neonThruster;
            thrusterSr.sortingOrder = 9;
            thrusterChild.SetActive(false);

            // Muzzle child
            GameObject muzzleChild = new GameObject("Muzzle");
            muzzleChild.transform.SetParent(playerObj.transform);
            muzzleChild.transform.localPosition = new Vector3(0, 0.45f, 0);

            var pCtrl = playerObj.AddComponent<PlayerController>();
            SetField(pCtrl, "thrusterVisual", thrusterChild);
            SetField(pCtrl, "shipRenderer", pSr);

            var pShoot = playerObj.AddComponent<PlayerShooting>();
            SetField(pShoot, "projectilePrefab", projPrefab);
            SetField(pShoot, "muzzlePoint", muzzleChild.transform);

            var pSkin = playerObj.AddComponent<ShipSkinApplier>();
            SetField(pSkin, "shipRenderer", pSr);
            SetField(pSkin, "thrusterRenderer", thrusterSr);

            PrefabUtility.SaveAsPrefabAsset(playerObj, $"{PREFABS_PATH}/Player.prefab");
            Object.DestroyImmediate(playerObj);

            Debug.Log("[Asteroids] Game Prefabs creats.");
        }

        #endregion

        #region 4. SKINS DATA SCRIPTABLE OBJECTS

        [MenuItem("Tools/Asteroids/4. Crear ScriptableObjects de Skins", false, 13)]
        public static void CreateSkinDataAssets()
        {
            EnsureDirectories();

            // Càrrega de recursos
            Sprite neonShip = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_ship.png");
            Sprite neonThruster = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_thruster.png");
            Sprite neonLaser = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_laser.png");
            Sprite neonAstL = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_asteroid_large.png");
            Sprite neonAstM = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_asteroid_med.png");
            Sprite neonAstS = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_asteroid_small.png");
            Sprite neonLife = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/neon_life.png");

            Sprite spaceShip = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/space_ship.png");
            Sprite spaceThruster = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/space_thruster.png");
            Sprite spaceLaser = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/space_laser.png");
            Sprite spaceAstL = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/space_asteroid_large.png");
            Sprite spaceAstM = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/space_asteroid_med.png");
            Sprite spaceAstS = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/space_asteroid_small.png");
            Sprite spaceLife = AssetDatabase.LoadAssetAtPath<Sprite>($"{SPRITES_PATH}/space_life.png");

            AudioClip neonShoot = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_laser_neon.wav");
            AudioClip neonThrust = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_thrust_neon.wav");
            AudioClip neonShipBoom = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_explosion_ship_neon.wav");
            AudioClip neonAstBoom = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_explosion_asteroid_neon.wav");
            AudioClip neonBgm = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/bgm_neon_loop.wav");

            AudioClip spaceShoot = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_laser_space.wav");
            AudioClip spaceThrust = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_thrust_space.wav");
            AudioClip spaceShipBoom = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_explosion_ship_space.wav");
            AudioClip spaceAstBoom = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_explosion_asteroid_space.wav");
            AudioClip spaceBgm = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/bgm_space_loop.wav");

            AudioClip gameOverSfx = AssetDatabase.LoadAssetAtPath<AudioClip>($"{AUDIO_PATH}/sfx_gameover.wav");

            GameObject neonVfx = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_PATH}/VFX_Explosion_Neon.prefab");
            GameObject spaceVfx = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_PATH}/VFX_Explosion_Space.prefab");

            // SKIN 1: RETRO NEON
            string neonPath = $"{SKINS_PATH}/Skin_RetroNeon.asset";
            GameSkinData skinNeon = AssetDatabase.LoadAssetAtPath<GameSkinData>(neonPath);
            if (skinNeon == null)
            {
                skinNeon = ScriptableObject.CreateInstance<GameSkinData>();
                AssetDatabase.CreateAsset(skinNeon, neonPath);
            }
            skinNeon.skinName = "Retro Neon";
            skinNeon.skinDescription = "Clàssic arcade d'estil vectorial amb colors neó i síntesi 8-bit.";
            skinNeon.themeAccentColor = new Color(0.1f, 1f, 1f, 1f);
            skinNeon.shipSprite = neonShip;
            skinNeon.shipColor = Color.white;
            skinNeon.thrusterSprite = neonThruster;
            skinNeon.shootSfx = neonShoot;
            skinNeon.thrustSfx = neonThrust;
            skinNeon.shipExplosionPrefab = neonVfx;
            skinNeon.shipExplosionSfx = neonShipBoom;
            skinNeon.projectileSprite = neonLaser;
            skinNeon.projectileColor = new Color(1f, 0.2f, 0.4f, 1f);
            skinNeon.projectileScale = new Vector2(1.2f, 1.2f);
            skinNeon.largeAsteroidSprites = new Sprite[] { neonAstL };
            skinNeon.mediumAsteroidSprites = new Sprite[] { neonAstM };
            skinNeon.smallAsteroidSprites = new Sprite[] { neonAstS };
            skinNeon.asteroidTint = Color.white;
            skinNeon.asteroidExplosionPrefab = neonVfx;
            skinNeon.asteroidExplosionSfx = neonAstBoom;
            skinNeon.backgroundMusic = neonBgm;
            skinNeon.gameOverSfx = gameOverSfx;
            skinNeon.backgroundColor = new Color(0.02f, 0.02f, 0.05f, 1f);
            skinNeon.lifeIconSprite = neonLife;
            skinNeon.uiTextColor = Color.white;
            EditorUtility.SetDirty(skinNeon);

            // SKIN 2: DEEP SPACE
            string spacePath = $"{SKINS_PATH}/Skin_DeepSpace.asset";
            GameSkinData skinSpace = AssetDatabase.LoadAssetAtPath<GameSkinData>(spacePath);
            if (skinSpace == null)
            {
                skinSpace = ScriptableObject.CreateInstance<GameSkinData>();
                AssetDatabase.CreateAsset(skinSpace, spacePath);
            }
            skinSpace.skinName = "Deep Space";
            skinSpace.skinDescription = "Fighter d'alta tecnologia daurat, plasma d'alta energia i asteroides rocosos densos.";
            skinSpace.themeAccentColor = new Color(1f, 0.75f, 0.2f, 1f);
            skinSpace.shipSprite = spaceShip;
            skinSpace.shipColor = Color.white;
            skinSpace.thrusterSprite = spaceThruster;
            skinSpace.shootSfx = spaceShoot;
            skinSpace.thrustSfx = spaceThrust;
            skinSpace.shipExplosionPrefab = spaceVfx;
            skinSpace.shipExplosionSfx = spaceShipBoom;
            skinSpace.projectileSprite = spaceLaser;
            skinSpace.projectileColor = new Color(0.2f, 1f, 0.4f, 1f);
            skinSpace.projectileScale = new Vector2(1f, 1f);
            skinSpace.largeAsteroidSprites = new Sprite[] { spaceAstL };
            skinSpace.mediumAsteroidSprites = new Sprite[] { spaceAstM };
            skinSpace.smallAsteroidSprites = new Sprite[] { spaceAstS };
            skinSpace.asteroidTint = Color.white;
            skinSpace.asteroidExplosionPrefab = spaceVfx;
            skinSpace.asteroidExplosionSfx = spaceAstBoom;
            skinSpace.backgroundMusic = spaceBgm;
            skinSpace.gameOverSfx = gameOverSfx;
            skinSpace.backgroundColor = new Color(0.04f, 0.04f, 0.04f, 1f);
            skinSpace.lifeIconSprite = spaceLife;
            skinSpace.uiTextColor = new Color(1f, 0.95f, 0.8f, 1f);
            EditorUtility.SetDirty(skinSpace);

            AssetDatabase.SaveAssets();
            Debug.Log("[Asteroids] ScriptableObjects de Skins creats.");
        }

        #endregion

        #region 5. SCENE SETUP

        [MenuItem("Tools/Asteroids/5. Configurar Escena Activa", false, 14)]
        public static void SetupSampleScene()
        {
            var scene = EditorSceneManager.GetActiveScene();

            // 1. Camera & Background Applier
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            cam.orthographic = true;
            cam.orthographicSize = 6.5f;
            cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f, 1f);
            cam.transform.position = new Vector3(0, 0, -10);

            if (!cam.GetComponent<BackgroundSkinApplier>())
            {
                cam.gameObject.AddComponent<BackgroundSkinApplier>();
            }

            // 2. Skins & SkinManager
            GameSkinData skinNeon = AssetDatabase.LoadAssetAtPath<GameSkinData>($"{SKINS_PATH}/Skin_RetroNeon.asset");
            GameSkinData skinSpace = AssetDatabase.LoadAssetAtPath<GameSkinData>($"{SKINS_PATH}/Skin_DeepSpace.asset");

            GameObject gmObj = GameObject.Find("GameManagers");
            if (gmObj == null) gmObj = new GameObject("GameManagers");

            var skinMgr = gmObj.GetComponent<SkinManager>() ?? gmObj.AddComponent<SkinManager>();
            SetField(skinMgr, "availableSkins", new GameSkinData[] { skinNeon, skinSpace });

            // 3. Audio Manager
            var audioMgr = gmObj.GetComponent<AudioManager>() ?? gmObj.AddComponent<AudioManager>();

            // 4. Asteroid Spawner
            var spawner = gmObj.GetComponent<AsteroidSpawner>() ?? gmObj.AddComponent<AsteroidSpawner>();
            GameObject largeAstPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_PATH}/Asteroid_Large.prefab");
            SetField(spawner, "largeAsteroidPrefab", largeAstPrefab);

            // 5. Player in scene
            GameObject playerInScene = GameObject.Find("Player");
            if (playerInScene == null)
            {
                GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PREFABS_PATH}/Player.prefab");
                if (playerPrefab != null)
                {
                    playerInScene = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
                    playerInScene.name = "Player";
                    playerInScene.transform.position = Vector3.zero;
                }
            }

            // 6. GameManager
            var gameMgr = gmObj.GetComponent<GameManager>() ?? gmObj.AddComponent<GameManager>();
            if (playerInScene != null)
            {
                SetField(gameMgr, "player", playerInScene.GetComponent<PlayerController>());
            }

            // 7. UI Canvas & Elements
            SetupUI(gmObj, skinNeon);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            Debug.Log("[Asteroids] Escena configurada i desada correctament.");
        }

        private static void SetupUI(GameObject gmObj, GameSkinData initialSkin)
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            GameObject canvasObj;
            if (canvas == null)
            {
                canvasObj = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                canvas = canvasObj.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObj.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
            }
            else
            {
                canvasObj = canvas.gameObject;
            }

            // Assegurar EventSystem amb el mòdul correcte (nou Input System)
            var existingES = Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (existingES == null)
            {
                var esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
#if ENABLE_INPUT_SYSTEM
                esObj.AddComponent<InputSystemUIInputModule>();
#else
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
            }
            else
            {
                // Substituir StandaloneInputModule per InputSystemUIInputModule si escau
#if ENABLE_INPUT_SYSTEM
                var oldModule = existingES.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                if (oldModule != null)
                {
                    Object.DestroyImmediate(oldModule);
                    existingES.gameObject.AddComponent<InputSystemUIInputModule>();
                    Debug.Log("<color=cyan>[Asteroids] StandaloneInputModule substituït per InputSystemUIInputModule.</color>");
                }
#endif
            }

            // HUD Container
            Transform hudTf = canvasObj.transform.Find("HUD");
            GameObject hudObj;
            if (hudTf == null)
            {
                hudObj = new GameObject("HUD", typeof(RectTransform));
                hudObj.transform.SetParent(canvasObj.transform, false);
                var rt = hudObj.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            else
            {
                hudObj = hudTf.gameObject;
            }

            // Text Score
            TextMeshProUGUI scoreText = CreateOrGetTMP(hudObj.transform, "ScoreText", "SCORE: 0", 36, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(40f, -40f), new Vector2(350f, 60f), TextAlignmentOptions.TopLeft);

            // Text HighScore
            TextMeshProUGUI highScoreText = CreateOrGetTMP(hudObj.transform, "HighScoreText", "BEST: 0", 30, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(350f, 60f), TextAlignmentOptions.TopRight);

            // Text Wave
            TextMeshProUGUI waveText = CreateOrGetTMP(hudObj.transform, "WaveText", "WAVE 1", 34, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(300f, 60f), TextAlignmentOptions.Top);

            // Text Theme Indicator / Hint
            TextMeshProUGUI skinText = CreateOrGetTMP(hudObj.transform, "ThemeText", "THEME: RETRO NEON [Prem 'T']", 24, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(40f, 40f), new Vector2(450f, 50f), TextAlignmentOptions.BottomLeft);

            // Lives Container
            Transform livesTf = hudObj.transform.Find("LivesContainer");
            GameObject livesContainer;
            if (livesTf == null)
            {
                livesContainer = new GameObject("LivesContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
                livesContainer.transform.SetParent(hudObj.transform, false);
                var rt = livesContainer.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(40f, -100f);
                rt.sizeDelta = new Vector2(200f, 36f);

                var layout = livesContainer.GetComponent<HorizontalLayoutGroup>();
                layout.spacing = 10;
                layout.childControlWidth = false;
                layout.childControlHeight = false;
            }
            else
            {
                livesContainer = livesTf.gameObject;
            }

            // Game Over Panel
            Transform goTf = canvasObj.transform.Find("GameOverPanel");
            GameObject goPanel;
            if (goTf == null)
            {
                goPanel = new GameObject("GameOverPanel", typeof(RectTransform), typeof(Image));
                goPanel.transform.SetParent(canvasObj.transform, false);
                var rt = goPanel.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.5f);
                rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.sizeDelta = new Vector2(500f, 350f);
                var img = goPanel.GetComponent<Image>();
                img.color = new Color(0.05f, 0.05f, 0.1f, 0.9f);
            }
            else
            {
                goPanel = goTf.gameObject;
            }

            CreateOrGetTMP(goPanel.transform, "Title", "GAME OVER", 54, new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), Vector2.zero, new Vector2(450f, 70f), TextAlignmentOptions.Center);
            TextMeshProUGUI finalScoreText = CreateOrGetTMP(goPanel.transform, "FinalScore", "FINAL SCORE\n0", 32, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(450f, 80f), TextAlignmentOptions.Center);

            // Restart Button
            Transform btnTf = goPanel.transform.Find("RestartButton");
            GameObject btnObj;
            if (btnTf == null)
            {
                btnObj = new GameObject("RestartButton", typeof(RectTransform), typeof(Image), typeof(Button));
                btnObj.transform.SetParent(goPanel.transform, false);
                var rt = btnObj.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.2f);
                rt.anchorMax = new Vector2(0.5f, 0.2f);
                rt.sizeDelta = new Vector2(260f, 55f);
                btnObj.GetComponent<Image>().color = new Color(0.1f, 0.6f, 0.9f, 1f);

                CreateOrGetTMP(btnObj.transform, "BtnText", "RESTART (R)", 26, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(250f, 50f), TextAlignmentOptions.Center);
            }
            else
            {
                btnObj = btnTf.gameObject;
            }
            Button restartBtn = btnObj.GetComponent<Button>();
            goPanel.SetActive(false);

            // Wire UIManager
            var uiMgr = canvasObj.GetComponent<UIManager>() ?? canvasObj.AddComponent<UIManager>();
            SetField(uiMgr, "scoreText", scoreText);
            SetField(uiMgr, "highScoreText", highScoreText);
            SetField(uiMgr, "waveText", waveText);
            SetField(uiMgr, "skinIndicatorText", skinText);
            SetField(uiMgr, "livesContainer", livesContainer.transform);
            SetField(uiMgr, "gameOverPanel", goPanel);
            SetField(uiMgr, "finalScoreText", finalScoreText);
            SetField(uiMgr, "restartButton", restartBtn);
        }

        private static TextMeshProUGUI CreateOrGetTMP(Transform parent, string name, string text, float fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, TextAlignmentOptions align)
        {
            Transform existing = parent.Find(name);
            GameObject obj;
            if (existing == null)
            {
                obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
                obj.transform.SetParent(parent, false);
            }
            else
            {
                obj = existing.gameObject;
            }

            var rt = obj.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin;
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            var tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = align;
            tmp.color = Color.white;
            return tmp;
        }

        private static void SetField(object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (field != null)
            {
                field.SetValue(target, value);
            }
        }

        // ──────────────────────────────────────────────────────────────────────
        //  MAIN MENU SETUP
        // ──────────────────────────────────────────────────────────────────────

        private const string CONFIGS_PATH = "Assets/ScriptableObjects/Configs";

        [MenuItem("Tools/Asteroids/6. Configurar Menú Principal", false, 15)]
        public static void SetupMainMenu()
        {
            EnsureConfigsDir();

            // ── 1. Crear o carregar MainMenuConfig ──────────────────────────
            const string configPath = CONFIGS_PATH + "/MainMenuConfig.asset";
            var config = AssetDatabase.LoadAssetAtPath<MainMenuConfig>(configPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<MainMenuConfig>();
                config.titleText        = "ASTEROIDS";
                config.subtitleText     = "SURVIVE THE VOID";
                config.playButtonText   = "JUGAR";
                config.versionText      = "v1.0";
                config.backgroundColor  = new Color(0.04f, 0.04f, 0.07f, 1f);
                config.titleColor       = Color.white;
                config.titleFontSize    = 96f;
                config.subtitleColor    = new Color(0.6f, 0.7f, 1f, 1f);
                config.subtitleFontSize = 28f;
                config.buttonNormalColor  = new Color(0.1f, 0.75f, 1f, 1f);
                config.buttonHoverColor   = new Color(0.3f, 0.95f, 1f, 1f);
                config.buttonPressedColor = new Color(0.0f, 0.45f, 0.7f, 1f);
                config.buttonTextColor    = new Color(0.03f, 0.03f, 0.06f, 1f);
                config.buttonFontSize     = 36f;
                config.buttonWidth        = 280f;
                config.buttonHeight       = 80f;
                config.buttonCornerRadius = 12f;
                config.versionColor       = new Color(0.45f, 0.45f, 0.45f, 1f);
                config.versionFontSize    = 18f;

                AssetDatabase.CreateAsset(config, configPath);
                AssetDatabase.SaveAssets();
                Debug.Log($"[Asteroids] MainMenuConfig creat a {configPath}");
            }

            // ── 2. Obtenir el Canvas de l'escena ────────────────────────────
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("[Asteroids] No s'ha trobat cap Canvas. Executa primer '5. Configurar Escena Activa'.");
                return;
            }
            Transform canvasTf = canvas.transform;

            // ── 3. Crear o reutilitzar el GameObject "MainMenu" ─────────────
            Transform menuTf = canvasTf.Find("MainMenu");
            GameObject menuObj;
            if (menuTf != null)
            {
                menuObj = menuTf.gameObject;
            }
            else
            {
                menuObj = new GameObject("MainMenu", typeof(RectTransform));
                menuObj.transform.SetParent(canvasTf, false);
            }

            // Estirar a pantalla completa
            var menuRt = menuObj.GetComponent<RectTransform>();
            menuRt.anchorMin = Vector2.zero;
            menuRt.anchorMax = Vector2.one;
            menuRt.offsetMin = Vector2.zero;
            menuRt.offsetMax = Vector2.zero;

            // Situar darrera del HUD (índex 0)
            menuObj.transform.SetAsFirstSibling();

            // ── 4. Fons ─────────────────────────────────────────────────────
            GameObject bgObj = GetOrCreateChild(menuObj.transform, "Background",
                typeof(RectTransform), typeof(Image));
            var bgRt = bgObj.GetComponent<RectTransform>();
            bgRt.anchorMin = Vector2.zero;
            bgRt.anchorMax = Vector2.one;
            bgRt.offsetMin = Vector2.zero;
            bgRt.offsetMax = Vector2.zero;
            var bgImg = bgObj.GetComponent<Image>();
            bgImg.color = config.backgroundColor;

            // ── 5. Títol ─────────────────────────────────────────────────────
            var titleTMP = CreateOrGetTMP(
                menuObj.transform, "Title",
                config.titleText, config.titleFontSize,
                new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f),
                Vector2.zero, new Vector2(900f, 130f),
                TextAlignmentOptions.Center);
            titleTMP.color = config.titleColor;
            titleTMP.fontStyle = FontStyles.Bold;

            // ── 6. Subtítol ──────────────────────────────────────────────────
            var subtitleTMP = CreateOrGetTMP(
                menuObj.transform, "Subtitle",
                config.subtitleText, config.subtitleFontSize,
                new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f),
                Vector2.zero, new Vector2(700f, 60f),
                TextAlignmentOptions.Center);
            subtitleTMP.color = config.subtitleColor;
            subtitleTMP.gameObject.SetActive(!string.IsNullOrWhiteSpace(config.subtitleText));

            // ── 7. Botó Play ─────────────────────────────────────────────────
            GameObject btnObj = GetOrCreateChild(menuObj.transform, "PlayButton",
                typeof(RectTransform), typeof(Image), typeof(Button));
            var btnRt = btnObj.GetComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0.5f, 0.4f);
            btnRt.anchorMax = new Vector2(0.5f, 0.4f);
            btnRt.pivot     = new Vector2(0.5f, 0.5f);
            btnRt.anchoredPosition = Vector2.zero;
            btnRt.sizeDelta = new Vector2(config.buttonWidth, config.buttonHeight);

            var btnImg = btnObj.GetComponent<Image>();
            btnImg.color = config.buttonNormalColor;

            var btn = btnObj.GetComponent<Button>();
            var btnColors = btn.colors;
            btnColors.normalColor     = config.buttonNormalColor;
            btnColors.highlightedColor = config.buttonHoverColor;
            btnColors.pressedColor    = config.buttonPressedColor;
            btnColors.selectedColor   = config.buttonHoverColor;
            btn.colors = btnColors;

            // Text del botó
            var btnLabelTMP = CreateOrGetTMP(
                btnObj.transform, "PlayLabel",
                config.playButtonText, config.buttonFontSize,
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(config.buttonWidth - 20f, config.buttonHeight - 10f),
                TextAlignmentOptions.Center);
            btnLabelTMP.color = config.buttonTextColor;
            btnLabelTMP.fontStyle = FontStyles.Bold;

            // ── 8. Text de versió ────────────────────────────────────────────
            var versionTMP = CreateOrGetTMP(
                menuObj.transform, "Version",
                config.versionText, config.versionFontSize,
                new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-20f, 20f), new Vector2(200f, 40f),
                TextAlignmentOptions.BottomRight);
            versionTMP.color = config.versionColor;
            versionTMP.gameObject.SetActive(!string.IsNullOrWhiteSpace(config.versionText));

            // ── 9. Afegir i connectar MainMenuManager ────────────────────────
            var mgr = menuObj.GetComponent<MainMenuManager>() ?? menuObj.AddComponent<MainMenuManager>();
            SetField(mgr, "config",          config);
            SetField(mgr, "backgroundImage", bgImg);
            SetField(mgr, "titleLabel",      titleTMP);
            SetField(mgr, "subtitleLabel",   subtitleTMP);
            SetField(mgr, "playButton",      btn);
            SetField(mgr, "playButtonLabel", btnLabelTMP);
            SetField(mgr, "versionLabel",    versionTMP);

            // ── 10. Desar ────────────────────────────────────────────────────
            EditorUtility.SetDirty(menuObj);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

            Debug.Log("<color=cyan>[Asteroids] Menú principal creat i configurat correctament.</color>");
        }

        private static void EnsureConfigsDir()
        {
            if (!Directory.Exists(CONFIGS_PATH))
                Directory.CreateDirectory(CONFIGS_PATH);
            AssetDatabase.Refresh();
        }

        private static GameObject GetOrCreateChild(Transform parent, string childName, params System.Type[] components)
        {
            Transform existing = parent.Find(childName);
            if (existing != null) return existing.gameObject;

            var go = new GameObject(childName);
            go.transform.SetParent(parent, false);
            foreach (var c in components)
            {
                if (go.GetComponent(c) == null)
                    go.AddComponent(c);
            }
            return go;
        }

        #endregion
    }
}
