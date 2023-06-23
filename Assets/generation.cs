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
}
enum biome {
    None,
    DeepOcean,
    Ocean,
    Beach,
    Desert,
    Desert_hills,
    Grassland,
    Grassland_hills,
    Forest,
    Mountains,
    Corruption,
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
class gameTile {
    public terrain terrainType { get; set; }
    public biome biomeType { get; set; }
    public climate climateType { get; set; }
    public Sprite sprite { get; set; }

    public gameTile() {
        terrainType = terrain.None;
        biomeType = biome.None;
        climateType = climate.None;
    }
}

public class generation : MonoBehaviour {
    private static Color DeepColor = new Color(68 / 255f, 139 / 255f, 237 / 255f, 1);
    private static Color ShallowColor = new Color(93 / 255f, 180 / 255f, 246 / 255f, 1);
    private static Color SandColor = new Color(232 / 255f, 228 / 255f, 167 / 255f, 1);
    private static Color GrassColor = new Color(129 / 255f, 179 / 255f, 96 / 255f, 1);
    private static Color HillsColor = new Color(102 / 255f, 142 / 255f, 87 / 255f, 1);
    private static Color MountainColor = new Color(99 / 255f, 110 / 255f, 114 / 255f, 1);
    private static Color SnowColor = new Color(1, 1, 1, 1);
    private gameTile[,] gameMap;

    Tilemap colorMap = null;
    public float[,] noiseMap = null;

    [Header("Generation settings")]
    [SerializeField]
    int mapHeight = 128;
    [SerializeField]
    int mapWidth = 128;
    [SerializeField]
    int TerrainOctaves = 6;
    [SerializeField]
    double TerrainFrequency = 1.25;
    [SerializeField]
    int MountainFrequency = 50;
    [SerializeField]
    Sprite square = null;

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

    int tempOctaves = 4;
    double tempFrequency = 3.25;

    ImplicitFractal terrainFractal = null;
    ImplicitFractal tempFractal = null;
    ImplicitGradient tempGradient = null;

    // Start is called before the first frame update
    void Start() {
        Random.InitState(Time.frameCount);
        Initialize();
        generationV1();
        //generationV2();
    }

    private void generationV1()
    {
        colorMap = GetComponent<Tilemap>();
        gameMap = new gameTile[mapWidth, mapHeight];

        //Terrain types
        terrainFractal.Seed = Random.Range(0, int.MaxValue);
        generateTerrainNoiseMap();
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMap[x, y] = new gameTile();
                if (noiseMap[x, y] > 0.9f)
                {
                    gameMap[x, y].terrainType = terrain.Mountain;
                }
                else if (noiseMap[x, y] > 0.75f)
                {
                    gameMap[x, y].terrainType = terrain.Hills;
                }
                else if (noiseMap[x, y] > 0.6f)
                {
                    gameMap[x, y].terrainType = terrain.Plain;
                }
                else if (noiseMap[x, y] > 0.5f)
                {
                    gameMap[x, y].terrainType = terrain.Beach;
                }
                else if (noiseMap[x, y] > 0.45f)
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
        generateTempNoiseMap();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (noiseMap[x, y] > 0.95f)
                {
                    gameMap[x, y].climateType = climate.Tropcial;
                }
                else if (noiseMap[x, y] > 0.85f)
                {
                    gameMap[x, y].climateType = climate.Subtropical;
                }
                else if (noiseMap[x, y] > 0.65f)
                {
                    gameMap[x, y].climateType = climate.Temperate;
                }
                else if (noiseMap[x, y] > 0.48f)
                {
                    gameMap[x, y].climateType = climate.Cold;
                }
                else
                {
                    gameMap[x, y].climateType = climate.Polar;
                }
            }
        }

