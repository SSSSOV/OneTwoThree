using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public struct StrategicResources
{
    public int horses;
    public int iron;
    public int niter;
    public int coal;
    public int oil;
    public int aluminum;
    public int uranium;

    public StrategicResources(int horses, int iron, int niter, int coal, int oil, int aluminum, int uranium)
    {
        this.horses = horses;
        this.iron = iron;
        this.niter = niter;
        this.coal = coal;
        this.oil = oil;
        this.aluminum = aluminum;
        this.uranium = uranium;
    }
}

public class Civilization
{
    public string name;

    public int gold;

    public StrategicResources strategicResources;

    public static List<string> names;

    public Civilization()
    {
        int nameIndex = Random.Range(0, names.Count);
        this.name = Civilization.names[nameIndex];
        Civilization.names.RemoveAt(nameIndex);
        strategicResources = new StrategicResources(0,0,0,0,0,0,0);
        gold = 100;
    }
}