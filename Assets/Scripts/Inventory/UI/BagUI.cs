using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BagUIState
{ ItemSelection, PartySelection, MoveToForget, Busy }

public class BagUI : MonoBehaviour
{
    private Action<ItemBase> onItemUsed;

    [SerializeField] private GameObject itemList;
    [SerializeField] private ItemSlotUI itemSlotUI;

    [Header("Bag Icons")]
    [SerializeField] private Image itemsIcon;

    [SerializeField] private Image medicineIcon;
    [SerializeField] private Image mmIcon;
    [SerializeField] private Image keyIcon;
    [SerializeField] private Image fruitIcon;
    [SerializeField] private Image tazocatcherIcon;

    [SerializeField] private Sprite[] itemsSprites, medicineSprites, mmSprites, keySprites, fruitSprites, tazocatcherSprites = new Sprite[2];

    [Header("Items Category")]
    [SerializeField] private TextMeshProUGUI categoryText;

    [Header("Description")]
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private Image itemIcon;

    [Header("References")]
    [SerializeField] private PartyScreen partyScreen;

    [SerializeField] private MoveSelectionUI moveSelectionUI;

    [SerializeField] private CanvasGroup canvasGroup;

    private int selectedItem = 0;
    private int selectedCategory = 0;

    private MoveBase moveToLearn;

    private BagUIState state;

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

