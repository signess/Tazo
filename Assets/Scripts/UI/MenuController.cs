using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject topHalfFrame;
    [SerializeField] private GameObject bottomHalfFrame;
    [SerializeField]private CanvasGroup canvasGroup;
    [SerializeField] private Image selectionArrow;
    [SerializeField] private List<MenuItem> menuItems;

    private float selectionOffSetX = -110f;
    private float selectionOffSetY = 590f;
    private int selectedItem = 0;

    private void Awake()
    {
        menuItems = menu.GetComponentsInChildren<MenuItem>().ToList();
    }

    public void OpenMenu()
    {
        menu.SetActive(true);
        StartCoroutine(AnimateMenu(true));
    }

    private IEnumerator AnimateMenu(bool enabled)
    {
        if(enabled)
        {
            var showSequence = DOTween.Sequence();
            yield return showSequence.Append(topHalfFrame.transform.DOLocalMoveY(540,.5f).SetEase(Ease.OutExpo)).Join(bottomHalfFrame.transform.DOLocalMoveY(-540, .5f).SetEase(Ease.OutExpo)).Join(canvasGroup.DOFade(1, .5f)).WaitForCompletion();
        }
        else
        {
            var hideSequence = DOTween.Sequence();
            yield return hideSequence.Append(topHalfFrame.transform.DOLocalMoveY(-540, .5f).SetEase(Ease.OutExpo)).Join(bottomHalfFrame.transform.DOLocalMoveY(540, .5f).SetEase(Ease.OutExpo)).Join(canvasGroup.DOFade(0, .5f)).WaitForCompletion();
        }
    }

    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;

        selectedItem = Mathf.Clamp(selectedItem, 0, menuItems.Count - 1);
        UpdateItemSelection();
    }

    private void UpdateItemSelection()
    {
        for(int i = 0; i < menuItems.Count; i++)
        {
            if (i == selectedItem)
                selectionArrow.transform.localPosition = menuItems[i].transform.position + (new Vector3(selectionOffSetX, 0));
        }
    }
}
