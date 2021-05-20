using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootContainer : Interactive
{
    public bool randomizeLoot = true;
    public int lootQuantity;
    public List<int> lootTable;
    [HideInInspector]
    public List<Storable> loot;

    public LootContainer() : base()
    {
        loot = new List<Storable>();
        lootTable = new List<int>();
    }

    protected override void Start()
    {
        base.Start();
        if (randomizeLoot)
        {
            int q = lootQuantity;
            var used = new List<int>();
            while (q > 0)
            {
                var random = new System.Random();
                int index = random.Next(0, lootTable.Count);
                if (used.Contains(index)) continue;
                loot.Add(Functions.ReadStorablePreset(lootTable[index]));
                used.Add(index);
                q--;
            }
        }
        else
        {
            for (int i = 0; i < lootTable.Count; i++)
            {
                loot.Add(Functions.ReadStorablePreset(lootTable[i]));
            }
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnMouseOver()
    {
        if (playerInRange && Input.GetMouseButtonUp(1))
        {
            events.OnLootContainerClicked(this);
        }
    }
}