        inventory.OnUpdate += UpdateItemList;
    }

    private void OnEnable()
    {
        UpdateItemList();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;

        if (state == BagUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                selectedItem++;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                selectedItem--;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                selectedCategory++;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                selectedCategory--;

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;

            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                UpdateCategory();
                UpdateItemList();
            }
            else if (prevSelection != selectedItem)
                UpdateItemSelection();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(ItemSelected());
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                onBack?.Invoke();
            }
        }
        else if (state == BagUIState.PartySelection)
        {
            Action onSelected = () =>
            {
                //Use item on selected tazo
                StartCoroutine(UseItem());
            };
            partyScreen.HandleUpdate(onSelected, ClosePartyScreen);
        }
        else if (state == BagUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };
            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    public IEnumerator ItemSelected()
    {
        state = BagUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Battle)
        {
            //in battle
            if (!item.CanUseInsideBattle)
            {
                yield return DialogManager.Instance.ShowDialog($"You cannot use this inside a battle!");
                state = BagUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            //outside battle
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialog($"You cannot use this outside a battle!");
                state = BagUIState.ItemSelection;
                yield break;
            }
        }

        if (selectedCategory == (int)ItemCaregory.Tazocatcher)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is MMItem)
                partyScreen.ShowIfMMIsUsable(item as MMItem);
                //show if tm is useable
        }
    }

    private IEnumerator UseItem()
    {
        state = BagUIState.Busy;

        yield return HandleMMItems();

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if ((usedItem is RecoveryItem))
                yield return DialogManager.Instance.ShowDialog($"The player used {usedItem.Name}!");

            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if ((usedItem is RecoveryItem))
                yield return DialogManager.Instance.ShowDialog($"It won't have any effect!");
        }

        if (partyScreen.isActiveAndEnabled)
            ClosePartyScreen();
    }

    private IEnumerator HandleMMItems()
    {
        var mmItem = inventory.GetItem(selectedItem, selectedCategory) as MMItem;
        if (mmItem == null)
            yield break;

        var tazo = partyScreen.SelectedMember;

        if (tazo.HasMove(mmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialog($"{tazo.Base.Name} already knows {mmItem.Move.Name}!");
            yield break;
        }

        if (!mmItem.CanBeTaught(tazo))
        {
            yield return DialogManager.Instance.ShowDialog($"{tazo.Base.Name} can't learn {mmItem.Move.Name}!");
            yield break;
        }

        if (tazo.Moves.Count < 4)
        {
            tazo.LearnMove(mmItem.Move);
            yield return DialogManager.Instance.ShowDialog($"{tazo.Base.Name} learned {mmItem.Move.Name}!");
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog($"{tazo.Base.Name} is trying to learn {mmItem.Move.Name}.");
            yield return DialogManager.Instance.ShowDialog($"But it cannot learn move than 4 moves.");
            yield return DialogManager.Instance.ShowDialog($"Do you want to forget a move, to learn {mmItem.Move.Name}?");
            yield return ChooseMoveToForget(tazo, mmItem.Move);
            yield return new WaitUntil(() => state != BagUIState.MoveToForget);
        }
    }

    private IEnumerator ChooseMoveToForget(Tazo tazo, MoveBase newMove)
    {
        state = BagUIState.Busy;

        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetTazoData(tazo, newMove);
        yield return moveSelectionUI.EnableMoveSelectionUI(true);
        moveToLearn = newMove;

        state = BagUIState.MoveToForget;
    }

    private void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);
        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIList[i].Frame.enabled = true;
            }
            else
                slotUIList[i].Frame.enabled = false;
        }

        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.enabled = true;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }

        HandleScrolling();
    }

    private void UpdateCategory()
    {
        categoryText.text = Inventory.ItemCategories[selectedCategory];

        itemsIcon.sprite = itemsSprites[0];
        tazocatcherIcon.sprite = tazocatcherSprites[0];
        keyIcon.sprite = keySprites[0];
        fruitIcon.sprite = fruitSprites[0];
        mmIcon.sprite = mmSprites[0];
        medicineIcon.sprite = medicineSprites[0];

        switch (Inventory.ItemCategories[selectedCategory])
        {
            case "Items":
                itemsIcon.sprite = itemsSprites[1];
                break;

            case "Tazocatchers":
                tazocatcherIcon.sprite = tazocatcherSprites[1];
                break;

            case "Key Items":
                keyIcon.sprite = keySprites[1];
                break;

            case "Fruits":
                fruitIcon.sprite = fruitSprites[1];
                break;

            case "Move Machines":
                mmIcon.sprite = mmSprites[1];
                break;

            case "Medicines":
                medicineIcon.sprite = medicineSprites[1];
                break;

            case null:
                itemIcon.sprite = itemsSprites[0];
                tazocatcherIcon.sprite = tazocatcherSprites[0];
                keyIcon.sprite = keySprites[0];
                fruitIcon.sprite = fruitSprites[0];
                mmIcon.sprite = mmSprites[0];
                medicineIcon.sprite = medicineSprites[0];
                break;
        }
    }

    private void ResetSelection()
    {
        selectedItem = 0;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        itemDescription.text = "";
    }

    private void UpdateItemList()
    {
        //Clear all the existing items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    private void HandleScrolling()
    {
        if (slotUIList.Count <= ITEMS_IN_VIEWPORT) return;

        float scrollPos = Mathf.Clamp(selectedItem - ITEMS_IN_VIEWPORT / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);
    }

    public void Open()
    {
        StartCoroutine(AnimateBagUI(true));
    }

    public void Close()
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

    private void OpenPartyScreen()
    {
        state = BagUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
        partyScreen.Open();
    }

    private void ClosePartyScreen()
    {
        partyScreen.Close();
        state = BagUIState.ItemSelection;
    }

    private IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var tazo = partyScreen.SelectedMember;

        StartCoroutine(moveSelectionUI.EnableMoveSelectionUI(false));
        if (moveIndex == 4)
        {
            // dont learn any moves
            yield return DialogManager.Instance.ShowDialog($"{tazo.Base.Name} did not learn {moveToLearn.Name}.");
        }
        else
        {
            //forget the selected move
            var selectedMove = tazo.Moves[moveIndex].Base;
            yield return DialogManager.Instance.ShowDialog($"{tazo.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}.");

            tazo.Moves[moveIndex] = new Move(moveToLearn);
        }
        moveToLearn = null;
        state = BagUIState.ItemSelection;
    }
}