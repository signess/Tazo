using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemCategory
{ Items, Tazocatcher, KeyItems, Fruits, MoveMachines, Medicines }

public class Inventory : MonoBehaviour, ISavable
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
        "Items", "Tazocatchers", "Key Items", "Fruits", "MMs & SMs", "Medicines"
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

    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].Item;
    }

    public ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem)
            return ItemCategory.Medicines;
        else if (item is TazocatcherItem)
            return ItemCategory.Tazocatcher;
        else if (item is MMItem)
            return ItemCategory.MoveMachines;
        else
            return ItemCategory.KeyItems;
    }

    public bool HasItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        return currentSlots.Exists(slot => slot.Item == item);
    }

    public ItemBase UseItem(int itemIndex, Tazo selectedTazo, int selectedCategory)
    {
        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.Use(selectedTazo);
        if (itemUsed)
        {
            if (!item.IsReuseable)
                RemoveItem(item);
            return item;
        }
        return null;
    }

    public void AddItem(ItemBase item, int count = 1)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.Item == item);
        if(itemSlot != null)
        {
            itemSlot.Count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                Item = item,
                Count = count
            });
        }

        OnUpdate?.Invoke();
    }

    public void RemoveItem(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.First(slot => slot.Item == item);
        itemSlot.Count--;
        if (itemSlot.Count == 0)
            currentSlots.Remove(itemSlot);

        OnUpdate?.Invoke();
    }

    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            Items = itemSlots.Select(i => i.GetSaveData()).ToList(),
            Tazocatchers = tazocatcherSlots.Select(i => i.GetSaveData()).ToList(),
            KeyItems = keySlots.Select(i => i.GetSaveData()).ToList(),
            Fruits = fruitSlots.Select(i => i.GetSaveData()).ToList(),
            MMs = mmSlots.Select(i => i.GetSaveData()).ToList(),
            Medicines = medicineSlots.Select(i => i.GetSaveData()).ToList()
        };
        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = state as InventorySaveData;
        itemSlots = saveData.Items.Select(i => new ItemSlot(i)).ToList();
        tazocatcherSlots = saveData.Tazocatchers.Select(i => new ItemSlot(i)).ToList();
        keySlots = saveData.KeyItems.Select(i => new ItemSlot(i)).ToList();
        fruitSlots = saveData.Fruits.Select(i => new ItemSlot(i)).ToList();
        mmSlots = saveData.MMs.Select(i => new ItemSlot(i)).ToList();
        medicineSlots = saveData.Medicines.Select(i => new ItemSlot(i)).ToList();

        allSlots = new List<List<ItemSlot>>() { itemSlots, tazocatcherSlots, keySlots, fruitSlots, mmSlots, medicineSlots };

        OnUpdate?.Invoke();
    }
}

[System.Serializable]
public class ItemSlot
{
    [SerializeField] private ItemBase item;
    [SerializeField] private int count;

    public ItemBase Item
    {
        get => item;
        set => item = value;
    }

    public int Count
    {
        get => count;
        set => count = value;
    }

    public ItemSlot() { }

    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.Name);
        count = saveData.Count;
    }

    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            Name = item.name,
            Count = count
        };
        return saveData;
    }
}

[System.Serializable]
public class ItemSaveData
{
    public string Name;
    public int Count;
}

[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> Items;
    public List<ItemSaveData> Tazocatchers;
    public List<ItemSaveData> KeyItems;
    public List<ItemSaveData> Fruits;
    public List<ItemSaveData> MMs;
    public List<ItemSaveData> Medicines;
}