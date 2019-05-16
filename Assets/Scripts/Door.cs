using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : Interactive
{
    public List<int> requiredItems;

    protected Unit player;

    public Door()
    {
        requiredItems = new List<int>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnMouseOver()
    {
        if (playerInRange && Input.GetMouseButtonUp(1))
        {
            if (player == null)
            {
                player = Game.player.GetComponent<Unit>();
            }

            if (requiredItems.Count > 0)
            {
                bool flag = true;
                for (int i = 0; i < requiredItems.Count; i++)
                {
                    int count = player.storage.inventory.Count(e => e.id == requiredItems[i]);
                    if (count < 1)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    events.OnDoorClicked(this);
                }
            }
            else
            {
                events.OnDoorClicked(this);
            }
        }
    }

}
