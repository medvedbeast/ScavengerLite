using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Enumerations;
using UnityEngine.EventSystems;

public class UI : MonoBehaviour
{
    [HideInInspector]
    public List<StorableSlot> equipmentSlots;
    [HideInInspector]
    public List<GameObject> inventorySlots;

    private static UI instance;
    private static GameObject gui;

    private Events events;

    public UI()
    {
        equipmentSlots = new List<StorableSlot>();
        inventorySlots = new List<GameObject>();
    }

    void Start()
    {
        events = GetComponent<Events>();
        gui = GameObject.Find("GUI");
        instance = this;
    }

    void Update()
    {
    }

    private void CreateEquipmentSlots()
    {
        var player = Game.player;
        var playerUnit = player.GetComponent<Unit>();
        var slotPrefab = Resources.Load("Prefabs/UI/item") as GameObject;

        for (int i = 0; i < playerUnit.equipmentSlots.Count; i++)
        {
            var equipmentSlot = playerUnit.equipmentSlots[i];
            string pathToParent = "";
            if (equipmentSlot.type == typeof(Weapon))
            {
                pathToParent = $"GUI/Character/Container/Equipment/Background/Weapons";
            }
            else if (equipmentSlot.type == typeof(Upgrade))
            {
                pathToParent = $"GUI/Character/Container/Equipment/Background/Upgrades";
            }
            else
            {
                pathToParent = $"GUI/Character/Container/Equipment/Background/Other";
            }
            for (int j = 0; j < equipmentSlot.count; j++)
            {
                var parent = GameObject.Find(pathToParent);
                var slot = Instantiate(slotPrefab, parent.transform);
                slot.name = $"{equipmentSlot.module}{j}";
                var slotTextComponent = slot.transform.Find("Background/Text").GetComponent<Text>();
                slotTextComponent.text = "<color=#757575>+</color>";
                slot.tag = "EquipmentSlot";
                var dragable = slot.GetComponent<Dragable>();
                dragable.storable = null;
                dragable.type = equipmentSlot.type;
                dragable.isEmpty = true;
                equipmentSlots.Add(new StorableSlot
                {
                    type = equipmentSlot.type,
                    link = slot
                });
            }
        }
    }

    private void CreateInventorySlots()
    {
        var player = Game.player.gameObject;
        var playerUnit = player.GetComponent<Unit>();
        var slotPrefab = Resources.Load("Prefabs/UI/item") as GameObject;

        var parent = GameObject.Find("GUI/Character/Container/Inventory/Background/Content");
        for (int i = 0; i < playerUnit.storage.inventorySize; i++)
        {
            var slot = Instantiate(slotPrefab, parent.transform);
            slot.name = $"Slot{i}";
            var slotTextComponent = slot.transform.Find("Background/Text").GetComponent<Text>();
            slotTextComponent.text = "<color=#757575>+</color>";
            slot.tag = "InventorySlot";
            var dragable = slot.GetComponent<Dragable>();
            dragable.storable = null;
            dragable.isEmpty = true;
            inventorySlots.Add(slot);
        }
    }

    public static void Initialize()
    {
        InitializeCharacterUI();
    }

    public static void InitializeCharacterUI()
    {
        instance.CreateEquipmentSlots();
        instance.CreateInventorySlots();
    }

    public static void InitializeLootUI(LootContainer container)
    {
        var parent = gui.transform.Find("Loot/Container/Content/Background/Content");
        foreach (Transform t in parent.transform)
        {
            Destroy(t.gameObject);
        }

        var slotPrefab = Resources.Load("Prefabs/UI/loot_item") as GameObject;
        for (int i = 0; i < container.loot.Count; i++)
        {
            var item = container.loot[i];
            var slot = Instantiate(slotPrefab, parent.transform);
            slot.name = $"LootItem{i}";
            slot.tag = "LootSlot";
            var slotTextComponent1 = slot.transform.Find("Background/Text").GetComponent<Text>();
            slotTextComponent1.text = $"#{item.id}";
            var slotTextComponent2 = slot.transform.Find("Text").GetComponent<Text>();
            string color = Functions.GetRarityColor(item.rarity);
            slotTextComponent2.text = $"<color={color}>{item.name}</color>";

            EventTrigger.Entry onClick = new EventTrigger.Entry();
            onClick.eventID = EventTriggerType.PointerClick;
            onClick.callback.AddListener((eventData) =>
            {
                instance.events.OnLootItemClicked(container, item, slot);
            });
            slot.GetComponent<EventTrigger>().triggers.Add(onClick);

            EventTrigger.Entry onMouseOver = new EventTrigger.Entry();
            onMouseOver.eventID = EventTriggerType.PointerEnter;
            onMouseOver.callback.AddListener((eventData) =>
            {
                instance.events.OnStorableMouseOver(slot, item);
            });
            slot.GetComponent<EventTrigger>().triggers.Add(onMouseOver);

            EventTrigger.Entry onMouseOut = new EventTrigger.Entry();
            onMouseOut.eventID = EventTriggerType.PointerExit;
            onMouseOut.callback.AddListener((eventData) =>
            {
                instance.events.OnStorableMouseOut();
            });
            slot.GetComponent<EventTrigger>().triggers.Add(onMouseOut);
        }
    }

    public static void Hide()
    {
        for (int i = 0; i < gui.transform.childCount; i++)
        {
            gui.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static void Show()
    {
        for (int i = 0; i < gui.transform.childCount; i++)
        {
            gui.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public static void Hide(string name)
    {
        gui.transform.Find(name).gameObject.SetActive(false);
    }

    public static void Show(string name)
    {
        gui.transform.Find(name).gameObject.SetActive(true);
    }
}
