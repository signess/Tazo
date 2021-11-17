using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BagUI : MonoBehaviour
{
    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;

    [SerializeField] private CanvasGroup canvasGroup;

    private int selectedItem = 0;

    private const int ITEMS_IN_VIEWPORT = 8;

    private List<ItemSlotUI> slotUIList;

    private Inventory inventory;
    private RectTransform itemListRect;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
    }

    private void OnEnable()
    {
        UpdateItemList();
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedItem++;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedItem--;

        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.Slots.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    private void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].Frame.enabled = true;
            }
            else
                slotUIList[i].Frame.enabled = false;
        }

        var item = inventory.Slots[selectedItem].Item;
        itemIcon.sprite = item.Icon;
        itemDescription.text = item.Description;

        HandleScrolling();
    }

    private void UpdateItemList()
    {
        //Clear all the existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots)
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    private void HandleScrolling()
    {
        float scrollPos = Mathf.Clamp(selectedItem - ITEMS_IN_VIEWPORT / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    public void OpenBagUI()
    {
        StartCoroutine(AnimateBagUI(true));
    }

    public void CloseBagUI()
    {
        StartCoroutine(AnimateBagUI(false));
    }

    private IEnumerator AnimateBagUI(bool enabled)
    {
        var sequence = DOTween.Sequence();
        if (enabled)
            yield return sequence.Append(transform.DOLocalMoveX(0, .5f)).SetEase(Ease.InOutCubic).Join(canvasGroup.DOFade(1, .5f)).WaitForCompletion();
        else
        {
            yield return sequence.Append(transform.DOLocalMoveX(1920, .5f)).SetEase(Ease.InOutCubic).Join(canvasGroup.DOFade(0, .5f)).WaitForCompletion();
            gameObject.SetActive(false);
        }
    }
}