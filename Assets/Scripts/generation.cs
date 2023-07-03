using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TinkerWorX.AccidentalNoiseLibrary;
using Unity.Collections.LowLevel.Unsafe;


static class gameColors {
    public static class terrainColors {
        public static Color DeepOcean = new Color(68 / 255f, 139 / 255f, 237 / 255f, 1);
        public static Color Ocean = new Color(93 / 255f, 180 / 255f, 246 / 255f, 1);
        public static Color Beach = new Color(232 / 255f, 228 / 255f, 167 / 255f, 1);
        public static Color Plain = new Color(129 / 255f, 179 / 255f, 96 / 255f, 1);
        public static Color Hills = new Color(102 / 255f, 142 / 255f, 87 / 255f, 1);
        public static Color Mountain = new Color(99 / 255f, 110 / 255f, 114 / 255f, 1);
        public static Color MountainTop = new Color(1, 1, 1, 1);
    }

    public static class climateColors {
        public static Color Polar = new Color(170 / 255f, 1, 1, 1);
        public static Color Cold = new Color(0, 229 / 255f, 133 / 255f, 1);
        public static Color Temperate = new Color(1, 1, 100 / 255f, 1);
        public static Color Subtropical = new Color(1, 100 / 255f, 0, 1);
        public static Color Tropical = new Color(241 / 255f, 12 / 255f, 0, 1);
    }

    public static class MoistureColors {
        public static Color Highest = new Color(20 / 255f, 70 / 255f, 255 / 255f, 1);
        public static Color High = new Color(85 / 255f, 255 / 255f, 255 / 255f, 1);
        public static Color Medium = new Color(80 / 255f, 255 / 255f, 0 / 255f, 1);
        public static Color Low = new Color(245 / 255f, 245 / 255f, 23 / 255f, 1);
        public static Color Lowest = new Color(255 / 255f, 139 / 255f, 17 / 255f, 1);
    }

    public static class BiomeColors {
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

public class generation : MonoBehaviour {
    private gameTile[,] gameMap;

    Tilemap colorMap = null;
    public float[,] noiseMap = null;

    [Header("Generation settings")]
    [SerializeField]
    GameObject MapObject;

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
    //[SerializeField]
    //Sprite square = null;

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
    float TemperateValue = 0.53f;
    [SerializeField]
    float ColdValue = 0.463f;

    [Header("Moisture settings")]
    [SerializeField]
    float RiverSpreadValue = 0.6f;
    [SerializeField]
    float HighestValue = 0.44f;
    [SerializeField]
    float HighValue = 0.34f;
    [SerializeField]
    float MediumValue = 0.23f;
    [SerializeField]
    float LowValue = 0.13f;

    [Header("River generation")]
    [SerializeField]
    float spreadValue = 400f;
    [SerializeField]
    int RiverCount = 10;
    [SerializeField]
    float MinRiverHeight = 0.3f;
    [SerializeField]
    int MaxRiverAttempts = 25;
    [SerializeField]
    int MinRiverTurns = 5;
    [SerializeField]
    int MinRiverLength = 20;
    [SerializeField]
    int MaxRiverIntersections = 2;

    [Header("New noise map generation settings")]
    [SerializeField]
    public float scale = 20f;
    [SerializeField]
    public int seed = 0;
    [SerializeField]
    public int octaves = 6;
    [SerializeField]
    public float persistence = 0.5f;
    [SerializeField]
    public float lacunarity = 2f;

    [SerializeField]
    UnityEngine.UI.Text textbox;

    Texture2D MapTexture;
    ImplicitFractal terrainFractal = null;
    ImplicitFractal tempFractal = null;
    ImplicitFractal moistureFractal = null;
    ImplicitGradient tempGradient = null;

    List<gameTile> Waters = new List<gameTile>();
    List<gameTile> Lands = new List<gameTile>();

    List<River> Rivers = new List<River>();
    List<RiverGroup> RiverGroups = new List<RiverGroup>();

