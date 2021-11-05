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
    [SerializeField] private Image statusIcon;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private Image expBar;
    [SerializeField] private CanvasGroup canvasGroup;
    public bool IsOn { get; set; }

    private Tazo _tazo;

    public void SetData(Tazo tazo)
    {
        _tazo = tazo;

        nameText.text = tazo.Base.Name;
        hpBar.SetHP(tazo.HP, tazo.MaxHp);
        SetLevel();
        SetExp();
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
        SetStatusIcon();
        _tazo.OnStatusChanged += SetStatusIcon;
    }

    private void SetStatusIcon()
    {
        if (_tazo.Status == null)
            statusIcon.sprite = null;
        else
        {
            statusIcon.sprite = _tazo.Status.Icon;
        }
    }

    public IEnumerator UpdateHP()
    {
        if (_tazo.HpChanged)
        {
            yield return hpBar.SetHPAsync(_tazo.HP);
            _tazo.HpChanged = false;
        }
    }

    public void SetLevel()
    {
        levelText.text = "Lvl. " + _tazo.Level;
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.fillAmount = normalizedExp;
    }

    public IEnumerator SetExpAsync(bool reset = false)
    {
        if (expBar == null) yield break;

        if(reset)
            expBar.fillAmount = 0;

        float normalizedExp = GetNormalizedExp();
        yield return expBar.DOFillAmount(normalizedExp, 1.5f).WaitForCompletion();
    }

    public float GetNormalizedExp()
    {
        int currLevelExp = _tazo.Base.GetExpForLevel(_tazo.Level);
        int nextLevelExp = _tazo.Base.GetExpForLevel(_tazo.Level + 1);

        float normalizedExp = (float) (_tazo.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public IEnumerator ShowBattleHUD(bool playerHUD)
    {
        if (IsOn)
            yield break;

        canvasGroup.alpha = 1;
        IsOn = true;
        if (playerHUD)
            yield return transform.DOLocalMoveX(-960, .5f).SetEase(Ease.InSine).WaitForCompletion();
        else
            yield return transform.DOLocalMoveX(960, .5f).SetEase(Ease.InSine).WaitForCompletion();
    }

    public IEnumerator HideBattleHUD(bool playerHUD)
    {
        if (!IsOn)
            yield break;

        if (playerHUD)
            yield return transform.DOLocalMoveX(-1630, .5f).SetEase(Ease.OutSine).WaitForCompletion();
        else
            yield return transform.DOLocalMoveX(1615, .5f).SetEase(Ease.OutSine).WaitForCompletion();
        canvasGroup.alpha = 0;
        IsOn = false;

    }
}