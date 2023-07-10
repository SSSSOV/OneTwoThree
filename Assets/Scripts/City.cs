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

    public City(string name, int x, int y, string spritePath, int startingPops, gameTile startingTile)
    {
        this.name = name;
        generation.gameMap[x, y].occupiedBy = this;
        sprite = Resources.Load<Sprite>(spritePath);
        level = 1;
        population = new Population(startingPops);
        infrastructure = new Infrastructure(new List<int>(), new List<int>());
        territory = new List<gameTile>() { startingTile };
        cityYield = population.populationYield + infrastructure.buildingsYield;
        cityYield = cityYield * (population.happiness / 50f);
    }


}
