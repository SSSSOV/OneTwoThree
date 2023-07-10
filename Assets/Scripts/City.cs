using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City
{
    public string name;
    public int level;
    public Vector2Int position;
    public Sprite sprite;
    public Population population;
    public Infrastructure infrastructure;
    public List<gameTile> territory;
    public BaseYield cityYield;
    public int cityHappiness;

    public City(string name, int x, int y, string spritePath, int startingPops, gameTile startingTile)
    {

    }
}
