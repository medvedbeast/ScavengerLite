using Enumerations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{

    public bool isActive = true;
    public float distance = 5.0f;
    public List<Renderer> elements;

    protected bool playerInRange = false;
    protected Events events;

    public Interactive()
    {
        elements = new List<Renderer>();
    }

    protected virtual void Start()
    {
        events = GameObject.Find("/Core").GetComponent<Events>();
    }

    protected virtual void Update()
    {
        if (isActive && Game.gameState == GAME_STATE.GAME)
        {
            if (Vector3.Distance(Game.player.transform.position, this.transform.position) <= distance)
            {
                if (!playerInRange)
                {
                    Highlight();
                    playerInRange = true;
                }
            }
            else
            {
                if (playerInRange)
                {
                    Highlight(false);
                    playerInRange = false;
                }
            }

        }
    }

    protected virtual void OnMouseOver() { return; }

    public void Highlight(bool state = true)
    {
        if (state)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].material.EnableKeyword("_EMISSION");
            }
        }
        else
        {
            for (int i = 0; i < elements.Count; i++)
            {
                elements[i].material.DisableKeyword("_EMISSION");
            }
        }
    }
}
