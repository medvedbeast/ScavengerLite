using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Storable storable;
    public System.Type type;
    public bool isEmpty = true;
    public event Action DragEnded;

    private GameObject shade;
    private GameObject dragable;
    private RectTransform dragableRect;

    public static bool inProgress = false;

    /// <summary>
    /// OnDragEnded action delegate.
    /// </summary>
    /// <param name="position">If position == null, Dragable is out of bounds, otherwise position is point, where drag ended.</param>
    public delegate void Action(GameObject subject, object position);

    public void Start()
    {
        shade = transform.Find("Background/Text/Shade").gameObject;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEmpty)
        {
            return;
        }

        shade.SetActive(true);

        var tmpParent = GameObject.Find("GUI");

        dragable = Instantiate(Resources.Load("Prefabs/UI/dragable"), tmpParent.transform) as GameObject;
        dragable.transform.Find("Background/Text").GetComponent<Text>().text = $"#{storable.id}";
        dragableRect = dragable.GetComponent<RectTransform>();

        inProgress = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isEmpty)
        {
            return;
        }

        dragableRect.anchoredPosition = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        shade.SetActive(false);
        DestroyImmediate(dragable);
        inProgress = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var source = eventData.pointerDrag.GetComponent<Dragable>();
        var target = eventData.pointerEnter.transform.parent?.parent?.GetComponent<Dragable>();

        if (target == null || source == null)
        {
            return;
        }

        try
        {
            var unit = Game.player.GetComponent<Unit>();
            if (source.gameObject.tag == "EquipmentSlot" && target.gameObject.tag == "InventorySlot")
            {
                if (source.storable.GetType() == target.storable?.GetType())
                {
                    int index = int.Parse(Regex.Replace(source.gameObject.name, "[A-z]+", ""));
                    unit.storage.Swap(source.storable as Module, target.storable as Module, index);
                }
                else if (target.isEmpty)
                {
                    if (unit.storage.inventory.Count + 1 < unit.storage.inventorySize)
                    {
                        unit.storage.Unequip(source.storable as Module);
                    }
                }
            }
            else if (source.gameObject.tag == "InventorySlot" && target.gameObject.tag == "EquipmentSlot")
            {
                if (target.isEmpty && target.type == source.storable.GetType())
                {
                    unit.storage.Equip(source.storable as Module);
                }
                else if (source.storable.GetType() == target.storable?.GetType())
                {
                    int index = int.Parse(Regex.Replace(target.gameObject.name, "[A-z]+", ""));
                    unit.storage.Swap(target.storable as Module, source.storable as Module, index);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
