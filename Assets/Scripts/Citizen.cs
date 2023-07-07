using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum profession
{
    None,
    farmer, 
    lightWorker,
    heavyWorker, 
    researcher,
    clerk,
}
public class Citizen
{
    public profession origin;
    public profession job;
    public BaseYield basicYield;
    public int happiness
    {
        get
        {
            return _happiness;
        }
        set
        {
            if (value > 0 && value < 100) _happiness= value;
        }
    }
    private int _happiness;

    public Citizen()
    {
        origin = (profession)Random.Range(1, 6);
        job = profession.None;
        basicYield = new BaseYield(1, 1, 1, 1);
        happiness = 50;
    }

    public Citizen(Citizen par1, Citizen par2)
    {
        int value = Random.Range(0, 100);
        if (value > 90) origin = (profession)Random.Range(1, 6);
        else if (value > 45) origin = par1.origin;
        else if (value > 0) origin = par2.origin;
        job = profession.None;
        basicYield = new BaseYield(1, 1, 1, 1);
        happiness = 50;
    }
}