    // Start is called before the first frame update
    void Start() {
        Random.InitState(Time.frameCount);
        
        colorMap = GetComponent<Tilemap>();
        Initialize();
        generateMap();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            generateMap();
        }
        if (Input.GetMouseButtonDown(1))
        {
            string text = "";
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int x = Mathf.FloorToInt(pos.x * 2);
            int y = Mathf.FloorToInt(pos.y * 2);
            text += Mathf.FloorToInt(gameMap[x, y].terrainValue * 1000) / 1000f;
            text += "; ";
            text += Mathf.FloorToInt(gameMap[x, y].climateValue * 1000) / 1000f;
            text += "; ";
            text += Mathf.FloorToInt(gameMap[x, y].moistureValue * 1000) / 1000f;
            textbox.text = text;
        }
    }
    private void generateMap() {

        terrainFractal.Seed = seed;
        MapTexture = new Texture2D(mapWidth, mapHeight);
        terrainFractal.Octaves = TerrainOctaves;
        terrainFractal.Frequency = TerrainFrequency;

        //Map init
        gameMap = new gameTile[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMap[x, y] = new gameTile();
                gameMap[x, y].x = x;
                gameMap[x, y].y = y;
            }
        }
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                gameMap[x, y].Top = gameMap[x, (y + 1) % mapHeight];
                gameMap[x, y].Bottom = gameMap[x, Mathf.Abs((y - 1) % mapHeight)];
                gameMap[x, y].Right = gameMap[(x + 1) % mapWidth, y];
                gameMap[x, y].Left = gameMap[Mathf.Abs((x - 1) % mapWidth), y];
            }
        }

        //Terrain types
        terrainFractal.Seed = Random.Range(0, int.MaxValue);
        generateNoiseMap(new float[] { 1 }, terrainFractal);
        //noiseMap = GenerateNoiseMapV2();
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (noiseMap[x, y] > MountainTopValue) {
                    gameMap[x, y].terrainType = terrain.MountainTop;
                }
                else if (noiseMap[x, y] > MountainValue) {
                    gameMap[x, y].terrainType = terrain.Mountain;
                }
                else if (noiseMap[x, y] > HillsValue) {
                    gameMap[x, y].terrainType = terrain.Hills;
                }
                else if (noiseMap[x, y] > PlainsValue) {
                    gameMap[x, y].terrainType = terrain.Plain;
                }
                else if (noiseMap[x, y] > BeachValue) {
                    gameMap[x, y].terrainType = terrain.Beach;
                }
                else if (noiseMap[x, y] > OceanValue) {
                    gameMap[x, y].terrainType = terrain.Ocean;
                    gameMap[x, y].Collidable = false;
                }
                else {
                    gameMap[x, y].terrainType = terrain.DeepOcean;
                    gameMap[x, y].Collidable = false;
                }
                gameMap[x, y].terrainValue = noiseMap[x, y];
            }
        }

        generateRivers();
        BuildRiverGroups();
        DigRiverGroups();

        //Climate Zones
        tempFractal.Seed = Random.Range(0, int.MaxValue);
        generateNoiseMap(new float[] { 0.85f, 0.15f }, tempGradient, tempFractal);

        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (noiseMap[x, y] > TropicalValue) {
                    gameMap[x, y].climateType = climate.Tropcial;
                }
                else if (noiseMap[x, y] > SubtropicalValue) {
                    gameMap[x, y].climateType = climate.Subtropical;
                }
                else if (noiseMap[x, y] > TemperateValue) {
                    gameMap[x, y].climateType = climate.Temperate;
                }
                else if (noiseMap[x, y] > ColdValue) {
                    gameMap[x, y].climateType = climate.Cold;
                }
                else {
                    gameMap[x, y].climateType = climate.Polar;
                }
                gameMap[x, y].climateValue = noiseMap[x, y];
            }
        }

        //Moisture
        generateNoiseMap(new float[] { 1 }, moistureFractal);
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                //adjust value according to climate
                if (gameMap[x, y].climateType == climate.Temperate)
                {
                    noiseMap[x, y] *= 0.85f;
                }
                else if(gameMap[x, y].climateType == climate.Cold)
                {
                    noiseMap[x, y] *= 0.75f;
                }
                else if (gameMap[x, y].climateType == climate.Polar)
                {
                    noiseMap[x, y] *= 0.45f;
                }
                
                //adjust value according to terrain
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                {
                    if (noiseMap[x, y] < 0.13f)
                        noiseMap[x, y] += 0.37f;

                    if (noiseMap[x, y] < 0.23f)
                        noiseMap[x, y] += 0.25f;

                    if (noiseMap[x, y] < 0.35f)
                        noiseMap[x, y] += 0.17f;

                    if (noiseMap[x, y] < 0.45f)
                        noiseMap[x, y] += 0.11f;
                }
                else if (gameMap[x, y].terrainType == terrain.Ocean || gameMap[x, y].terrainType == terrain.River)
                {
                    if (noiseMap[x, y] < 0.13f)
                        noiseMap[x, y] += 0.25f;

                    if (noiseMap[x, y] < 0.23f)
                        noiseMap[x, y] += 0.16f;

                    if (noiseMap[x, y] < 0.35f)
                        noiseMap[x, y] += 0.05f;
                }

                if (gameMap[x, y].terrainType == terrain.River)
                {
                    spreadMoisture(x, y, 130, 30);
                }

                //Select moisture level
                if (noiseMap[x, y] > HighestValue) {
                    gameMap[x, y].moistureType = moisture.Highest;
                }
                else if (noiseMap[x, y] > HighValue) {
                    gameMap[x, y].moistureType = moisture.High;
                }
                else if (noiseMap[x, y] > MediumValue) {
                    gameMap[x, y].moistureType = moisture.Medium;
                }
                else if (noiseMap[x, y] > LowValue) {
                    gameMap[x, y].moistureType = moisture.Low;
                }
                else {
                    gameMap[x, y].moistureType = moisture.Lowest;
                }
                gameMap[x, y].moistureValue = noiseMap[x, y];
            }
        }

        //Biomes
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                //DeepOcean
                if (gameMap[x, y].terrainType == terrain.DeepOcean) {
                    if (gameMap[x, y].climateType == climate.Tropcial ||
                        gameMap[x, y].climateType == climate.Subtropical) {
                        gameMap[x, y].biomeType = biome.WarmOcean;
                    }
                    else if (gameMap[x, y].climateType == climate.Temperate) {
                        gameMap[x, y].biomeType = biome.TemperateOcean;
                    }
                    else gameMap[x, y].biomeType = biome.ColdOcean;
                }

                //Ocean
                else if (gameMap[x, y].terrainType == terrain.Ocean) {
                    if (gameMap[x, y].climateType == climate.Tropcial ||
                        gameMap[x, y].climateType == climate.Subtropical) {
                        gameMap[x, y].biomeType = biome.WarmOcean;
                    }
                    else if (gameMap[x, y].climateType == climate.Temperate) {
                        gameMap[x, y].biomeType = biome.TemperateOcean;
                    }
                    else gameMap[x, y].biomeType = biome.ColdOcean;
                }

                //River
                else if (gameMap[x, y].terrainType == terrain.River)
                {
                    gameMap[x, y].biomeType = biome.River;
                }

                //Plains, Hills, Mountains
                else {
                    //Polar
                    if (gameMap[x, y].climateType == climate.Polar) {
                        if (gameMap[x, y].moistureType == moisture.Lowest ||
                            gameMap[x, y].moistureType == moisture.Low) {
                            gameMap[x, y].biomeType = biome.SnowWasteland;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Medium) {
                            gameMap[x, y].biomeType = biome.Tundra;
                        }
                        else {
                            gameMap[x, y].biomeType = biome.Ice;
                        }
                    }

                    //Cold
                    else if (gameMap[x, y].climateType == climate.Cold) {
                        if (gameMap[x, y].moistureType == moisture.Lowest) {
                            gameMap[x, y].biomeType = biome.SnowWasteland;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Low ||
                            gameMap[x, y].moistureType == moisture.Medium) {
                            gameMap[x, y].biomeType = biome.Tundra;
                        }
                        else {
                            gameMap[x, y].biomeType = biome.BorealForest;
                        }
                    }

                    //Temperate
                    else if (gameMap[x, y].climateType == climate.Temperate) {
                        if (gameMap[x, y].moistureType == moisture.Lowest) {
                            gameMap[x, y].biomeType = biome.Savanna;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Low) {
                            gameMap[x, y].biomeType = biome.Grassland;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Medium) {
                            gameMap[x, y].biomeType = biome.TemperateForest;
                        }
                        else {
                            gameMap[x, y].biomeType = biome.SeasonalForest;
                        }
                    }

                    //Subtropical
                    else if (gameMap[x, y].climateType == climate.Subtropical) {
                        if (gameMap[x, y].moistureType == moisture.Lowest) {
                            gameMap[x, y].biomeType = biome.Desert;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Low ||
                            gameMap[x, y].moistureType == moisture.Medium) {
                            gameMap[x, y].biomeType = biome.Savanna;
                        }
                        else {
                            gameMap[x, y].biomeType = biome.Rainforest;
                        }
                    }

                    //Tropical
                    else if (gameMap[x, y].climateType == climate.Tropcial) {
                        if (gameMap[x, y].moistureType == moisture.Lowest ||
                            gameMap[x, y].moistureType == moisture.Low) {
                            gameMap[x, y].biomeType = biome.Desert;
                        }
                        else if (gameMap[x, y].moistureType == moisture.Medium) {
                            gameMap[x, y].biomeType = biome.Savanna;
                        }
                        else {
                            gameMap[x, y].biomeType = biome.Rainforest;
                        }
                    }
                }
            }
        }

        paint_terrain();
        MapTexture.filterMode = FilterMode.Point;
        MapObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(MapTexture, new Rect(0, 0, mapWidth, mapHeight), Vector2.zero);
        MapObject.transform.localScale = new Vector2(50, 50);
    }
    private void SetTileColour(Color colour, Vector3Int position) {
        MapTexture.SetPixel(position.x, position.y, colour);
    }
    private void Initialize() {

        //terrain
        terrainFractal = new ImplicitFractal(FractalType.Multi,
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

                foreach (ImplicitModuleBase fractal in fractals) {
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
    private float[,] GenerateNoiseMapV2() {
        // Create new noise map
        float[,] noiseMap = new float[mapWidth, mapHeight];

        // Generate noise values for each pixel
        for (int y = 0; y < mapHeight; y++) {
            for (int x = 0; x < mapWidth; x++) {
                float amplitude = 0.5f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (float)x / scale * frequency + seed;
                    float sampleY = (float)y / scale * frequency + seed;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        return noiseMap;
    }
    private void generateRivers()
    {
        int attempts = 0;
        int rivercount = RiverCount;
        Rivers = new List<River>();

        while (rivercount > 0 && attempts < MaxRiverAttempts)
        {

            int x = UnityEngine.Random.Range(0, mapWidth);
            int y = UnityEngine.Random.Range(0, mapHeight);
            gameTile tile = gameMap[x, y];

            if (tile.Collidable == false) continue;
            if (tile.Rivers.Count > 0) continue;

            if (tile.terrainValue > MinRiverHeight)
            {
                River river = new River(rivercount);

                river.CurrentDirection = tile.GetLowestNeighbor();

                FindPathToWater(tile, river.CurrentDirection, ref river);

                // Check river
                if (river.TurnCount < MinRiverTurns || river.Tiles.Count < MinRiverLength || river.Intersections > MaxRiverIntersections)
                {
                    for (int i = 0; i < river.Tiles.Count; i++)
                    {
                        gameTile t = river.Tiles[i];
                        t.Rivers.Remove(river);
                    }
                }
                else if (river.Tiles.Count >= MinRiverLength)
                {
                    //Проверка пройдена - добавляем реку в список
                    Rivers.Add(river);
                    tile.Rivers.Add(river);
                    rivercount--;
                }
            }
            attempts++;
        }
    }
    private void BuildRiverGroups()
    {
        //loop each tile, checking if it belongs to multiple rivers
        for (var x = 0; x < mapWidth; x++)
        {
            for (var y = 0; y < mapHeight; y++)
            {
                gameTile t = gameMap[x, y];

                if (t.Rivers.Count > 1)
                {
                    // multiple rivers == intersection
                    RiverGroup group = null;

                    // Does a rivergroup already exist for this group?
                    for (int n = 0; n < t.Rivers.Count; n++)
                    {
                        River tileriver = t.Rivers[n];
                        for (int i = 0; i < RiverGroups.Count; i++)
                        {
                            for (int j = 0; j < RiverGroups[i].Rivers.Count; j++)
                            {
                                River river = RiverGroups[i].Rivers[j];
                                if (river.ID == tileriver.ID)
                                {
                                    group = RiverGroups[i];
                                }
                                if (group != null) break;
                            }
                            if (group != null) break;
                        }
                        if (group != null) break;
                    }

                    // existing group found -- add to it
                    if (group != null)
                    {
                        for (int n = 0; n < t.Rivers.Count; n++)
                        {
                            if (!group.Rivers.Contains(t.Rivers[n]))
                                group.Rivers.Add(t.Rivers[n]);
                        }
                    }
                    else   //No existing group found - create a new one
                    {
                        group = new RiverGroup();
                        for (int n = 0; n < t.Rivers.Count; n++)
                        {
                            group.Rivers.Add(t.Rivers[n]);
                        }
                        RiverGroups.Add(group);
                    }
                }
            }
        }
    }
    private void FindPathToWater(gameTile tile, Direction direction, ref River river)
	{
		if (tile.Rivers.Contains (river))
			return;

		// check if there is already a river on this tile
		if (tile.Rivers.Count > 0)
			river.Intersections++;

		river.addTile(tile);

		// get neighbors
		gameTile left = tile.Left;
        gameTile right = tile.Right;
        gameTile top = tile.Top;
        gameTile bottom = tile.Bottom;
		
		float leftValue = int.MaxValue;
		float rightValue = int.MaxValue;
		float topValue = int.MaxValue;
		float bottomValue = int.MaxValue;
		
		// query height values of neighbors
		if (left.getRiverNeighborCount(river) < 2 && !river.Tiles.Contains(left)) 
			leftValue = left.terrainValue;
		if (right.getRiverNeighborCount(river) < 2 && !river.Tiles.Contains(right)) 
			rightValue = right.terrainValue;
		if (top.getRiverNeighborCount(river) < 2 && !river.Tiles.Contains(top)) 
			topValue = top.terrainValue;
		if (bottom.getRiverNeighborCount(river) < 2 && !river.Tiles.Contains(bottom)) 
			bottomValue = bottom.terrainValue;
		
		// if neighbor is existing river that is not this one, flow into it
		if (bottom.Rivers.Count == 0 && !bottom.Collidable)
			bottomValue = 0;
		if (top.Rivers.Count == 0 && !top.Collidable)
			topValue = 0;
		if (left.Rivers.Count == 0 && !left.Collidable)
			leftValue = 0;
		if (right.Rivers.Count == 0 && !right.Collidable)
			rightValue = 0;
		
		// override flow direction if a tile is significantly lower
		if (direction == Direction.Left)
			if (Mathf.Abs (rightValue - leftValue) < 0.1f)
				rightValue = int.MaxValue;
		if (direction == Direction.Right)
			if (Mathf.Abs (rightValue - leftValue) < 0.1f)
				leftValue = int.MaxValue;
		if (direction == Direction.Top)
			if (Mathf.Abs (topValue - bottomValue) < 0.1f)
				bottomValue = int.MaxValue;
		if (direction == Direction.Bottom)
			if (Mathf.Abs (topValue - bottomValue) < 0.1f)
				topValue = int.MaxValue;
		
		// find mininum
		float min = Mathf.Min (Mathf.Min (Mathf.Min (leftValue, rightValue), topValue), bottomValue);
		
		// if no minimum found - exit
		if (min == int.MaxValue)
			return;
		
		//Move to next neighbor
		if (min == leftValue) {
			if (left.Collidable)
			{
				if (river.CurrentDirection != Direction.Left){
					river.TurnCount++;
					river.CurrentDirection = Direction.Left;
				}
				FindPathToWater (left, direction, ref river);
			}
		} else if (min == rightValue) {
			if (right.Collidable)
			{
				if (river.CurrentDirection != Direction.Right){
					river.TurnCount++;
					river.CurrentDirection = Direction.Right;
				}
				FindPathToWater (right, direction, ref river);
			}
		} else if (min == bottomValue) {
			if (bottom.Collidable)
			{
				if (river.CurrentDirection != Direction.Bottom){
					river.TurnCount++;
					river.CurrentDirection = Direction.Bottom;
				}
				FindPathToWater (bottom, direction, ref river);
			}
		} else if (min == topValue) {
			if (top.Collidable)
			{
				if (river.CurrentDirection != Direction.Top){
					river.TurnCount++;
					river.CurrentDirection = Direction.Top;
				}
				FindPathToWater (top, direction, ref river);
			}
		}
	}
    private void DigRiver(River river, River parent)
    {
        int intersectionID = 0;
        int intersectionSize = 0;

        // determine point of intersection
        for (int i = 0; i < river.Tiles.Count; i++)
        {
            gameTile t1 = river.Tiles[i];
            for (int j = 0; j < parent.Tiles.Count; j++)
            {
                gameTile t2 = parent.Tiles[j];
                if (t1.Equals(t2))
                {
                    intersectionID = i;
                    intersectionSize = t2.RiverSize;
                }
            }
        }

        int counter = 0;
        int intersectionCount = river.Tiles.Count - intersectionID;
        int size = UnityEngine.Random.Range(intersectionSize, 5);
        river.Length = river.Tiles.Count;

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;

        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;

        // randomize length of each size
        int count1 = UnityEngine.Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + UnityEngine.Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + UnityEngine.Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            int extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                else if (count3 > 0) { count3--; count4--; extra--; }
                else if (count4 > 0) { count4--; extra--; }
            }
        }

        // adjust size of river at intersection point
        if (intersectionSize == 1)
        {
            count4 = intersectionCount;
            count1 = 0;
            count2 = 0;
            count3 = 0;
        }
        else if (intersectionSize == 2)
        {
            count3 = intersectionCount;
            count1 = 0;
            count2 = 0;
        }
        else if (intersectionSize == 3)
        {
            count2 = intersectionCount;
            count1 = 0;
        }
        else if (intersectionSize == 4)
        {
            count1 = intersectionCount;
        }
        else
        {
            count1 = 0;
            count2 = 0;
            count3 = 0;
            count4 = 0;
        }

        // dig out the river
        for (int i = river.Tiles.Count - 1; i >= 0; i--)
        {

            gameTile t = river.Tiles[i];

            if (counter < count1)
            {
                t.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                t.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                t.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                t.DigRiver(river, 1);
            }
            else
            {
                t.DigRiver(river, 0);
            }
            counter++;
        }
    }
    private void DigRiver(River river)
    {
        int counter = 0;

        // How wide are we digging this river?
        int size = UnityEngine.Random.Range(1, 5);
        river.Length = river.Tiles.Count;

        // randomize size change
        int two = river.Length / 2;
        int three = two / 2;
        int four = three / 2;
        int five = four / 2;

        int twomin = two / 3;
        int threemin = three / 3;
        int fourmin = four / 3;
        int fivemin = five / 3;

        // randomize lenght of each size
        int count1 = UnityEngine.Random.Range(fivemin, five);
        if (size < 4)
        {
            count1 = 0;
        }
        int count2 = count1 + UnityEngine.Random.Range(fourmin, four);
        if (size < 3)
        {
            count2 = 0;
            count1 = 0;
        }
        int count3 = count2 + UnityEngine.Random.Range(threemin, three);
        if (size < 2)
        {
            count3 = 0;
            count2 = 0;
            count1 = 0;
        }
        int count4 = count3 + UnityEngine.Random.Range(twomin, two);

        // Make sure we are not digging past the river path
        if (count4 > river.Length)
        {
            int extra = count4 - river.Length;
            while (extra > 0)
            {
                if (count1 > 0) { count1--; count2--; count3--; count4--; extra--; }
                else if (count2 > 0) { count2--; count3--; count4--; extra--; }
                else if (count3 > 0) { count3--; count4--; extra--; }
                else if (count4 > 0) { count4--; extra--; }
            }
        }

        // Dig it out
        for (int i = river.Tiles.Count - 1; i >= 0; i--)
        {
            gameTile t = river.Tiles[i];

            if (counter < count1)
            {
                t.DigRiver(river, 4);
            }
            else if (counter < count2)
            {
                t.DigRiver(river, 3);
            }
            else if (counter < count3)
            {
                t.DigRiver(river, 2);
            }
            else if (counter < count4)
            {
                t.DigRiver(river, 1);
            }
            else
            {
                t.DigRiver(river, 0);
            }
            counter++;
        }
    }
    private void DigRiverGroups()
    {
        for (int i = 0; i < RiverGroups.Count; i++)
        {

            RiverGroup group = RiverGroups[i];
            River longest = null;

            //Find longest river in this group
            for (int j = 0; j < group.Rivers.Count; j++)
            {
                River river = group.Rivers[j];
                if (longest == null)
                    longest = river;
                else if (longest.Tiles.Count < river.Tiles.Count)
                    longest = river;
            }

            if (longest != null)
            {
                //Dig out longest path first
                DigRiver(longest);

                for (int j = 0; j < group.Rivers.Count; j++)
                {
                    River river = group.Rivers[j];
                    if (river != longest)
                    {
                        DigRiver(river, longest);
                    }
                }
            }
        }
    }
    private void spreadMoisture(int x, int y, int chance, int strength) {
        if (x < 0 || y < 0) return;
        if (x >= mapWidth || y >= mapHeight) return;
        if (gameMap[x, y].terrainType != terrain.River)
        {
            if (gameMap[x, y].moistureType != moisture.Highest)
                gameMap[x, y].moistureType += 1;
            if (gameMap[x, y].moistureType == moisture.Lowest)
                gameMap[x, y].moistureValue = 0.05f;
            else if (gameMap[x, y].moistureType == moisture.Low)
                gameMap[x, y].moistureValue = LowValue;
            else if (gameMap[x, y].moistureType == moisture.Medium)
                gameMap[x, y].moistureValue = MediumValue;
            else if (gameMap[x, y].moistureType == moisture.High)
                gameMap[x, y].moistureValue = HighValue;
            else if (gameMap[x, y].moistureType == moisture.Highest)
                gameMap[x, y].moistureValue = HighestValue;
        }

        if (Random.Range(0, 100) < chance) {
            spreadMoisture(x + 1, y, chance - strength, strength);
            spreadMoisture(x, y + 1, chance - strength, strength);
            spreadMoisture(x - 1, y, chance - strength, strength);
            spreadMoisture(x, y - 1, chance - strength, strength);
        }
    }
    private void spreadTerrain(int x, int y, terrain terrainToSpread, List<int> terrainToChange, int chance, int strength, int side, bool check) {
        if (x < 0 || y < 0) return;
        if (x >= mapWidth || y >= mapHeight) return;
        if (check == true)
            if (gameMap[x, y].terrainType == terrainToSpread) return;

        int val = (int)gameMap[x, y].terrainType;
        if (terrainToChange.Contains(val) == true)
            gameMap[x, y].terrainType = terrainToSpread;
        //else return;
        int sideStrength = Mathf.FloorToInt(strength * 1.1f);
        if (Random.Range(0, 100) < chance) {
            //all
            if (side == -1) {
                spreadTerrain(x + 1, y, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
                spreadTerrain(x - 1, y, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
                spreadTerrain(x, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
                spreadTerrain(x, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, -1, true);
            }
            //up
            if (side == 0) {
                spreadTerrain(x, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x, y + 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //up-right
            else if (side == 1) {
                spreadTerrain(x + 1, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x + 1, y + 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //right
            else if (side == 2) {
                spreadTerrain(x + 1, y, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x + 1, y, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //right-down
            else if (side == 3) {
                spreadTerrain(x + 1, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x + 1, y - 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //down
            else if (side == 4) {
                spreadTerrain(x, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x, y - 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //down-left
            else if (side == 5) {
                spreadTerrain(x - 1, y - 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x - 1, y - 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //left
            else if (side == 6) {
                spreadTerrain(x - 1, y, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x - 1, y, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
            //left-up
            else if (side == 7) {
                spreadTerrain(x - 1, y + 1, terrainToSpread, terrainToChange, chance - strength, strength, side, true);
                spreadTerrain(x - 1, y + 1, terrainToSpread, terrainToChange, chance - strength, sideStrength, -1, false);
            }
        }
    }

    public void paint_terrain() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].terrainType == terrain.DeepOcean)
                    SetTileColour(gameColors.terrainColors.DeepOcean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Ocean)
                    SetTileColour(gameColors.terrainColors.Ocean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Beach)
                    SetTileColour(gameColors.terrainColors.Beach, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Plain)
                    SetTileColour(gameColors.terrainColors.Plain, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.River)
                    SetTileColour(gameColors.terrainColors.Ocean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Hills)
                    SetTileColour(gameColors.terrainColors.Hills, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.Mountain)
                    SetTileColour(gameColors.terrainColors.Mountain, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].terrainType == terrain.MountainTop)
                    SetTileColour(gameColors.terrainColors.MountainTop, new Vector3Int(x, y, 0));
            }
        }
        MapTexture.Apply();
    }
    public void paint_ClimateZones() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].climateType == climate.Polar)
                    SetTileColour(gameColors.climateColors.Polar, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Cold)
                    SetTileColour(gameColors.climateColors.Cold, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Temperate)
                    SetTileColour(gameColors.climateColors.Temperate, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].climateType == climate.Subtropical)
                    SetTileColour(gameColors.climateColors.Subtropical, new Vector3Int(x, y, 0));
                else
                    SetTileColour(gameColors.climateColors.Tropical, new Vector3Int(x, y, 0));
            }
        }
        MapTexture.Apply();
    }
    public void paint_moisture() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].moistureType == moisture.Highest)
                    SetTileColour(gameColors.MoistureColors.Highest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].moistureType == moisture.High)
                    SetTileColour(gameColors.MoistureColors.High, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].moistureType == moisture.Medium)
                    SetTileColour(gameColors.MoistureColors.Medium, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].moistureType == moisture.Low)
                    SetTileColour(gameColors.MoistureColors.Low, new Vector3Int(x, y, 0));
                else
                    SetTileColour(gameColors.MoistureColors.Lowest, new Vector3Int(x, y, 0));
            }
        }
        MapTexture.Apply();
    }
    public void paint_biomes() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {
                if (gameMap[x, y].biomeType == biome.Ice)
                    SetTileColour(gameColors.BiomeColors.Ice, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.SnowWasteland)
                    SetTileColour(gameColors.BiomeColors.SnowWasteland, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Tundra)
                    SetTileColour(gameColors.BiomeColors.Tundra, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.BorealForest)
                    SetTileColour(gameColors.BiomeColors.BorealForest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Grassland)
                    SetTileColour(gameColors.BiomeColors.Grassland, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.TemperateForest)
                    SetTileColour(gameColors.BiomeColors.TemperateForest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.SeasonalForest)
                    SetTileColour(gameColors.BiomeColors.SeasonalForest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Savanna)
                    SetTileColour(gameColors.BiomeColors.Savanna, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Desert)
                    SetTileColour(gameColors.BiomeColors.Desert, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.Rainforest)
                    SetTileColour(gameColors.BiomeColors.Rainforest, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.River)
                    SetTileColour(gameColors.terrainColors.Ocean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.WarmOcean)
                    SetTileColour(gameColors.BiomeColors.WarmOcean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.TemperateOcean)
                    SetTileColour(gameColors.BiomeColors.TemperateOcean, new Vector3Int(x, y, 0));
                else if (gameMap[x, y].biomeType == biome.ColdOcean)
                    SetTileColour(gameColors.BiomeColors.ColdOcean, new Vector3Int(x, y, 0));
            }
        }
        MapTexture.Apply();
    }
    public void paint_ready() {
        for (int x = 0; x < mapWidth; x++) {
            for (int y = 0; y < mapHeight; y++) {

            }
        }
    }
    private int coordFix(int coord, int limit)
    {
        if (coord < limit) return coord;
        else return Mathf.Abs(coord - limit);
    }
}
