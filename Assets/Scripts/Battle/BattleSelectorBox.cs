using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleSelectorBox : MonoBehaviour
{
    [SerializeField] private GameObject actionSelector;
    [SerializeField] private GameObject movesSelector;

    [SerializeField] private CanvasGroup actionCanvasGroup;
    [SerializeField] private CanvasGroup movesCanvasGroup;

    [SerializeField] private List<TextMeshProUGUI> actionTexts;

    [SerializeField] private List<GameObject> movesBox;


    public IEnumerator ShowActionSelector()
    {
        if (actionCanvasGroup.alpha >= 1)
            yield break;
        var sequence = DOTween.Sequence();
        yield return sequence.Append(actionSelector.transform.DOLocalMoveX(960, .5f)).SetEase(Ease.InSine).Join(actionCanvasGroup.DOFade(1,.3f)).WaitForCompletion();
        actionCanvasGroup.alpha = 1;
    }

    public IEnumerator HideActionSelector()
    {
        if (actionCanvasGroup.alpha <= 0)
            yield break;
        var sequence = DOTween.Sequence();
        yield return sequence.Append(actionSelector.transform.DOLocalMoveX(1480, .5f)).SetEase(Ease.OutSine).Join(actionCanvasGroup.DOFade(0, .3f)).WaitForCompletion();
        actionCanvasGroup.alpha = 0;
    }


    public IEnumerator ShowMovesSelector()
    {
        if (movesCanvasGroup.alpha >= 1)
            yield break;
        var sequence = DOTween.Sequence();
        yield return sequence.Append(movesSelector.transform.DOLocalMoveX(0, .5f)).SetEase(Ease.InSine).Join(movesCanvasGroup.DOFade(1, .3f)).WaitForCompletion();
        movesCanvasGroup.alpha = 1;
    }

    public IEnumerator HideMovesSelector()
    {
        if (movesCanvasGroup.alpha <= 0)
            yield break;
        var sequence = DOTween.Sequence();
        yield return sequence.Append(movesSelector.transform.DOLocalMoveX(635, .5f)).SetEase(Ease.OutSine).Join(movesCanvasGroup.DOFade(0, .3f)).WaitForCompletion();
        movesCanvasGroup.alpha = 0;
    }


    public void UpdateActionSelection(int selectedAction)
    {
        for(int i =0; i < actionTexts.Count; i++)
        {
            if(i == selectedAction)
            {
                actionTexts[i].color = GlobalSettings.Instance.HighlightedColorText;
                actionTexts[i].GetComponentInParent<Image>().color = GlobalSettings.Instance.HighlightedColorFrame;
            }
            else
            {
                actionTexts[i].color = GlobalSettings.Instance.HighlightedColorFrame;
                actionTexts[i].GetComponentInParent<Image>().color = GlobalSettings.Instance.HighlightedColorText;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove)
    {
        for (int i = 0; i < movesBox.Count; i++)
        {
            if (i == selectedMove)
            {
                movesBox[i].transform.Find("Frame").gameObject.SetActive(true);
            }
            else
            {
                movesBox[i].transform.Find("Frame").gameObject.SetActive(false);
            }
        }
    }

    public void SetMoveNames(List<Move> moves)
    {
        for(int i = 0; i < movesBox.Count; i++)
        {
            if (i < moves.Count)
            {
                movesBox[i].transform.Find("Name Text").GetComponent<TextMeshProUGUI>().text = moves[i].Base.Name;
                movesBox[i].transform.Find("EP Text").GetComponent<TextMeshProUGUI>().text = $"{moves[i].EP}/{moves[i].Base.EP}";              
                if (moves[i].EP == 0)
                {
                    movesBox[i].transform.Find("EP Text").GetComponent<TextMeshProUGUI>().color = Color.red;
                }
                else
                {
                    movesBox[i].transform.Find("EP Text").GetComponent<TextMeshProUGUI>().color = Color.black;
                }
                switch (moves[i].Base.Type)
                {
                    case TazoType.Normal:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Normal;
                        break;
                    case TazoType.Fire:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Fire;
                        break;
                    case TazoType.Water:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Water;
                        break;
                    case TazoType.Grass:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Grass;
                        break;
                    case TazoType.Flying:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Flying;
                        break;
                    case TazoType.Fighting:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Fighting;
                        break;
                    case TazoType.Poison:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Poison;
                        break;
                    case TazoType.Electric:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Electric;
                        break;
                    case TazoType.Ground:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Ground;
                        break;
                    case TazoType.Rock:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Rock;
                        break;
                    case TazoType.Psychic:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Psychic;
                        break;
                    case TazoType.Ice:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Ice;
                        break;
                    case TazoType.Bug:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Bug;
                        break;
                    case TazoType.Ghost:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Ghost;
                        break;
                    case TazoType.Steel:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Steel;
                        break;
                    case TazoType.Dragon:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Dragon;
                        break;
                    case TazoType.Dark:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Dark;
                        break;
                    case TazoType.Fairy:
                        movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.Fairy;
                        break;
                }
            }
            else
            {
                movesBox[i].transform.Find("Name Text").GetComponent<TextMeshProUGUI>().text = "-";
                movesBox[i].transform.Find("EP Text").GetComponent<TextMeshProUGUI>().text = "";
                movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.None;
            }
        }
    }
}
