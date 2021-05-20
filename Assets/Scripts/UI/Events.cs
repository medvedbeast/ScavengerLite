using UnityEngine;
using Enumerations;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class Events : MonoBehaviour
{
    public GameObject previewCamera;

    protected GameObject gui;
    protected UI ui;

    public void Start()
    {
        gui = GameObject.Find("GUI");
        ui = GetComponent<UI>();
    }

    public void OnStartButtonClick()
    {
        UI.Hide();
        UI.Show("Game");

        OnPlayerHpChanged(Game.player.GetComponent<Destructible>());
        OnPlayerXpChanged(Game.player.GetComponent<Unit>());

        Game.gameState = GAME_STATE.GAME;
        Time.timeScale = 1;
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnShowCharacterWindowClicked()
    {
        previewCamera.SetActive(true);
        Game.gameState = GAME_STATE.PAUSE;
        Time.timeScale = 0;
        UI.Show("Character");
    }

    public void OnCloseCharacterWindowClicked()
    {
        previewCamera.SetActive(false);
        Game.gameState = GAME_STATE.GAME;
        Time.timeScale = 1;
        UI.Hide("Character");
    }

    public void OnPlayerHpChanged(Destructible player)
    {
        var e = GameObject.Find("GUI/Game/HpBackground/Hp");
        float fill = 0.2f + (0.8f * ((player.unit.hp / (player.unit.maxHp * 0.01f)) / 100));
        e.GetComponent<Image>().fillAmount = fill;

        e = GameObject.Find("GUI/Game/HpBackground/TextBackground/Container/Hp");
        e.GetComponent<Text>().text = $"{Math.Round(player.unit.hp)} / {Math.Round(player.unit.maxHp)}";
    }

    public void OnPlayerXpChanged(Unit playerUnit)
    {
        var e = GameObject.Find("GUI/Game/HpBackground/XpBackground/Xp");
        float fill = ((playerUnit.xp / (playerUnit.maxXp * 0.01f)) / 100);
        e.GetComponent<Image>().fillAmount = fill;

        e = GameObject.Find("GUI/Game/HpBackground/XpBackground/TextBackground/Text");
        e.GetComponent<Text>().text = playerUnit.level.ToString();
    }

    public void OnStorableMouseOver(GameObject slot, Storable s)
    {
        if (!Dragable.inProgress)
        {
            var slotRect = slot.GetComponent<RectTransform>();
            Vector3[] corners = new Vector3[4];
            slotRect.GetWorldCorners(corners);
            var position = Camera.main.WorldToScreenPoint(corners[2]);
            position.x += 3;

            UI.Show("ItemDescription");

            var panel = GameObject.Find("GUI/ItemDescription/Container");

            panel.GetComponent<RectTransform>().anchoredPosition = position;

            string color = Functions.GetRarityColor(s.rarity);
            var tooltip = GameObject.Find("GUI/ItemDescription/Container/Background/Text");
            string text = $"<b><color={color}>{s.name}</color></b>\n<i><color=#757575>{s.GetType()}</color></i>";
            if (s is Module)
            {
                Module m = s as Module;
                text += $"\n\n+{m.hp} hp\n+{m.armor} armor";
                if (m is Weapon)
                {
                    Weapon w = m as Weapon;
                    text += $"\n{w.attackSpeed} attack speed\n{w.damage} damage";
                }
            }
            text += $"\n\n<i><color=#757575>{s.description}</color></i>";
            tooltip.GetComponent<Text>().text = text;
        }
    }

    public void OnStorableMouseOut()
    {
        UI.Hide("ItemDescription");
    }

    public void OnPlayerInventoryChanged(Unit playerUnit)
    {
        int i = 0;
        for (; i < playerUnit.storage.inventory.Count; i++)
        {
            var item = playerUnit.storage.inventory[i];
            var slot = ui.inventorySlots[i];

            EventTrigger.Entry onEnter = new EventTrigger.Entry();
            onEnter.eventID = EventTriggerType.PointerEnter;
            onEnter.callback.AddListener((eventData) =>
            {
                OnStorableMouseOver(slot, item);
            });
            slot.GetComponent<EventTrigger>().triggers.Add(onEnter);

            EventTrigger.Entry onExit = new EventTrigger.Entry();
            onExit.eventID = EventTriggerType.PointerExit;
            onExit.callback.AddListener((eventData) =>
            {
                OnStorableMouseOut();
            });
            slot.GetComponent<EventTrigger>().triggers.Add(onExit);

            var dragable = slot.GetComponent<Dragable>();
            dragable.storable = item;
            dragable.isEmpty = false;

            var textComponent = slot.transform.Find("Background/Text").GetComponent<Text>().text = $"#{item.id}";
        }
        for (; i < ui.inventorySlots.Count; i++)
        {
            var slot = ui.inventorySlots[i];
            slot.GetComponent<EventTrigger>().triggers.Clear();

            var dragable = slot.GetComponent<Dragable>();
            dragable.storable = null;
            dragable.isEmpty = true;

            var textComponent = slot.transform.Find("Background/Text").GetComponent<Text>().text = "+";
        }
    }

    public void OnPlayerEquipmentChanged(Unit playerUnit)
    {
        var slotList = new List<StorableSlot>();
        slotList.AddRange(ui.equipmentSlots);
        for (int i = 0; i < playerUnit.storage.equipment.Count; i++)
        {
            var item = playerUnit.storage.equipment[i];
            var slot = slotList.Where(e => e.type == item.GetType()).FirstOrDefault();
            if (slot.link != null)
            {
                EventTrigger.Entry onEnter = new EventTrigger.Entry();
                onEnter.eventID = EventTriggerType.PointerEnter;
                onEnter.callback.AddListener((eventData) =>
                {
                    OnStorableMouseOver(slot.link, item);
                });
                slot.link.GetComponent<EventTrigger>().triggers.Add(onEnter);

                EventTrigger.Entry onExit = new EventTrigger.Entry();
                onExit.eventID = EventTriggerType.PointerExit;
                onExit.callback.AddListener((eventData) =>
                {
                    OnStorableMouseOut();
                });
                slot.link.GetComponent<EventTrigger>().triggers.Add(onExit);

                var dragable = slot.link.GetComponent<Dragable>();
                dragable.storable = item;
                dragable.isEmpty = false;

                var textComponent = slot.link.transform.Find("Background/Text").GetComponent<Text>().text = $"#{item.id}";
                slotList.Remove(slot);
            }
        }
        for (int i = 0; i < slotList.Count; i++)
        {
            var slot = slotList[i];
            slot.link.GetComponent<EventTrigger>().triggers.Clear();

            var dragable = slot.link.GetComponent<Dragable>();
            dragable.storable = null;
            dragable.isEmpty = true;

            var textComponent = slot.link.transform.Find("Background/Text").GetComponent<Text>().text = "+";
        }
    }

    public void OnLootContainerClicked(Interactive container)
    {
        var c = container as LootContainer;
        Time.timeScale = 0;
        Game.gameState = GAME_STATE.PAUSE;
        UI.InitializeLootUI(c);
        UI.Show("Loot");
    }

    public void OnLootItemClicked(LootContainer container, Storable item, GameObject slot)
    {
        var unit = Game.player.GetComponent<Unit>();
        if (unit.storage.inventory.Count + 1 < unit.storage.inventorySize)
        {
            unit.storage.inventory.Add(item);
            container.loot.Remove(item);
            DestroyImmediate(slot);
        }
        if (container.loot.Count == 0)
        {
            container.isActive = false;
            container.Highlight(false);
            OnCloseLootWindowClicked();    
        }
        UI.Hide("ItemDescription");
    }

    public void OnCloseLootWindowClicked()
    {
        UI.Hide("Loot");
        Time.timeScale = 1;
        Game.gameState = GAME_STATE.GAME;
    }

    public void OnDoorClicked(Interactive interactive)
    {
        var animator = interactive.gameObject.GetComponent<Animator>();
        var isActive = animator.GetBool("isOpen");
        animator.SetBool("isOpen", !isActive);
    }
}
