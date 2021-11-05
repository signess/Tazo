using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private int lettersPerSecond;

    [SerializeField] private BattleChoiceBox choiceBox;

    public bool IsOn { get; set; }

    public void SetDialog(string dialog)
    {
        ShowDialogBox();
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog, bool waitForInput = false)
    {
        dialogText.text = "";
        if (!IsOn)
            yield return ShowDialogBox();
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        if (!waitForInput)
            yield return new WaitForSeconds(1f);
        else
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            yield return HideDialogBox();
        }
    }

    public IEnumerator ShowDialogBox()
    {
        var sequence = DOTween.Sequence();
        yield return sequence.Append(transform.DOLocalMoveX(0, .5f)).SetEase(Ease.OutSine).Join(canvasGroup.DOFade(1, .3f)).WaitForCompletion();
        canvasGroup.alpha = 1;

        IsOn = true;
    }

    public IEnumerator HideDialogBox()
    {
        var sequence = DOTween.Sequence();
        yield return sequence.Append(transform.DOLocalMoveX(1930, .5f)).SetEase(Ease.InSine).Join(canvasGroup.DOFade(0, .3f)).WaitForCompletion();
        dialogText.text = "";
        canvasGroup.alpha = 0;
        transform.DOLocalMoveX(-1940, 0f);
        yield return new WaitForSeconds(.3f);

        IsOn = false;
    }

    public void EnableChoiceBox(bool enabled)
    {
        if(enabled)
        {
            choiceBox.gameObject.SetActive(true);
            StartCoroutine(choiceBox.ShowChoiceBox());
        }
        else
        {
            StartCoroutine(choiceBox.HideChoiceBox());
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        choiceBox.UpdateChoiceBox(yesSelected);
    }
}