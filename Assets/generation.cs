using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.Sprites;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using TinkerWorX.AccidentalNoiseLibrary;

enum terrain {
    None,
    DeepOcean,
    Ocean,
    Beach,
    Plain,
    Hills,
    Mountain,
    MountainTop
}
enum climate
{
    None,
    Polar,
    Cold,
    Temperate,
    Subtropical,
    Tropcial,
}
public enum moisture
{
    None,
    Lowest,
    Low,
    Medium,
    High,
    Highest,
}
enum biome
{
    None,
    Ice,
    SnowWasteland,
    Tundra,
    BorealForest,
    Grassland, 
    TemperateForest,
    SeasonalForest,
    Savanna,
    Desert, 
    Rainforest,
    WarmOcean,
    TemperateOcean,
    ColdOcean,
}
static class gameColors
{
    public static class terrainColors
    {
        public static Color DeepOcean = new Color(68 / 255f, 139 / 255f, 237 / 255f, 1);
        public static Color Ocean = new Color(93 / 255f, 180 / 255f, 246 / 255f, 1);
        public static Color Beach = new Color(232 / 255f, 228 / 255f, 167 / 255f, 1);
        public static Color Plain = new Color(129 / 255f, 179 / 255f, 96 / 255f, 1);
        public static Color Hills = new Color(102 / 255f, 142 / 255f, 87 / 255f, 1);
        public static Color Mountain = new Color(99 / 255f, 110 / 255f, 114 / 255f, 1);
        public static Color MountainTop = new Color(1, 1, 1, 1);
    }

    public static class climateColors
    {
        public static Color Polar = new Color(170 / 255f, 1, 1, 1);
        public static Color Cold = new Color(0, 229 / 255f, 133 / 255f, 1);
        public static Color Temperate = new Color(1, 1, 100 / 255f, 1);
        public static Color Subtropical = new Color(1, 100 / 255f, 0, 1);
        public static Color Tropical = new Color(241 / 255f, 12 / 255f, 0, 1);
    }

    public static class MoistureColors
    {
        public static Color Highest = new Color(20 / 255f, 70 / 255f, 255 / 255f, 1);
        public static Color High = new Color(85 / 255f, 255 / 255f, 255 / 255f, 1);
        public static Color Medium = new Color(80 / 255f, 255 / 255f, 0 / 255f, 1);
        public static Color Low = new Color(245 / 255f, 245 / 255f, 23 / 255f, 1);
        public static Color Lowest = new Color(255 / 255f, 139 / 255f, 17 / 255f, 1);
    }

    public static class BiomeColors
    {
        public static Color Ice = new Color(1f, 1f, 1f, 1);
        public static Color SnowWasteland = new Color(0.828f, 0.915f, 0.92f, 1);
        public static Color Tundra = new Color(0.397f, 0.628f, 0.64f, 1);
        public static Color BorealForest = new Color(0.0228f, 0.38f, 0.285f, 1);
        public static Color Savanna = new Color(0.64f, 0.571f, 0.0448f, 1);
        public static Color Grassland = new Color(129 / 255f, 179 / 255f, 96 / 255f, 1);
        public static Color TemperateForest = new Color(0.0144f, 0.36f, 0.135f, 1);
        public static Color SeasonalForest = new Color(0.063f, 0.45f, 0.198f, 1);
        public static Color Desert = new Color(232 / 255f, 228 / 255f, 167 / 255f, 1);
        public static Color Rainforest = new Color(0.33f, 0.77f, 0.231f, 1);
        public static Color WarmOcean = new Color(0.315f, 0.71f, 0.83f, 1);
        public static Color TemperateOcean = new Color(0.198f, 0.522f, 0.62f, 1);
        public static Color ColdOcean = new Color(0.147f, 0.387f, 0.46f, 1);
    }
}

class gameTile {
    public terrain terrainType { get; set; }
    public biome biomeType { get; set; }
    public climate climateType { get; set; }
    public moisture moistureType { get; set; }
    public Tile tile { get; set; }

    public gameTile() {
        terrainType = terrain.None;
        biomeType = biome.None;
        climateType = climate.None;
        moistureType = moisture.None;
        tile = (Tile)ScriptableObject.CreateInstance("Tile");
        tile.flags = TileFlags.None;
    }
}

