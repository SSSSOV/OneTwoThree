using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infrastructure
{
    public List<Building> completedBuildings;
    public List<Building> allowedBuildings;
    public BaseYield buildingsYield;
    public int production;
    public int constructing;

    public Infrastructure(List<int> completedIds, List<int> allowedIds) {
        completedBuildings = new List<Building>();
        allowedBuildings = new List<Building>();
        buildingsYield = new BaseYield();
        foreach (Building build in Building.Buildings)
        {
            if (completedIds.Contains(build.id)) completedBuildings.Add(build);
            else if (allowedIds.Contains(build.id)) allowedBuildings.Add(build);
        }
        
        foreach (Building building in completedBuildings)
        {
            buildingsYield += building.basicYield;
        }
        production = 0;
        constructing = -1;
    }

    public bool update(int productionIncome)
    {
        production += productionIncome;
        if (constructing >= 0 && production > allowedBuildings[constructing].productionCost) {
            production -= allowedBuildings[constructing].productionCost;
            build(constructing);
            return true;
        }
        return false;
    }
    public bool build(int allowedIndex)
    {
        if (allowedIndex < 0 || allowedIndex >= allowedBuildings.Count) return false;
        Building temp = allowedBuildings[allowedIndex];
        completedBuildings.Add(temp);
        allowedBuildings.RemoveAt(allowedIndex);
        buildingsYield += temp.basicYield;
        return true;
    }
    public bool destroy(int completedIndex)
    {
        if (completedIndex < 0 || completedIndex >= completedBuildings.Count) return false;
        Building temp = completedBuildings[completedIndex];
        completedBuildings.RemoveAt(completedIndex);
        allowedBuildings.Add(temp);
        buildingsYield -= temp.basicYield;
        return true;
    }

}
