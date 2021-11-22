using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCaregory { Items, Tazocatcher, KeyItems, Fruits, MoveMachines, Medicines}

public class Inventory : MonoBehaviour
{
    public event System.Action OnUpdate;

    [SerializeField] private List<ItemSlot> itemSlots;
    [SerializeField] private List<ItemSlot> tazocatcherSlots;
    [SerializeField] private List<ItemSlot> keySlots;
    [SerializeField] private List<ItemSlot> fruitSlots;
    [SerializeField] private List<ItemSlot> mmSlots;
    [SerializeField] private List<ItemSlot> medicineSlots;

    private List<List<ItemSlot>> allSlots;

    public static List<string> ItemCategories { get; set; } = new List<string>()
    {
        "Items", "Tazocatchers", "Key Items", "Fruits", "Move Machines", "Medicines"
    };

    public static Inventory GetInventory()
    {
        return FindObjectOfType<PlayerController>().GetComponent<Inventory>();
    }

    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() { itemSlots, tazocatcherSlots, keySlots, fruitSlots, mmSlots, medicineSlots };
    }

    public List<ItemSlot> GetSlotsByCategory(int categoryIntex)
    {
        return allSlots[categoryIntex];
    }

    public ItemBase UseItem(int itemIndex, Tazo selectedTazo, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var item = currentSlots[itemIndex].Item;
        bool itemUsed = item.Use(selectedTazo);
        if(itemUsed)
        {
            RemoveItem(item, selectedCategory);
            return item;
        }
        return null;
    }

    public void RemoveItem(ItemBase item, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
            itemSlots.Remove(itemSlot);

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
