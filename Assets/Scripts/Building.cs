using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public enum prodCosts
{
    tier1 = 90,
    tier2 = 600,
    tier3 = 2850,
}

public class Building
{
    public int id;
    public string name;
    public Sprite sprite;
    public BaseYield basicYield;
    public int tier;
    public int productionCost;
    public int workingPlaces;
    public profession profession;
    public List<Citizen> workingCitizens;
    public bool isBuilt = false;

    public static List<Building> Buildings = new List<Building>();

    public Building(int id, string name, string sprite, BaseYield basicYield, int tier, profession profession)
    {
        this.id = id;
        this.name = name;
        this.sprite = Resources.Load(sprite) as Sprite;
        this.basicYield = basicYield;
        switch (tier)
        {
            case 1: 
                this.productionCost = (int)prodCosts.tier1;
                break;
            case 2:
                this.productionCost = (int)prodCosts.tier2;
                break;
            case 3:
                this.productionCost = (int)prodCosts.tier3;
                break;
            default:
                this.productionCost = 0;
                break;
        }
        this.workingPlaces = tier;
        this.profession = profession;
    }
    static Building getBuilding(string name)
    {
        foreach (Building building in Buildings)
        {
            if (building.name.Equals(name)) return building;
        }
        return null;
    }
    static Building getBuilding(int id)
    {
        foreach (Building building in Buildings)
        {
            if (building.id == id) return building;
        }
        return null;
    }
    public static bool operator ==(Building a, Building b) => a.id == b.id;
    public static bool operator !=(Building a, Building b) => a.id != b.id;
    public static bool operator >(Building a, Building b) => a.id > b.id;
    public static bool operator <(Building a, Building b) => a.id < b.id;
    public static bool operator >=(Building a, Building b) => a.id >= b.id;
    public static bool operator <=(Building a, Building b) => a.id <= b.id;
}
