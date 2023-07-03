using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum terrain
{
    None,
    DeepOcean,
    Ocean,
    Beach,
    Plain,
    River,
    Hills,
    Mountain,
    MountainTop
}
public enum climate
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
public enum biome
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
public enum Direction
{
    Left,
    Right,
    Top,
    Bottom
}
public class gameTile
{
    public gameTile Left;

    public gameTile Right;

    public gameTile Top;

    public gameTile Bottom;

    public int x, y;

    public int bitmask;
    public float terrainValue { get; set; }
    public float climateValue { get; set; }
    public float moistureValue { get; set; }
    public terrain terrainType { get; set; }
    public biome biomeType { get; set; }
    public climate climateType { get; set; }
    public moisture moistureType { get; set; }
    public List<River> Rivers { get; }
    public int RiverSize { get; set; }

    public bool Collidable;

    public bool FloodFilled;

    public gameTile()
    {
        terrainType = terrain.None;
        biomeType = biome.None;
        climateType = climate.None;
        moistureType = moisture.None;
        Collidable = true;
        Rivers = new List<River>();
    }

    public void updateBitmask()
    {
        int count = 0;
            
        if (Collidable && Top.terrainType == terrainType)
            count += 1;
        if (Collidable && Right.terrainType == terrainType)
            count += 2;
        if (Collidable && Bottom.terrainType == terrainType)
            count += 4;
        if (Collidable && Left.terrainType == terrainType)
            count += 8;

        bitmask = count;
    }

    public int getRiverNeighborCount(River river)
    {
        int count = 0;
        if (Left.Rivers.Count > 0 && Left.Rivers.Contains(river))
            count++;
        if (Right.Rivers.Count > 0 && Right.Rivers.Contains(river))
            count++;
        if (Top.Rivers.Count > 0 && Top.Rivers.Contains(river))
            count++;
        if (Bottom.Rivers.Count > 0 && Bottom.Rivers.Contains(river))
            count++;
        return count;
    }
    public Direction GetLowestNeighbor()
    {
        if (Left.terrainValue < Right.terrainValue && Left.terrainValue < Top.terrainValue && Left.terrainValue < Bottom.terrainValue)
            return Direction.Left;
        else if (Right.terrainValue < Left.terrainValue && Right.terrainValue < Top.terrainValue && Right.terrainValue < Bottom.terrainValue)
            return Direction.Right;
        else if (Top.terrainValue < Left.terrainValue && Top.terrainValue < Right.terrainValue && Top.terrainValue < Bottom.terrainValue)
            return Direction.Right;
        else if (Bottom.terrainValue < Left.terrainValue && Bottom.terrainValue < Top.terrainValue && Bottom.terrainValue < Right.terrainValue)
            return Direction.Right; 
        else
            return Direction.Bottom;
    }
    public void setRiverPath(River river)
    {
        if (!Collidable)
            return;

        if (!Rivers.Contains(river))
        {
            Rivers.Add(river);
        }
    }

    private void SetRiverTile(River river)
    {
        setRiverPath(river);
        terrainType = terrain.River;
        Collidable = false;
    }

    public void DigRiver(River river, int size)
    {
        SetRiverTile(river);
        RiverSize = size;

        if (size == 1)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
        }

        if (size == 2)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
            Top.SetRiverTile(river);
            Top.Left.SetRiverTile(river);
            Top.Right.SetRiverTile(river);
            Left.SetRiverTile(river);
            Left.Bottom.SetRiverTile(river);
        }

        if (size == 3)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
            Top.SetRiverTile(river);
            Top.Left.SetRiverTile(river);
            Top.Right.SetRiverTile(river);
            Left.SetRiverTile(river);
            Left.Bottom.SetRiverTile(river);
            Right.Right.SetRiverTile(river);
            Right.Right.Bottom.SetRiverTile(river);
            Bottom.Bottom.SetRiverTile(river);
            Bottom.Bottom.Right.SetRiverTile(river);
        }

        if (size == 4)
        {
            Bottom.SetRiverTile(river);
            Right.SetRiverTile(river);
            Bottom.Right.SetRiverTile(river);
            Top.SetRiverTile(river);
            Top.Right.SetRiverTile(river);
            Left.SetRiverTile(river);
            Left.Bottom.SetRiverTile(river);
            Right.Right.SetRiverTile(river);
            Right.Right.Bottom.SetRiverTile(river);
            Bottom.Bottom.SetRiverTile(river);
            Bottom.Bottom.Right.SetRiverTile(river);
            Left.Bottom.Bottom.SetRiverTile(river);
            Left.Left.Bottom.SetRiverTile(river);
            Left.Left.SetRiverTile(river);
            Left.Left.Top.SetRiverTile(river);
            Left.Top.SetRiverTile(river);
            Left.Top.Top.SetRiverTile(river);
            Top.Top.SetRiverTile(river);
            Top.Top.Right.SetRiverTile(river);
            Top.Right.Right.SetRiverTile(river);
        }
    }
}
