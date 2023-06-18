using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.Sprites;
using AccidentalNoise;
using UnityEngine.Rendering;

public class generation : MonoBehaviour
{

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

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (noiseMap[x, y] < 0.4f)
                    SetTileColour(Color.white, new Vector3Int(x, y, 0));
                else
                    SetTileColour(Color.blue, new Vector3Int(x, y, 0));
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
}
