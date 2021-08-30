using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private int lettersPerSecond;


    public void SetDialog(string dialog)
    {
        ShowDialogBox();
        dialogText.text = dialog;
    }
    
    public IEnumerator TypeDialog(string dialog)
    {
        ShowDialogBox();
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }

    public void ShowDialogBox()
    {
        canvasGroup.alpha = 1;
        transform.DOLocalMoveX(0, .5f).SetEase(Ease.OutSine).WaitForCompletion();
    }

    public IEnumerator HideDialogBox()
    {
        yield return transform.DOLocalMoveX(1930, .5f).SetEase(Ease.InSine).WaitForCompletion();
        canvasGroup.alpha = 0;
        transform.DOLocalMoveX(-1940, 0f);
        yield return new WaitForSeconds(.5f);
    }
}