        //Biomes
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                {
                    gameMap[x, y].biomeType = biome.DeepOcean;
                }
                else if (gameMap[x, y].terrainType == terrain.Ocean)
                {
                    gameMap[x, y].biomeType = biome.Ocean;
                }
                else if (gameMap[x, y].terrainType == terrain.Beach)
                {
                    gameMap[x, y].biomeType = biome.Beach;
                }
                else if (gameMap[x, y].terrainType == terrain.Plain)
                {
                    gameMap[x, y].biomeType = biome.Grassland;
                }
                else if (gameMap[x, y].terrainType == terrain.Hills)
                {
                    gameMap[x, y].biomeType = biome.Grassland_hills;
                }
                else if (gameMap[x, y].terrainType == terrain.Mountain)
                {
                    gameMap[x, y].biomeType = biome.Mountains;
                }
            }
        }

        paint_ClimateZones();
    }
    private void generationV2()
    {
        colorMap = GetComponent<Tilemap>();
        generateTerrainNoiseMap();
        gameMap = new gameTile[mapWidth, mapHeight];

        //Terrain types
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMap[x, y] = new gameTile();
                if (noiseMap[x, y] > 0.4f)
                {
                    gameMap[x, y].terrainType = terrain.Plain;
                }
                else if (noiseMap[x, y] > 0.3f)
                {
                    gameMap[x, y].terrainType = terrain.Beach;
                }
                else if (noiseMap[x, y] > 0.1f)
                {
                    gameMap[x, y].terrainType = terrain.Ocean;
                }
                else
                {
                    gameMap[x, y].terrainType = terrain.DeepOcean;
                }
            }
        }

        for (int i = 0; i < MountainFrequency; i ++)
        {
            spreadMountainChain(Random.Range(0, mapWidth), 
                                Random.Range(0, mapHeight),
                                Random.Range(0, 8),
                                Random.Range(0, 10),
                                Random.Range(0, 4));
        }

        //Biomes
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                {
                    gameMap[x, y].biomeType = biome.DeepOcean;
                }
                else if (gameMap[x, y].terrainType == terrain.Ocean)
                {
                    gameMap[x, y].biomeType = biome.Ocean;
                }
                else if (gameMap[x, y].terrainType == terrain.Beach)
                {
                    gameMap[x, y].biomeType = biome.Beach;
                }
                else if (gameMap[x, y].terrainType == terrain.Plain)
                {
                    gameMap[x, y].biomeType = biome.Grassland;
                }
                else if (gameMap[x, y].terrainType == terrain.Hills)
                {
                    gameMap[x, y].biomeType = biome.Grassland_hills;
                }
                else if (gameMap[x, y].terrainType == terrain.Mountain)
                {
                    gameMap[x, y].biomeType = biome.Mountains;
                }
            }
        }

        //paint_terrains();
        paint_ready();
    }
    private void SetTileColour(Color colour, Vector3Int position) {
        Tile tile = new Tile();
        tile.sprite = square;
        tile.color = colour;
        colorMap.SetTile(position, tile);
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.E))
        {
            generationV1();
        }
    }
    private void Initialize() {

        //terrain
        terrainFractal = new ImplicitFractal (FractalType.Billow,
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
    }

    private void generateTerrainNoiseMap() {
        float max = 0;
        float min = 0;
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

                float value = (float)terrainFractal.Get(nx, ny, nz, nw);

                //отслеживаем максимальные и минимальные найденные значения
                if (value > max) max = value;
                if (value < min) min = value;

                value = (value - min) / (max - min);
                noiseMap[x, y] = value;
                
            }
        }
    }

    private void generateTempNoiseMap()
    {
        float max = 0;
        float min = 0;
        noiseMap = new float[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
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

                float value = (float)tempGradient.Get(nx, ny, nz, nw) * 0.85f + (float)tempFractal.Get(nx, ny, nz, nw) * 0.15f;

                //отслеживаем максимальные и минимальные найденные значения
                if (value > max) max = value;
                if (value < min) min = value;

                value = (value - min) / (max - min);
                noiseMap[x, y] = value;
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
    public void paint_terrains() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                    SetTileColour(DeepColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Ocean)
                    SetTileColour(ShallowColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Beach)
                    SetTileColour(SandColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Plain)
                    SetTileColour(GrassColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Hills)
                    SetTileColour(HillsColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Mountain)
                    SetTileColour(MountainColor, new Vector3Int(x, y, 0));
            }
        }
    }   

    public void paint_biomes() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].biomeType == biome.DeepOcean)
                    SetTileColour(DeepColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Beach)
                    SetTileColour(SandColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Desert)
                    SetTileColour(SandColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Desert_hills)
                    SetTileColour(SandColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Grassland)
                    SetTileColour(GrassColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Grassland_hills)
                    SetTileColour(GrassColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Forest)
                    SetTileColour(GrassColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Mountains)
                    SetTileColour(MountainColor, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Corruption)
                    SetTileColour(MountainColor, new Vector3Int(x, y, 0));
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
                    SetTileColour(new Color(170/255f, 1, 1, 1), new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Cold)
                    SetTileColour(new Color(0, 229 / 255f, 133 / 255f, 1), new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Temperate)
                    SetTileColour(new Color(1, 1, 100 / 255f, 1), new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Subtropical)
                    SetTileColour(new Color(1, 100 / 255f, 0, 1), new Vector3Int(x, y, 0));
                else
                    SetTileColour(new Color(241 / 255f, 12 / 255f, 0, 1), new Vector3Int(x, y, 0));
            }
        }
    }
    public void paint_ready() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].terrainType == terrain.DeepOcean) {
                    if (gameMap[x, y].biomeType != biome.Corruption)
                        SetTileColour(DeepColor, new Vector3Int(x, y, 0));
                    else
                        SetTileColour(new Color(0.024f, 0.21f, 0.26f, 1), new Vector3Int(x, y, 0));
                }
                else if (gameMap[x, y].terrainType == terrain.Ocean) {
                    if (gameMap[x, y].biomeType != biome.Corruption)
                        SetTileColour(ShallowColor, new Vector3Int(x, y, 0));
                    else
                        SetTileColour(new Color(0.47f, 0.35f, 0.56f, 1), new Vector3Int(x, y, 0));

                }
                else if (gameMap[x, y].terrainType == terrain.Beach)
                {
                    if (gameMap[x, y].biomeType != biome.Corruption)
                        SetTileColour(SandColor, new Vector3Int(x, y, 0));
                    else
                        SetTileColour(new Color(0.47f, 0.35f, 0.56f, 1), new Vector3Int(x, y, 0));

                }
                else if (gameMap[x, y].terrainType == terrain.Plain) {
                    if (gameMap[x, y].biomeType == biome.Corruption)
                        SetTileColour(new Color(0.41f, 0.10f, 0.63f, 1), new Vector3Int(x, y, 0));
                    else
                    if (gameMap[x, y].biomeType == biome.Grassland) {
                        SetTileColour(GrassColor, new Vector3Int(x, y, 0));
                    }
                }
                else if (gameMap[x, y].terrainType == terrain.Hills) {
                    if (gameMap[x, y].biomeType == biome.Corruption)
                        SetTileColour(new Color(0.32f, 0.06f, 0.5f, 1), new Vector3Int(x, y, 0));
                    else
                    if (gameMap[x, y].biomeType == biome.Grassland_hills) {
                        SetTileColour(HillsColor, new Vector3Int(x, y, 0));
                    }
                }
                else if (gameMap[x, y].terrainType == terrain.Mountain) {
                    if (gameMap[x, y].biomeType == biome.Corruption)
                        SetTileColour(new Color(0.14f, 0.03f, 0.22f, 1), new Vector3Int(x, y, 0));
                    else
                        SetTileColour(MountainColor, new Vector3Int(x, y, 0));
                }
            }
        }
    }
}
