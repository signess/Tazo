using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionUI : MonoBehaviour
{
    [Header("Name / Level / Gender")]
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image genderIcon;

    [Space]
    [Header("Stats / Item / Ability")]
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI spAttackText;
    [SerializeField] private TextMeshProUGUI spDefenseText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI abilityText;
    [SerializeField] private TextMeshProUGUI itemText;

    [Space]
    [Header("Moves")]
    [SerializeField] private List<TextMeshProUGUI> movesText;

    [SerializeField] private List<TextMeshProUGUI> epText;
    [SerializeField] private TextMeshProUGUI moveCategoryText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private Image typeIcon;

    [Space]
    [Header("Misc")]
    [SerializeField] private CanvasGroup canvasGroup;
    private Tazo _tazo;
    private int currentSelection = 0;

    public void SetTazoData(Tazo tazo, MoveBase newMove)
    {
        _tazo = tazo;
        nameText.text = tazo.Base.Name;
        levelText.text = $"Lv. {tazo.Level}";
        switch (tazo.Gender)
        {
            case Gender.Male:
                genderIcon.sprite = GlobalSettings.Instance.MaleIcon;
                genderIcon.enabled = true;
                break;

            case Gender.Female:
                genderIcon.sprite = GlobalSettings.Instance.FemaleIcon;
                genderIcon.enabled = true;
                break;

            case Gender.Neuter:
                genderIcon.sprite = null;
                genderIcon.enabled = false;
                break;
        }
        hpText.text = tazo.Base.MaxHp.ToString();
        attackText.text = tazo.Attack.ToString();
        defenseText.text = tazo.Defense.ToString();
        spAttackText.text = tazo.SpAttack.ToString();
        spDefenseText.text = tazo.SpDefense.ToString();
        speedText.text = tazo.Speed.ToString();

        for (int i = 0; i < tazo.Moves.Count; i++)
        {
            movesText[i].text = tazo.Moves[i].Base.Name;
            epText[i].text = $"{tazo.Moves[i].EP}/{tazo.Moves[i].Base.EP}";
        }
        movesText[tazo.Moves.Count].text = newMove.Name;
        epText[tazo.Moves.Count].text = $"{newMove.EP}/{newMove.EP}";

        moveCategoryText.text = tazo.Moves[currentSelection].Base.Category.ToString();
        powerText.text = tazo.Moves[currentSelection].Base.Power.ToString();
        accuracyText.text = tazo.Moves[currentSelection].Base.Accuracy.ToString();
        switch (tazo.Moves[currentSelection].Base.Type)
        {
            case TazoType.Normal:
                typeIcon.sprite = GlobalSettings.Instance.Normal;
                break;

            case TazoType.Fire:
                typeIcon.sprite = GlobalSettings.Instance.Fire;
                break;

            case TazoType.Water:
                typeIcon.sprite = GlobalSettings.Instance.Water;
                break;

            case TazoType.Grass:
                typeIcon.sprite = GlobalSettings.Instance.Grass;
                break;

            case TazoType.Flying:
                typeIcon.sprite = GlobalSettings.Instance.Flying;
                break;

            case TazoType.Fighting:
                typeIcon.sprite = GlobalSettings.Instance.Fighting;
                break;

            case TazoType.Poison:
                typeIcon.sprite = GlobalSettings.Instance.Poison;
                break;

            case TazoType.Electric:
                typeIcon.sprite = GlobalSettings.Instance.Electric;
                break;

            case TazoType.Ground:
                typeIcon.sprite = GlobalSettings.Instance.Ground;
                break;

            case TazoType.Rock:
                typeIcon.sprite = GlobalSettings.Instance.Rock;
                break;

            case TazoType.Psychic:
                typeIcon.sprite = GlobalSettings.Instance.Psychic;
                break;

            case TazoType.Ice:
                typeIcon.sprite = GlobalSettings.Instance.Ice;
                break;

            case TazoType.Bug:
                typeIcon.sprite = GlobalSettings.Instance.Bug;
                break;

            case TazoType.Ghost:
                typeIcon.sprite = GlobalSettings.Instance.Ghost;
                break;

            case TazoType.Steel:
                typeIcon.sprite = GlobalSettings.Instance.Steel;
                break;

            case TazoType.Dragon:
                typeIcon.sprite = GlobalSettings.Instance.Dragon;
                break;

            case TazoType.Dark:
                typeIcon.sprite = GlobalSettings.Instance.Dark;
                break;

            case TazoType.Fairy:
                typeIcon.sprite = GlobalSettings.Instance.Fairy;
                break;
        }
    }

    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentSelection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentSelection;

        currentSelection = Mathf.Clamp(currentSelection, 0, 4);
        UpdateMoveSelection(currentSelection);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke(currentSelection);
        }
    }

    public void UpdateMoveSelection(int selection)
    {
        for(int i = 0; i < 5; i++)
        {
            if (i == selection)
            {
                movesText[i].color = GlobalSettings.Instance.HighlightedColorText;
                epText[i].color = GlobalSettings.Instance.HighlightedColorText;
            }
            else
            {
                movesText[i].color = GlobalSettings.Instance.HighlightedColorFrame;
                epText[i].color = GlobalSettings.Instance.HighlightedColorFrame;
            }
        }
    }

    public IEnumerator EnableMoveSelectionUI(bool enabled)
    {
        if (enabled && canvasGroup.alpha >= 1)
            yield break;
        else if (!enabled && canvasGroup.alpha <= 0)
            yield break;

        var sequence = DOTween.Sequence();
        if (enabled)
        {
            yield return sequence.Append(transform.DOLocalMoveX(0, .5f)).SetEase(Ease.InSine).Join(canvasGroup.DOFade(1, .5f)).WaitForCompletion();
            canvasGroup.alpha = 1;
        }
        else
        {
            yield return sequence.Append(transform.DOLocalMoveX(1920, .5f)).SetEase(Ease.InSine).Join(canvasGroup.DOFade(0, .5f)).WaitForCompletion();
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}