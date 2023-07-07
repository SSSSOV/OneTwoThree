using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum prodCosts
{
    tier1 = 50,
    tier2 = 300,
    tier3 = 1250,
}

public enum bonusResources
{
    None,
    //Food
    //Cold
    berries,
    deer,
    mushrooms,
    //Temperate
    wheat,
    cattle,
    wine,
    //Subtropical
    citrus,
    bananas,
    rice,
    //Tropical
    dates,
    camel,
    spices,
    //Any
    fish,
    crabs,
    seaweed,

    //Production
    //Any
    stone,
    marble, 
    gypsum,
    copper,
    lead,
    ironwood,

    reefs,
    corals,

    //Science
    //Any
    tea,
    mercury,
    meteorite,

    //Gold
    //Any
    olives,
    salt,
    gold, 
    silver,
    pearls,
    diamonds,
}

public enum strategicResources
{
    None,
    //Ancient
    horses,
    //Medieval
    iron,
    //Renaissance
    niter,
    //Industrial era
    coal,
    //Modern era
    oil,
    aluminum,
    uranium,
}
public class Building
{
    public string name;
    public Sprite sprite;
    public BaseYield basicYield;
    public int tier;
    public int productionCost;
    public int workingPlaces;
    public profession profession;
    public bool isBuilt = false;

    public Building(string name, BaseYield basicYield, prodCosts tier, int productionOffset, int workingPlaces, profession profession, Sprite sprite)
    {
        this.name = name;
        this.basicYield = basicYield;
        this.tier = (int)tier;
        this.productionCost = (int)tier + productionOffset;
        this.workingPlaces = workingPlaces;
        this.profession = profession;
        this.sprite = sprite;
    }

    public Building(bonusResources resource)
    {
        //food t1
        if (resource == bonusResources.berries)
        {
            name = "Berries farm";
            //sprite = 
            basicYield = new BaseYield(3, 0, 1, -1);
            productionCost = (int)prodCosts.tier1;
        }
        else if (resource == bonusResources.wheat)
        {
            name = "Wheat farm";
            //sprite = 
            basicYield = new BaseYield(3, 1, 0, -1);
            productionCost = (int)prodCosts.tier1;
        }
        else if (resource == bonusResources.rice)
        {
            name = "Rice farm";
            //sprite = 
            basicYield = new BaseYield(3, 1, 0, -1);
            productionCost = productionCost = (int)prodCosts.tier1;
        }
        else if (resource == bonusResources.dates)
        {
            name = "Date farm";
            //sprite = 
            basicYield = new BaseYield(2, 1, 1, -1);
            productionCost = productionCost = (int)prodCosts.tier1;
        }
        else if (resource == bonusResources.fish)
        {
            name = "Fishery";
            //sprite = 
            basicYield = new BaseYield(3, 0, 1, 0);
            productionCost = (int)prodCosts.tier1;
        }

        //food t2
        else if (resource == bonusResources.deer)
        {
            name = "Deer camp";
            //sprite = 
            basicYield = new BaseYield(7, 2, 1, -3);
            productionCost = (int)prodCosts.tier2;
        }
        else if (resource == bonusResources.cattle)
        {
            name = "Livestock farm";
            //sprite = 
            basicYield = new BaseYield(8, 1, 1, -3);
            productionCost = (int)prodCosts.tier2;
        }
        else if (resource == bonusResources.bananas)
        {
            name = "Banana plantation";
            //sprite = 
            basicYield = new BaseYield(8, 1, 1, -1);
            productionCost = (int)prodCosts.tier2;
        }
        else if (resource == bonusResources.camel)
        {
            name = "Camel camp";
            //sprite = 
            basicYield = new BaseYield(7, 3, 2, -4);
            productionCost = (int)prodCosts.tier2;
        }
        else if (resource == bonusResources.crabs)
        {
            name = "Crabs farm";
            //sprite = 
            basicYield = new BaseYield(9, 1, 2, -3);
            productionCost = (int)prodCosts.tier2;
        }

        //food t3
        else if (resource == bonusResources.mushrooms)
        {
            name = "Mushroom farm";
            //sprite = 
            basicYield = new BaseYield(12, 4, 4, -1);
            productionCost = (int)prodCosts.tier3;
        }
        else if (resource == bonusResources.wine)
        {
            name = "Wineyard";
            //sprite = 
            basicYield = new BaseYield(13, 2, 3, 10);
            productionCost = (int)prodCosts.tier3;
        }
        else if (resource == bonusResources.citrus)
        {
            name = "Citrus plantation";
            //sprite = 
            basicYield = new BaseYield(11, 4, 3, 8);
            productionCost = (int)prodCosts.tier3;
        }
        else if (resource == bonusResources.spices)
        {
            name = "Spices plantation";
            //sprite = 
            basicYield = new BaseYield(11, 2, 5, 9);
            productionCost = (int)prodCosts.tier3;
        }
        else if (resource == bonusResources.seaweed)
        {
            name = "Seaweed farm";
            //sprite = 
            basicYield = new BaseYield(13, 3, 4, -1);
            productionCost = (int)prodCosts.tier3;
        }

        //production t1
        else if (resource == bonusResources.stone)
        {
            name = "Stone quarry";
            //sprite = 
            basicYield = new BaseYield(0, 4, 1, -1);
            productionCost = (int)prodCosts.tier1;
        }
        else if (resource == bonusResources.wine)
        {
            name = "Marble quarry";
            //sprite = 
            basicYield = new BaseYield(0, 3, 0, 2);
            productionCost = (int)prodCosts.tier1;
        }

        //production t2
        else if (resource == bonusResources.gypsum)
        {
            name = "Gypsum quarry";
            //sprite = 
            basicYield = new BaseYield(0, 9, 2, 1);
            productionCost = (int)prodCosts.tier2;
        }
        else if (resource == bonusResources.copper)
        {
            name = "Copper mine";
            //sprite = 
            basicYield = new BaseYield(0, 10, 1, 2);
            productionCost = (int)prodCosts.tier2;
        }
        else if (resource == bonusResources.reefs)
        {
            name = "Underwater resources collectors guild";
            //sprite = 
            basicYield = new BaseYield(2, 7, 2, 2);
            productionCost = (int)prodCosts.tier2;
        }

        //production t3
        else if (resource == bonusResources.ironwood)
        {
            name = "Ironwood sawmill";
            //sprite = 
            basicYield = new BaseYield(1, 17, 2, 4);
            productionCost = (int)prodCosts.tier3;
        }
        else if (resource == bonusResources.lead)
        {
            name = "Lead mine";
            //sprite = 
            basicYield = new BaseYield(0, 15, 4, 5);
            productionCost = (int)prodCosts.tier3;
        }
        else if (resource == bonusResources.corals)
        {
            name = "Coral collectors guild";
            //sprite = 
            basicYield = new BaseYield(3, 14, 3, 3);
            productionCost = (int)prodCosts.tier3;
        }
    }
}
