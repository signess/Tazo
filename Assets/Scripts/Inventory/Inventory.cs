using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public event System.Action OnUpdate;

    [SerializeField] private List<ItemSlot> slots;

    public List<ItemSlot> Slots => slots;

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    public ItemBase UseItem(int itemIndex, Tazo selectedTazo)
    {
        var item = slots[itemIndex].Item;
        bool itemUsed = item.Use(selectedTazo);
        if(itemUsed)
        {
            RemoveItem(item);
            return item;
        }
        return null;
    }

    public void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
            slots.Remove(itemSlot);

        OnUpdate?.Invoke();
    }
}

[System.Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemBase Item => item;
    public int Count
    {
        get => count;
        set => count = value;
    }
}
