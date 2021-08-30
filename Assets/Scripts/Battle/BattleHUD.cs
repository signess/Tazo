using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image genderIcon;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private CanvasGroup canvasGroup;
    public bool IsOn { get; set; }

    private Tazo _tazo;

    public void SetData(Tazo tazo)
    {
        _tazo = tazo;

        nameText.text = tazo.Base.Name;
        levelText.text = "Lvl. " + tazo.Level;
        hpBar.SetHP(tazo.HP, tazo.MaxHp);
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
    }

    public IEnumerator UpdateHP()
    {
        yield return hpBar.SetHPAsync(_tazo.HP);
    }

    public IEnumerator ShowBattleHUD(bool playerHUD)
    {
        canvasGroup.alpha = 1;
        if (playerHUD)
            yield return transform.DOLocalMoveX(-960, .5f).SetEase(Ease.InSine).WaitForCompletion();
        else
            yield return transform.DOLocalMoveX(960, .5f).SetEase(Ease.InSine).WaitForCompletion();
        IsOn = true;
    }

    public IEnumerator HideBattleHUD(bool playerHUD)
    {
        if (playerHUD)
            yield return transform.DOLocalMoveX(-1630, .5f).SetEase(Ease.OutSine).WaitForCompletion();
        else
            yield return transform.DOLocalMoveX(1615, .5f).SetEase(Ease.OutSine).WaitForCompletion();
        canvasGroup.alpha = 0;

        IsOn = false;
    }
}