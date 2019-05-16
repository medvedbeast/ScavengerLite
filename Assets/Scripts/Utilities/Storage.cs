using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Storage
{
    public Unit unit;
    public ObservableList<Module> equipment;
    public ObservableList<Storable> inventory;
    public int inventorySize;

    public event System.Action<Unit> EquipmentChanged;
    public event System.Action<Unit> InventoryChanged;

    public Storage(Unit u, int inventorySize)
    {
        unit = u;
        this.inventorySize = inventorySize;
        equipment = new ObservableList<Module>();
        inventory = new ObservableList<Storable>();
        equipment.Changed += OnEquipmentChanged;
        inventory.Changed += OnInventoryChanged;
    }

    public void Swap(Module equipmentItem, Module inventoryItem, int index)
    {
        int j = 0;
        for (int i = 0; i < equipment.Count; i++)
        {
            if (equipment[i].type == equipmentItem.type)
            {
                if (j == index)
                {
                    equipment.Insert(inventoryItem, i, true);
                    inventory.Add(equipmentItem, true);
                    equipment.RemoveAt(i + 1);
                    inventory.Remove(inventoryItem);
                    break;
                }
                j++;
            }
        }
    }

    public void Unequip(Module m)
    {
        equipment.Remove(m);
        inventory.Add(m);
    }

    public void Equip(Module m)
    {
        inventory.Remove(m);
        equipment.Add(m);
    }

    private void OnInventoryChanged()
    {
        InventoryChanged?.Invoke(this.unit);
    }

    private void OnEquipmentChanged()
    {
        EquipmentChanged?.Invoke(this.unit);
    }
}
