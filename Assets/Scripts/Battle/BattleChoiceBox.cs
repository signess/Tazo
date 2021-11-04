using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleChoiceBox : MonoBehaviour
{
    [SerializeField] private GameObject choiceBox;

    [SerializeField] private CanvasGroup choiceBoxCanvasGroup;

    [SerializeField] private TextMeshProUGUI yesText;
    [SerializeField] private TextMeshProUGUI noText;

    public IEnumerator ShowChoiceBox()
    {
        if (choiceBoxCanvasGroup.alpha >= 1)
            yield break;
        choiceBoxCanvasGroup.alpha = 1;
        yield return choiceBox.transform.DOLocalMoveX(960, .5f).SetEase(Ease.InSine).WaitForCompletion();
    }

    public IEnumerator HideChoiceBox()
    {
        if (choiceBoxCanvasGroup.alpha <= 0)
            yield break;
        yield return choiceBox.transform.DOLocalMoveX(1480, .5f).SetEase(Ease.OutSine).WaitForCompletion();
        choiceBoxCanvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = GlobalSettings.Instance.HighlightedColorText;
            yesText.GetComponentInParent<Image>().color = GlobalSettings.Instance.HighlightedColorFrame;

            noText.color = GlobalSettings.Instance.HighlightedColorFrame;
            noText.GetComponentInParent<Image>().color = GlobalSettings.Instance.HighlightedColorText;
        }
        else
        {
            noText.color = GlobalSettings.Instance.HighlightedColorText;
            noText.GetComponentInParent<Image>().color = GlobalSettings.Instance.HighlightedColorFrame;

            yesText.color = GlobalSettings.Instance.HighlightedColorFrame;
            yesText.GetComponentInParent<Image>().color = GlobalSettings.Instance.HighlightedColorText;
        }
    }
}