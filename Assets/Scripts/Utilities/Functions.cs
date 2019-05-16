using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Enumerations;

public static class Functions
{
    public static UnitPreset ReadUnitPreset(string name)
    {
        StreamReader sr = new StreamReader($"{System.Environment.CurrentDirectory}/{Game.pathToDatabase}/Units/{name}.json");
        string data = sr.ReadToEnd();
        sr.Close();
        UnitPreset preset = JsonConvert.DeserializeObject<UnitPreset>(data);

        for (int i = 0; i < preset.equipmentSlots.Count; i++)
        {
            preset.equipmentSlots[i].type = Type.GetType(preset.equipmentSlots[i].module);
        }

        for (int i = 0; i < preset.equipment.Count; i++)
        {
            var item = ReadStorablePreset(preset.equipment[i].id);
            preset.equipment[i] = item as Module;
        }

        for (int i = 0; i < preset.inventory.Count; i++)
        {
            var item = ReadStorablePreset(preset.inventory[i].id);
            preset.inventory[i] = item;
        }

        return preset;
    }

    public static Storable ReadStorablePreset(int id)
    {

        if (Game.loadedPresets.ContainsKey(id))
        {
            return Storable.Clone(Game.loadedPresets[id]);
        }
        else
        {
            StreamReader sr = new StreamReader($"{System.Environment.CurrentDirectory}/{Game.pathToDatabase}/Storables/{id}.json");
            string data = sr.ReadToEnd();
            sr.Close();

            Storable s = JsonConvert.DeserializeObject<Storable>(data, new StorableConverter());

            Game.loadedPresets.Add(s.id, s);

            return s;
        }
    }

    public static string GetRarityColor(RARITY rarity)
    {
        switch (rarity)
        {
            case RARITY.POOR: return "#757575";
            case RARITY.COMMON: return "#FFFFFF";
            case RARITY.UNCOMMON: return "#1ADA05";
            case RARITY.RARE: return "#0168CD";
            case RARITY.EPIC: return "#9E34E8";
            case RARITY.LEGENDARY: return "#FC7F00";
            case RARITY.HEIRLOOM: return "#DFC27B";
        }
        return "#FFFFFF";
    }
}