public class generation : MonoBehaviour {
    private gameTile[,] gameMap;

    Tilemap colorMap = null;
    public float[,] noiseMap = null;

    [Header("Generation settings")]
    [SerializeField]
    int mapHeight = 256;
    [SerializeField]
    int mapWidth = 256;
    [SerializeField]
    int TerrainOctaves = 6;
    [SerializeField]
    double TerrainFrequency = 1.75;
    [SerializeField]
    int tempOctaves = 4;
    [SerializeField]
    double tempFrequency = 3.25;
    [SerializeField]
    int moistureOctaves = 4;
    [SerializeField]
    double moistureFrequency = 3.0;
    [SerializeField]
    Sprite square = null;

    [Header("Terrain settings")]
    [SerializeField]
    float MountainTopValue = 0.6375f;
    [SerializeField]
    float MountainValue = 0.52f;
    [SerializeField]
    float HillsValue = 0.42f;
    [SerializeField]
    float PlainsValue = 0.31f;
    [SerializeField]
    float BeachValue = 0.28f;
    [SerializeField]
    float OceanValue = 0.15f;

    [Header("Climate settings")]
    [SerializeField]
    float TropicalValue = 0.95f;
    [SerializeField]
    float SubtropicalValue = 0.85f;
    [SerializeField]
    float TemperateValue = 0.65f;
    [SerializeField]
    float ColdValue = 0.48f;

    [Header("Moisture settings")]
    [SerializeField]
    float HighestValue = 0.9f;
    [SerializeField]
    float HighValue = 0.62f;
    [SerializeField]
    float MediumValue = 0.39f;
    [SerializeField]
    float LowValue = 0.21f;

    // New noise map generation
    //[Header("New noise map generation settings")]
    //[SerializeField]
    //public float scale = 20f;
    //[SerializeField]
    //public int seed = 0;
    //[SerializeField]
    //public int octaves = 6;
    //[SerializeField]
    //public float persistence = 0.5f;
    //[SerializeField]
    //public float lacunarity = 2f;

    int MountainFrequency = 50;

    ImplicitFractal terrainFractal = null;
    ImplicitFractal tempFractal = null;
    ImplicitFractal moistureFractal = null;
    ImplicitGradient tempGradient = null;

