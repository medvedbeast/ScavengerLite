using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Enumerations;
using Newtonsoft.Json;

public class Storable
{
    public int id;
    public RARITY rarity;
    public string name;
    public string description;
    public float price;

    public Storable()
    {

    }

    public Storable(int id)
    {
        this.id = id;
    }

    public override string ToString()
    {
        return $"{name}: {rarity.ToString()}";
    }

    public static Storable Clone(Storable source)
    {
        var text = JsonConvert.SerializeObject(source, new StorableConverter());
        return JsonConvert.DeserializeObject<Storable>(text, new StorableConverter());
    }
}

