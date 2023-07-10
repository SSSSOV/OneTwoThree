using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public List<Citizen> citizens;
    public int amount;
    public int foodAmount;
    public int happiness;
    public BaseYield populationYield;

    public Population(int citizenAmount) {
        foodAmount = 0;
        citizens= new List<Citizen>();
        populationYield = new BaseYield(0, 0, 0, 0);
        amount = citizenAmount;
        for (int i = 0;i < citizenAmount; i++)
        {
            citizens.Add(new Citizen());
            populationYield += citizens[i].basicYield;
        }
        updateHappiness();
    }

    public bool Update(int foodIncome)
    {
        int thresold = citizens.Count * 10;
        foodAmount+= foodIncome;

        if (foodAmount > thresold) {
            addCitizen();
            foodAmount -= thresold;
            return true;
        }
        else if (foodAmount < -thresold) {
            killCitizen(Random.Range(0, citizens.Count));
            foodAmount += thresold;
            return true;
        }
        return false;
    }
    public void addCitizen()
    {
        int par1, par2;
        if (citizens.Count > 1)
        {
            do
            {
                par1 = Random.Range(0, citizens.Count);
                par2 = Random.Range(0, citizens.Count);
            } while (par1 != par2);
            Citizen newCitizen = new Citizen(citizens[par1], citizens[par2]);
            citizens.Add(newCitizen);
            amount++;
            populationYield += newCitizen.basicYield;
        }
        else
        {
            citizens.Add(new Citizen());
        }
        updateHappiness();
    }
    public bool killCitizen(int index)
    {
        if (citizens.Count > 0 && index >= 0 && index < citizens.Count)
        {
            populationYield -= citizens[index].basicYield;
            citizens.RemoveAt(index);
            amount--;
            updateHappiness();
            return true;
        }
        else return false;
    }
    public void updateHappiness()
    {
        happiness = 0;
        foreach (Citizen pop in citizens)
            happiness += pop.happiness;
        happiness = happiness / amount;
    }


}
