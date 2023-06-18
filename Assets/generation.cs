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
    private static Color DeepColor = new Color(68 / 255f, 139  / 255f, 237  / 255f, 1);
    private static Color ShallowColor = new Color(93  / 255f, 180  / 255f, 246  / 255f, 1);
    private static Color SandColor = new Color(232  / 255f, 228  / 255f, 167  / 255f, 1);
    private static Color GrassColor = new Color(129  / 255f, 179  / 255f, 96  / 255f, 1);
    private static Color ForestColor = new Color(102 / 255f, 142 / 255f, 87 / 255f, 1);
    private static Color RockColor = new Color(99 / 255f, 110  / 255f, 114  / 255f, 1);
    private static Color SnowColor = new Color(1, 1, 1, 1);

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
                if (noiseMap[x, y] > 0.8f)
                    SetTileColour(RockColor, new Vector3Int(x, y, 0));
                else if(noiseMap[x, y] > 0.6f)
                    SetTileColour(ForestColor, new Vector3Int(x, y, 0));
                else if(noiseMap[x, y] > 0.4f)
                    SetTileColour(GrassColor, new Vector3Int(x, y, 0));
                else if (noiseMap[x, y] > 0.3f)
                    SetTileColour(SandColor, new Vector3Int(x, y, 0));
                else if (noiseMap[x, y] > 0.2f)
                    SetTileColour(ShallowColor, new Vector3Int(x, y, 0));
                else if (noiseMap[x, y] > 0.1f)
                    SetTileColour(ShallowColor, new Vector3Int(x, y, 0));
                else
                    SetTileColour(DeepColor, new Vector3Int(x, y, 0));
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
                //���������� ��� � ���������� �����������
                float x1 = x / (float)mapWidth;
                float y1 = y / (float)mapHeight;

                float value = (float)module.Get(x1, y1);

                //����������� ������������ � ����������� ��������� ��������
                if (value > max) max = value;
                if (value < min) min = value;

                value = (value - min) / (max - min);
                noiseMap[x, y] = value;
            }
        }
    }
}