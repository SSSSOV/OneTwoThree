using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.Sprites;
using AccidentalNoise;
using UnityEngine.Rendering;

enum terrain
{
    None,
    DeepOcean,
    ShallowWater,
    Plain,
    Hills,
    Mountain,
}

enum biome
{
    None,
    DeepOcean,
    Beach,
    Desert,
    Desert_hills,
    Grassland,
    Grassland_hills,
    Forest,
    Mountains,
    Corruption,
}
class gameTile
{
    public terrain terrainType { get; set; }
    public biome biomeType { get; set; }
    public Sprite sprite { get; set; }

    public gameTile()
    {
        //terrainType = new terrain();
        terrainType = terrain.None;
        //biomeType = new biome();
        biomeType = biome.None;
    }
}

public class generation : MonoBehaviour
{
    private static Color DeepColor = new Color(68 / 255f, 139  / 255f, 237  / 255f, 1);
    private static Color ShallowColor = new Color(93  / 255f, 180  / 255f, 246  / 255f, 1);
    private static Color SandColor = new Color(232  / 255f, 228  / 255f, 167  / 255f, 1);
    private static Color GrassColor = new Color(129  / 255f, 179  / 255f, 96  / 255f, 1);
    private static Color HillsColor = new Color(102 / 255f, 142 / 255f, 87 / 255f, 1);
    private static Color MountainColor = new Color(99 / 255f, 110  / 255f, 114  / 255f, 1);
    private static Color SnowColor = new Color(1, 1, 1, 1);
    private gameTile[,] gameMap;

    Tilemap colorMap = null;
    public float[,] noiseMap= null;
    [SerializeField]
    int mapHeight = 10;
    [SerializeField]
    int mapWidth = 10;
    [SerializeField]
    int TerrainOctaves = 6;
    [SerializeField]
    double TerrainFrequency = 1.25;
    [SerializeField]
    Sprite square = null;
    
    Fractal module= null;


    // Start is called before the first frame update
    void Start()
    {
        colorMap = GetComponent<Tilemap>();
        Initialize();
        generateNoiseMap();
        gameMap = new gameTile[mapHeight, mapWidth];

        //Terrain types
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMap[x, y] = new gameTile();
                if (noiseMap[x, y] > 0.8f)
                {
                    gameMap[x, y].terrainType = terrain.Mountain;
                }
                else if (noiseMap[x, y] > 0.6f)
                {
                    gameMap[x, y].terrainType = terrain.Hills;
                }
                else if (noiseMap[x, y] > 0.4f)
                {
                    gameMap[x, y].terrainType = terrain.Plain;
                }
                else if (noiseMap[x, y] > 0.1f)
                {
                    gameMap[x, y].terrainType = terrain.ShallowWater;
                }
                else
                {
                    gameMap[x, y].terrainType = terrain.DeepOcean;
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
                if (gameMap[x, y].terrainType == terrain.ShallowWater)
                {
                    gameMap[x, y].biomeType = biome.Beach;
                    spreadBiome(x + 1, y, biome.Beach, new List<int>() { (int)terrain.Plain }, 120, 20);
                    spreadBiome(x - 1, y, biome.Beach, new List<int>() { (int)terrain.Plain }, 120, 20);
                    spreadBiome(x, y + 1, biome.Beach, new List<int>() { (int)terrain.Plain }, 120, 20);
                    spreadBiome(x, y - 1, biome.Beach, new List<int>() { (int)terrain.Plain }, 120, 20);
                }
                if (gameMap[x, y].terrainType == terrain.Plain)
                {
                    gameMap[x, y].biomeType = biome.Grassland;
                }
                if (gameMap[x, y].terrainType == terrain.Hills)
                {
                    gameMap[x, y].biomeType = biome.Grassland_hills;
                }
                if (gameMap[x, y].terrainType == terrain.Mountain)
                {
                    gameMap[x, y].biomeType = biome.Mountains;
                }

            }
        }

        spreadBiome(Random.Range(0, mapWidth),
                    Random.Range(0, mapHeight),
                    biome.Corruption, new List<int>() { (int)terrain.DeepOcean,
                                                        (int)terrain.ShallowWater,
                                                        (int)terrain.Plain,
                                                        (int)terrain.Hills,
                                                        (int)terrain.Mountain},
                    600,
                    5);


        //Colour for tilemap
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                {
                    if(gameMap[x, y].biomeType != biome.Corruption)
                        SetTileColour(DeepColor, new Vector3Int(x, y, 0));
                    else
                        SetTileColour(new Color(0.024f, 0.21f, 0.26f, 1), new Vector3Int(x, y, 0));
                }
                if (gameMap[x, y].terrainType == terrain.ShallowWater)
                {
                    if (gameMap[x, y].biomeType != biome.Corruption)
                        SetTileColour(ShallowColor, new Vector3Int(x, y, 0));
                    else
                        SetTileColour(new Color(0.47f, 0.35f, 0.56f, 1), new Vector3Int(x, y, 0));
                    
                }
                if (gameMap[x, y].terrainType == terrain.Plain)
                {
                    if (gameMap[x, y].biomeType == biome.Corruption)
                        SetTileColour(new Color(0.41f, 0.10f, 0.63f, 1), new Vector3Int(x, y, 0));
                    else
                    if (gameMap[x, y].biomeType == biome.Beach)
                    {
                        SetTileColour(SandColor, new Vector3Int(x, y, 0));
                    }
                    if (gameMap[x, y].biomeType == biome.Grassland)
                    {
                        SetTileColour(GrassColor, new Vector3Int(x, y, 0));
                    }
                }
                if (gameMap[x, y].terrainType == terrain.Hills)
                {
                    if (gameMap[x, y].biomeType == biome.Corruption)
                        SetTileColour(new Color(0.32f, 0.06f, 0.5f, 1), new Vector3Int(x, y, 0));
                    else
                    if (gameMap[x, y].biomeType == biome.Grassland_hills)
                    {
                        SetTileColour(HillsColor, new Vector3Int(x, y, 0));
                    }
                }
                if (gameMap[x, y].terrainType == terrain.Mountain)
                {
                    if (gameMap[x, y].biomeType == biome.Corruption)
                        SetTileColour(new Color(0.14f, 0.03f, 0.22f, 1), new Vector3Int(x, y, 0));
                    else
                        SetTileColour(MountainColor, new Vector3Int(x, y, 0));
                }
            }
        }
    }
    private void SetTileColour(Color colour, Vector3Int position)
    {
        Tile tile = new Tile();
        tile.sprite = square;
        tile.color = colour;
        colorMap.SetTile(position, tile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialize()
    {
        module = new Fractal(FractalType.MULTI,
                             BasisTypes.SIMPLEX,
                             InterpTypes.QUINTIC,
                             TerrainOctaves,
                             TerrainFrequency,
                             (uint)Random.Range(0, int.MaxValue));
    }

    private void generateNoiseMap()
    {
        float max = 0;
        float min = 0;
        noiseMap = new float[mapHeight, mapWidth];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                //Сэмплируем шум с небольшими интервалами
                float x1 = x / (float)mapWidth;
                float y1 = y / (float)mapHeight;

                float value = (float)module.Get(x1, y1);

                //отслеживаем максимальные и минимальные найденные значения
                if (value > max) max = value;
                if (value < min) min = value;

                value = (value - min) / (max - min);
                noiseMap[x, y] = value;
            }
        }
    }

    private void spreadBiome(int x, int y, biome biomeToSpread, List<int> terrainToChange, int chance, int strength)
    {
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
}
