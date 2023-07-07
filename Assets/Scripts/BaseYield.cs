using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseYield
{
    public int food { get; set; }
    public int production { get; set; }
    public int science { get; set; }
    public int gold { get; set; }

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
}
