using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public class Resource
{
    public int id;
    public string name;
    public Sprite sprite;
    public bool strategic;
    public List<terrain> terrain;
    public List<climate> climate;

    public static List<Resource> resources = new List<Resource>();

    public Resource(int id, string name, string sprite, bool strategic, List<terrain> tList, List<climate> cList)
    {
        this.id = id;
        this.name = name;
        this.sprite = Resources.Load(sprite) as Sprite;
        this.strategic = strategic;
        this.terrain= tList;
        this.climate = cList;
    }
}
