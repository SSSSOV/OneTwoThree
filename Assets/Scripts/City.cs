using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class City
{
    public Civilization belongsTo;

    public string name;
    public int level;
    public Vector2Int position;
    public Sprite sprite;
    public Population population;
    public Infrastructure infrastructure;
    public List<gameTile> territory;
    public BaseYield cityYield;

    public static List<string> names;
    public static string nameTweak = "";

    public City(int x, int y, string spritePath, int startingPops, gameTile startingTile)
    {
        int nameIndex = Random.Range(0, names.Count);
        this.name = City.names[nameIndex];
        Civilization.names.RemoveAt(nameIndex);
        generation.gameMap[x, y].occupiedBy = this;
        sprite = Resources.Load<Sprite>(spritePath);
        level = 1;
        population = new Population(startingPops);
        infrastructure = new Infrastructure(new List<int>(), new List<int>());
        territory = new List<gameTile>() { startingTile };
        cityYield = population.populationYield + infrastructure.buildingsYield;
        cityYield = cityYield * (population.happiness / 50f);
    }

    public void update()
    {

    }

    public string getName()
    {
        if (names.Count != 0)
        {
            int nameIndex = Random.Range(0, names.Count);
            string name = nameTweak + City.names[nameIndex];
            Civilization.names.RemoveAt(nameIndex);
            return name;
        }
        else
        {
            XmlInfo.loadNames();
            nameTweak += "New ";
            return getName();
        }
    }
}
