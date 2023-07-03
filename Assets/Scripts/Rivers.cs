using UnityEngine;
using System.Collections.Generic;



public class River
{

    public int Length;
    public List<gameTile> Tiles;
    public int ID;

    public int Intersections;
    public float TurnCount;
    public Direction CurrentDirection;

    public River(int id)
    {
        ID = id;
        Tiles = new List<gameTile>();
    }

    public void addTile(gameTile tile)
    {
        tile.setRiverPath(this);
        Tiles.Add(tile);
    }
}

public class RiverGroup
{
    public List<River> Rivers = new List<River>();
}
