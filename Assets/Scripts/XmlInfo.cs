using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class XmlInfo
{
    public static void loadXmlInfo()
    {

    }

    public static void loadResources()
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
                Resource.resources.Add(new Resource(id, name, spritePath, strategic, tList, cList));
            }
        }
    }

    public static void loadBuildings()
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load("Buildings.xml");
        XmlElement xRoot = xDoc.DocumentElement;

        if (xRoot != null)
        {
            foreach (XmlElement xRes in xRoot)
            {
                int id = int.Parse(xRes.Attributes.GetNamedItem("Id").Value);
                string name = xRes.Attributes.GetNamedItem("Name").Value;
                string spritePath = xRes.Attributes.GetNamedItem("SpritePath").Value;
                BaseYield yield = new BaseYield(xRes.Attributes.GetNamedItem("BaseYield").Value);
                int tier = int.Parse(xRes.Attributes.GetNamedItem("Tier").Value);
                int prof = int.Parse(xRes.Attributes.GetNamedItem("Profession").Value);
                Building.Buildings.Add(new Building(id, name, spritePath, yield, tier, (profession)prof));
            }
            Building.Buildings.Sort();
        }
    }

    public static void loadNames()
    {
        XmlDocument xDoc = new XmlDocument();
        xDoc.Load("Names.xml");
        XmlElement xRoot = xDoc.DocumentElement;

        if (xRoot != null)
        {
            foreach (XmlElement xElem in xRoot)
            {
                if (xElem.Name == "Civilizations")
                {
                    Civilization.names = new List<string>(xElem.Attributes.GetNamedItem("CivNames").Value.Split(","));
                }
                if (xElem.Name == "Cities")
                {
                    City.names = new List<string>(xElem.Attributes.GetNamedItem("CityNames").Value.Split(","));
                }
            }
        }
    }
}