    // Start is called before the first frame update
    void Start() {
        Random.InitState(Time.frameCount);
        gameMap = new gameTile[mapWidth, mapHeight];
        colorMap = GetComponent<Tilemap>();
        Initialize();
        generateMap();
    }
    private void generateMap()
    {
        //Terrain types
        terrainFractal.Seed = Random.Range(0, int.MaxValue);
        generateNoiseMap(new float[] { 1 }, terrainFractal);
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMap[x, y] = new gameTile();
                if (noiseMap[x, y] > MountainTopValue)
                {
                    gameMap[x, y].terrainType = terrain.MountainTop;
                }
                else if (noiseMap[x, y] > MountainValue)
                {
                    gameMap[x, y].terrainType = terrain.Mountain;
                }
                else if (noiseMap[x, y] > HillsValue)
                {
                    gameMap[x, y].terrainType = terrain.Hills;
                }
                else if (noiseMap[x, y] > PlainsValue)
                {
                    gameMap[x, y].terrainType = terrain.Plain;
                }
                else if (noiseMap[x, y] > BeachValue)
                {
                    gameMap[x, y].terrainType = terrain.Beach;
                }
                else if (noiseMap[x, y] > OceanValue)
                {
                    gameMap[x, y].terrainType = terrain.Ocean;
                }
                else
                {
                    gameMap[x, y].terrainType = terrain.DeepOcean;
                }
            }
        }

        //Climate Zones
        tempFractal.Seed = Random.Range(0, int.MaxValue);
        generateNoiseMap(new float[] { 0.85f, 0.15f }, tempGradient, tempFractal);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (noiseMap[x, y] > TropicalValue)
                {
                    gameMap[x, y].climateType = climate.Tropcial;
                }
                else if (noiseMap[x, y] > SubtropicalValue)
                {
                    gameMap[x, y].climateType = climate.Subtropical;
                }
                else if (noiseMap[x, y] > TemperateValue)
                {
                    gameMap[x, y].climateType = climate.Temperate;
                }
                else if (noiseMap[x, y] > ColdValue)
                {
                    gameMap[x, y].climateType = climate.Cold;
                }
                else
                {
                    gameMap[x, y].climateType = climate.Polar;
                }
            }
        }

        //Moisture
        generateNoiseMap(new float[] { 1 }, moistureFractal);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //adjust value according to climate
                if (gameMap[x, y].climateType == climate.Cold)
                {
                    noiseMap[x, y] -= 0.2f * ColdValue;
                }
                else if (gameMap[x, y].climateType == climate.Polar)
                {
                    noiseMap[x, y] += 0.55f * ColdValue;
                }

                //adjust value according to terrain
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                {
                    noiseMap[x, y] += 8f * OceanValue;
                }
                else if (gameMap[x, y].terrainType == terrain.Ocean)
                {
                    noiseMap[x, y] += 3f * OceanValue;
                }
                else if (gameMap[x, y].terrainType == terrain.Beach)
                {
                    noiseMap[x, y] += 1f * BeachValue;
                }
                else if (gameMap[x, y].terrainType == terrain.Plain)
                {
                    noiseMap[x, y] += 0.25f * PlainsValue;
                }

                //Select moisture level
                if (noiseMap[x, y] > HighestValue)
                {
                    gameMap[x, y].moistureType = moisture.Highest;
                }
                else if (noiseMap[x, y] > HighValue)
                {
                    gameMap[x, y].moistureType = moisture.High;
                }
                else if (noiseMap[x, y] > MediumValue)
                {
                    gameMap[x, y].moistureType = moisture.Medium;
                }
                else if (noiseMap[x, y] > LowValue)
                {
                    gameMap[x, y].moistureType = moisture.Low;
                }
                else
                {
                    gameMap[x, y].moistureType = moisture.Lowest;
                }
            }
        }

        //Biomes
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //DeepOcean
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                {
                    if (gameMap[x, y].climateType == climate.Tropcial ||
                        gameMap[x, y].climateType == climate.Subtropical)
                    {
                        gameMap[x, y].biomeType = biome.WarmOcean;
                    }
                    else if (gameMap[x, y].climateType == climate.Temperate)
                    {
                        gameMap[x, y].biomeType = biome.TemperateOcean;
                    }
                    else gameMap[x, y].biomeType = biome.ColdOcean;
                }

                //Ocean
                else if (gameMap[x, y].terrainType == terrain.Ocean)
                {
                    if (gameMap[x, y].climateType == climate.Tropcial ||
                        gameMap[x, y].climateType == climate.Subtropical)
                    {
                        gameMap[x, y].biomeType = biome.WarmOcean;
                    }
                    else if (gameMap[x, y].climateType == climate.Temperate)
                    {
                        gameMap[x, y].biomeType = biome.TemperateOcean;
                    }
                    else gameMap[x, y].biomeType = biome.ColdOcean;
                }

                //Plains, Hills, Mountains
                else
                {
                    //Polar
                    if (gameMap[x, y].climateType == climate.Polar)
                    {
                        if (gameMap[x, y].moistureType == moisture.Lowest ||
                            gameMap[x, y].moistureType == moisture.Low)
                        {
                            gameMap[x, y].biomeType = biome.SnowWasteland;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Medium)
                        {
                            gameMap[x, y].biomeType = biome.Tundra;
                        }
                        else
                        {
                            gameMap[x, y].biomeType = biome.Ice;
                        }
                    }

                    //Cold
                    else if (gameMap[x, y].climateType == climate.Cold)
                    {
                        if (gameMap[x, y].moistureType == moisture.Lowest)
                        {
                            gameMap[x, y].biomeType = biome.SnowWasteland;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Low ||
                            gameMap[x, y].moistureType == moisture.Medium)
                        {
                            gameMap[x, y].biomeType = biome.Tundra;
                        }
                        else
                        {
                            gameMap[x, y].biomeType = biome.BorealForest;
                        }
                    }

                    //Temperate
                    else if (gameMap[x, y].climateType == climate.Temperate)
                    {
                        if (gameMap[x, y].moistureType == moisture.Lowest)
                        {
                            gameMap[x, y].biomeType = biome.Savanna;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Low)
                        {
                            gameMap[x, y].biomeType = biome.Grassland;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Medium)
                        {
                            gameMap[x, y].biomeType = biome.TemperateForest;
                        }
                        else
                        {
                            gameMap[x, y].biomeType = biome.SeasonalForest;
                        }
                    }

                    //Subtropical
                    else if (gameMap[x, y].climateType == climate.Subtropical)
                    {
                        if (gameMap[x, y].moistureType == moisture.Lowest)
                        {
                            gameMap[x, y].biomeType = biome.Desert;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Low ||
                            gameMap[x, y].moistureType == moisture.Medium)
                        {
                            gameMap[x, y].biomeType = biome.Savanna;
                        }
                        else
                        {
                            gameMap[x, y].biomeType = biome.Rainforest;
                        }
                    }

                    //Tropical
                    else if (gameMap[x, y].climateType == climate.Tropcial)
                    {
                        if (gameMap[x, y].moistureType == moisture.Lowest ||
                            gameMap[x, y].moistureType == moisture.Low)
                        {
                            gameMap[x, y].biomeType = biome.Desert;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Medium)
                        {
                            gameMap[x, y].biomeType = biome.Savanna;
                        }
                        else
                        {
                            gameMap[x, y].biomeType = biome.Rainforest;
                        }
                    }
                }
            }
        }
        paint_moisture();
        //paint_ClimateZones();
    }
    private void SetTileColour(Tile tile, Color colour, Vector3Int position) {
        tile = (Tile)ScriptableObject.CreateInstance("Tile");
        tile.sprite = square;
        tile.color = colour;
        colorMap.SetTile(position, tile);
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            generateMap();
        }
    }
    private void Initialize() {

        //terrain
        terrainFractal = new ImplicitFractal (FractalType.Multi,
                             BasisType.Simplex,
                             InterpolationType.Quintic);
        terrainFractal.Octaves = TerrainOctaves;
        terrainFractal.Frequency = TerrainFrequency;

        //temp
        tempFractal = new ImplicitFractal(FractalType.Multi,
                             BasisType.Simplex,
                             InterpolationType.Quintic);
        tempFractal.Octaves = tempOctaves;
        tempFractal.Frequency = tempFrequency;

        tempGradient = new ImplicitGradient(1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1);

        //moisture
        moistureFractal = new ImplicitFractal(FractalType.Multi,
                             BasisType.Simplex,
                             InterpolationType.Quintic);
        moistureFractal.Octaves = moistureOctaves;
        moistureFractal.Frequency = moistureFrequency;
    }
    private void generateNoiseMap(float[] weights, params ImplicitModuleBase[] fractals) {
        float max = 0;
        float min = 0;
        int i = 0;
        float value = 0;
        noiseMap = new float[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {

                ////Сэмплируем шум с небольшими интервалами
                //float x1 = x / (float)mapWidth;
                //float y1 = y / (float)mapHeight;

                //float value = (float)terrainFractal.Get(x1, y1);
                
                // Пределы шума
                float x1 = 0, x2 = 2;
                float y1 = 0, y2 = 2;
                float dx = x2 - x1;
                float dy = y2 - y1;

                //Сэмплируем шум с небольшими интервалами
                float s = x / (float)mapWidth;
                float t = y / (float)mapHeight;

                // Вычисляем четырехмерные координаты
                float nx = x1 + Mathf.Cos(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                float ny = y1 + Mathf.Cos(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);
                float nz = x1 + Mathf.Sin(s * 2 * Mathf.PI) * dx / (2 * Mathf.PI);
                float nw = y1 + Mathf.Sin(t * 2 * Mathf.PI) * dy / (2 * Mathf.PI);

                foreach (ImplicitModuleBase fractal in fractals)
                {
                    value += (float)fractal.Get(nx, ny, nz, nw) * weights[i++];
                }

                //отслеживаем максимальные и минимальные найденные значения
                if (value > max) max = value;
                if (value < min) min = value;

                value = (value - min) / (max - min);
                noiseMap[x, y] = value;

                value = 0;
                i = 0;
            }
        }
    }

    //private float[,] GenerateNoiseMap() {
    //    // Create new noise map
    //    float[,] noiseMap = new float[mapWidth, mapHeight];

    //    // Generate noise values for each pixel
    //    for (int y = 0; y < mapHeight; y++) {
    //        for (int x = 0; x < mapWidth; x++) {
    //            float amplitude = 1f;
    //            float frequency = 1f;
    //            float noiseHeight = 0f;

    //            for (int i = 0; i < octaves; i++) {
    //                float sampleX = (float)x / scale * frequency + seed;
    //                float sampleY = (float)y / scale * frequency + seed;

    //                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
    //                noiseHeight += perlinValue * amplitude;

    //                amplitude *= persistence;
    //                frequency *= lacunarity;
    //            }

    //            noiseMap[x, y] = noiseHeight;
    //        }
    //    }

    //    return noiseMap;
    //}
    private void spreadBiome(int x, int y, biome biomeToSpread, List<int> terrainToChange, int chance, int strength) {
        if (x < 0 || y < 0) return;
        if (x >= mapWidth || y >= mapHeight) return;
        if (gameMap[x, y].biomeType == biomeToSpread) return;

        int val = (int)gameMap[x, y].terrainType;
        if (terrainToChange.Contains(val) == true)
            gameMap[x, y].biomeType = biomeToSpread;
        else return;
        if (Random.Range(0, 100) < chance) {
            spreadBiome(x + 1, y, biomeToSpread, terrainToChange, chance - strength, strength);
            spreadBiome(x, y + 1, biomeToSpread, terrainToChange, chance - strength, strength);
            spreadBiome(x - 1, y, biomeToSpread, terrainToChange, chance - strength, strength);
            spreadBiome(x, y - 1, biomeToSpread, terrainToChange, chance - strength, strength);
        }
    }
    private void spreadTerrain(int x, int y, terrain terrainToSpread, List<int> terrainToChange, int chance, int strength, int side, bool check)
    {
        if (x < 0 || y < 0) return;
        if (x >= mapWidth || y >= mapHeight) return;
        if (check == true)
            if (gameMap[x, y].terrainType == terrainToSpread) return;

        int val = (int)gameMap[x, y].terrainType;
        if (terrainToChange.Contains(val) == true)
            gameMap[x, y].terrainType = terrainToSpread;
        //else return;
        int sideStrength = Mathf.FloorToInt(strength * 1.1f);
        if (Random.Range(0, 100) < chance)
        {
            //all
            if (side == -1)
            {
                spreadTerrain(x + 1, y, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
                spreadTerrain(x - 1, y, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
                spreadTerrain(x, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
                spreadTerrain(x, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
            }
            //up
            if (side == 0)
            {
                spreadTerrain(x, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x, y + 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //up-right
            else if (side == 1)
            {
                spreadTerrain(x + 1, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x + 1, y + 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //right
            else if (side == 2)
            {
                spreadTerrain(x + 1, y, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x + 1, y, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //right-down
            else if (side == 3)
            {
                spreadTerrain(x + 1, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x + 1, y - 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //down
            else if (side == 4)
            {
                spreadTerrain(x, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x, y - 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //down-left
            else if (side == 5)
            {
                spreadTerrain(x - 1, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x - 1, y - 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //left
            else if (side == 6)
            {
                spreadTerrain(x - 1, y, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x - 1, y, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //left-up
            else if (side == 7)
            {
                spreadTerrain(x - 1, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x - 1, y + 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
        }
    }
    private void spreadMountainChain(int x, int y, int side, int length, int offset)
    {
        if (length == 0) return;
        if (x < 0 || y < 0) return;
        if (x >= mapWidth || y >= mapHeight) return;

        if (gameMap[x, y].terrainType == terrain.Mountain)
            gameMap[x, y].terrainType = terrain.Plain;

        int newSide = Random.Range(-1, 2) + side;
        if (newSide < 0) newSide += 8;
        int oppositeSide = side - 4;
        if (oppositeSide < 0) oppositeSide += 8;

        if (gameMap[x, y].terrainType == terrain.Plain)
        {
            spreadTerrain(x, y, terrain.Mountain, new List<int>() { (int)terrain.Plain }, 160, 30, oppositeSide, true);
        }
        else return;

        //up
        if (newSide == 0)
        {
            spreadMountainChain(x, y + offset, newSide, length - 1, offset);
        }
        //up-right
        else if (newSide == 1)
        {
            spreadMountainChain(x + offset, y + offset, newSide, length - 1, offset);
        }
        //right
        else if (newSide == 2)
        {
            spreadMountainChain(x + offset, y, newSide, length - 1, offset);
        }
        //right-down
        else if (newSide == 3)
        {
            spreadMountainChain(x + offset, y - offset, newSide, length - 1, offset);
        }
        //down
        else if (newSide == 4)
        {
            spreadMountainChain(x, y - offset, newSide, length - 1, offset);
        }
        //down-left
        else if (newSide == 5)
        {
            spreadMountainChain(x - offset, y - offset, newSide, length - 1, offset);
        }
        //left
        else if (newSide == 6)
        {
            spreadMountainChain(x - offset, y, newSide, length - 1, offset);
        }
        //left-up
        else if (newSide == 7)
        {
            spreadMountainChain(x - offset, y + offset, newSide, length - 1, offset);
        }

    }
    public void paint_terrain() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                    SetTileColour(gameMap[x, y].tile, gameColors.terrainColors.DeepOcean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Ocean)
                    SetTileColour(gameMap[x, y].tile, gameColors.terrainColors.Ocean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Beach)
                    SetTileColour(gameMap[x, y].tile, gameColors.terrainColors.Beach, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Plain)
                    SetTileColour(gameMap[x, y].tile, gameColors.terrainColors.Plain, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Hills)
                    SetTileColour(gameMap[x, y].tile, gameColors.terrainColors.Hills, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Mountain)
                    SetTileColour(gameMap[x, y].tile, gameColors.terrainColors.Mountain, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.MountainTop)
                    SetTileColour(gameMap[x, y].tile, gameColors.terrainColors.MountainTop, new Vector3Int(x, y, 0));
            }
        }
    }   
    public void paint_ClimateZones()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (gameMap[x, y].climateType == climate.Polar)
                    SetTileColour(gameMap[x, y].tile, gameColors.climateColors.Polar, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Cold)
                    SetTileColour(gameMap[x, y].tile, gameColors.climateColors.Cold, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Temperate)
                    SetTileColour(gameMap[x, y].tile, gameColors.climateColors.Temperate, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Subtropical)
                    SetTileColour(gameMap[x, y].tile, gameColors.climateColors.Subtropical, new Vector3Int(x, y, 0));
                else
                    SetTileColour(gameMap[x, y].tile, gameColors.climateColors.Tropical, new Vector3Int(x, y, 0));
            }
        }
    }
    public void paint_moisture()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (gameMap[x, y].moistureType == moisture.Highest)
                    SetTileColour(gameMap[x, y].tile, gameColors.MoistureColors.Highest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].moistureType == moisture.High)
                    SetTileColour(gameMap[x, y].tile, gameColors.MoistureColors.High, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].moistureType == moisture.Medium)
                    SetTileColour(gameMap[x, y].tile, gameColors.MoistureColors.Medium, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].moistureType == moisture.Low)
                    SetTileColour(gameMap[x, y].tile, gameColors.MoistureColors.Low, new Vector3Int(x, y, 0));
                else
                    SetTileColour(gameMap[x, y].tile, gameColors.MoistureColors.Lowest, new Vector3Int(x, y, 0));
            }
        }
    }
    public void paint_biomes()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (gameMap[x, y].biomeType == biome.Ice)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.Ice, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.SnowWasteland)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.SnowWasteland, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Tundra)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.Tundra, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.BorealForest)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.BorealForest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Grassland)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.Grassland, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.TemperateForest)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.TemperateForest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.SeasonalForest)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.SeasonalForest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Savanna)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.Savanna, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Desert)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.Desert, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Rainforest)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.Rainforest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.WarmOcean)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.WarmOcean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.TemperateOcean)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.TemperateOcean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.ColdOcean)
                    SetTileColour(gameMap[x, y].tile, gameColors.BiomeColors.ColdOcean, new Vector3Int(x, y, 0));
            }
        }
    }
    public void paint_ready() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                
            }
        }
    }
}
