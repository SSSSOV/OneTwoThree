using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseYield
{
    public int food;
    public int production;
    public int science;
    public int gold;

    public BaseYield() {
        food = 0;
        production = 0;
        science = 0;
        gold = 0;
    }

    public BaseYield(int food, int production, int science, int gold)
    {
        this.food = food;
        this.production = production;
        this.science = science;
        this.gold = gold;
    }

    public BaseYield(string pars)
    {
        string[] arr = pars.Split(',');
        if (int.TryParse(arr[0], out this.food) == false) this.food = 0;
        if (int.TryParse(arr[1], out this.production) == false) this.production = 0;
        if (int.TryParse(arr[2], out this.science) == false) this.science = 0;
        if (int.TryParse(arr[3], out this.gold) == false) this.gold = 0;
    }

    public static BaseYield operator +(BaseYield a, BaseYield b)
    {
        return new BaseYield(
                a.food + b.food,
                a.production + b.production,
                a.science + b.science,
                a.gold + b.gold
            );
    }
    public static BaseYield operator -(BaseYield a) => new BaseYield(-a.food, -a.production, -a.science, -a.gold);
    public static BaseYield operator -(BaseYield a, BaseYield b) => a + (-b);

    public static BaseYield operator *(BaseYield a, float b)
    {
        return new BaseYield(
                Mathf.FloorToInt((float)a.food * b),
                Mathf.FloorToInt((float)a.production * b),
                Mathf.FloorToInt((float)a.science * b),
                Mathf.FloorToInt((float)a.gold * b)
            );
    }
}
