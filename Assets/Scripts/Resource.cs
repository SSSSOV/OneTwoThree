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

    static Resource()
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load("Resources.xml");
        XmlElement xRoot = xDoc.DocumentElement;

        if (xRoot != null)
        {
            foreach (XmlElement xRes in xRoot)
            {
                int id = int.Parse(xRes.Attributes.GetNamedItem("Id").Value);
                string name = xRes.Attributes.GetNamedItem("Name").Value;
                string spritePath = xRes.Attributes.GetNamedItem("SpritePath").Value;
                bool strategic = bool.Parse(xRes.Attributes.GetNamedItem("Strategic").Value);
                List<terrain> tList = new List<terrain>();
                string[] array = xRes.Attributes.GetNamedItem("Terrain").Value.Split(',');
                foreach (string ter in array)
                {
                    tList.Add((terrain)int.Parse(ter));
                }
                List<climate> cList = new List<climate>();
                array = xRes.Attributes.GetNamedItem("Climate").Value.Split(',');
                foreach (string ter in array)
                {
                    cList.Add((climate)int.Parse(ter));
                }
                resources.Add(new Resource(id, name, spritePath, strategic, tList, cList));
            }
        }
    }

}
