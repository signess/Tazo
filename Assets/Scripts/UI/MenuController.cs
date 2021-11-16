using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public event Action<int> OnMenuSelected;
    public event Action OnBack;

    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject topHalfFrame;
    [SerializeField] private GameObject bottomHalfFrame;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image selectionArrow;
    [SerializeField] private List<MenuItem> menuItems;

    private float selectionOffSetX = -110f;
    private int selectedItem = 0;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<MenuItem>().ToList();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        UpdateItemSelection();
        StartCoroutine(AnimateMenu(true));
    }

    public void CloseMenu()
    {
        StartCoroutine(AnimateMenu(false));
    }

    private IEnumerator AnimateMenu(bool enabled)
    {
        if (enabled)
        {
            var showSequence = DOTween.Sequence();
            yield return showSequence.Append(topHalfFrame.transform.DOLocalMoveY(540, .5f).SetEase(Ease.InExpo)).Join(bottomHalfFrame.transform.DOLocalMoveY(-540, .5f).SetEase(Ease.InExpo)).Join(canvasGroup.DOFade(1, .5f)).WaitForCompletion();
        }
        else
        {
            var hideSequence = DOTween.Sequence();
            yield return hideSequence.Append(topHalfFrame.transform.DOLocalMoveY(1080, .5f).SetEase(Ease.InFlash)).Join(bottomHalfFrame.transform.DOLocalMoveY(-1080, .5f).SetEase(Ease.InFlash)).Join(canvasGroup.DOFade(0, .5f)).WaitForCompletion();
            menu.SetActive(false);
        }
    }

    public void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedItem += 4;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedItem -= 4;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);

        if (prevSelection != selectedItem)
            UpdateItemSelection();

        if(Input.GetKeyDown(KeyCode.Z))
        {
            OnMenuSelected?.Invoke(selectedItem);
            CloseMenu();
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            OnBack?.Invoke();
            CloseMenu();
        }
    }

    private void UpdateItemSelection()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
            {
                selectionArrow.transform.SetParent(menuItems[i].transform, false);
                selectionArrow.transform.position = menuItems[i].transform.position + (new Vector3(selectionOffSetX, 0));
            }
        }
    }
}