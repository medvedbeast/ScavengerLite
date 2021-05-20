using Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UNIT_TYPE type;
    [Space(15)]
    public float xp;
    public float maxXp;
    public int level;
    public float inputDamageMultiplier;
    public float outputDamageMultiplier;
    [Space(15)]
    public float hp;
    public float maxHp;
    public float armor;
    public float moveSpeed;
    public float turnSpeed;
    [Space(15)]
    public float value = 100;
    [Space(15)]
    public bool loadEquipment = false;
    public string preset;

    public Storage storage;
    public List<UnitPresetSlot> equipmentSlots;

    public event System.Action<Unit> UnitXpChanged;
    
    

    public Unit()
    {
        equipmentSlots = new List<UnitPresetSlot>();
        storage = new Storage(this, 54);
    }

    public void Start()
    {
        storage.EquipmentChanged += OnEquipmentChanged;
        if (loadEquipment)
        {
            UnitPreset preset = Functions.ReadUnitPreset(this.preset);
            equipmentSlots.AddRange(preset.equipmentSlots);
            storage.inventory.AddRange(preset.inventory, true);
            storage.equipment.AddRange(preset.equipment, true);
        }
        GetComponent<PlayerController>()?.OnUnitInitialized();
        OnEquipmentChanged(this);
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < storage.equipment.Count; i++)
        {
            if (storage.equipment[i].IsUpdateable())
            {
                storage.equipment[i].Update();
            }
        }
    }

    public void OnEquipmentChanged(Unit u)
    {
        float maxHp = 0;
        float armor = 0;
        float moveSpeed = 0;
        float turnSpeed = 0;
        for (int i = 0; i < storage.equipment.Count; i++)
        {
            maxHp += storage.equipment[i].hp;
            armor += storage.equipment[i].armor;
            if (storage.equipment[i] is Body)
            {
                moveSpeed += (storage.equipment[i] as Body).movementSpeed;
                turnSpeed += (storage.equipment[i] as Body).turnSpeed;
            }
        }
        if (this.maxHp != 0 && this.hp != 0)
        {
            this.hp = (this.hp / (this.maxHp * 0.01f)) * (maxHp * 0.01f);
        }
        else
        {
            this.hp = maxHp;
        }
        this.maxHp = maxHp;
        this.armor = armor;
        this.moveSpeed = moveSpeed;
        this.turnSpeed = turnSpeed;
    }

    public void OnUnitDestroyed(Unit u)
    {
        xp += u.value;
        while (xp > maxXp)
        {
            xp -= maxXp;
            level++;
            maxXp *= 1.15f;
            outputDamageMultiplier += 0.3f;
        }
        UnitXpChanged?.Invoke(this);
    }

}
