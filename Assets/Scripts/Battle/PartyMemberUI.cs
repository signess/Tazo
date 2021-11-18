using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image tazoIcon;
    [SerializeField] private Image genderIcon;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private Image expBar;
    [SerializeField] private Image selectedFrame;

    private Tazo _tazo;

    public void Init(Tazo tazo)
    {
        _tazo = tazo;
        UpdateData();

        _tazo.OnHPChanged += UpdateData;
    }

    private void UpdateData()
    {
        nameText.text = _tazo.Base.Name;
        levelText.text = "Lvl. " + _tazo.Level;
        tazoIcon.sprite = _tazo.Base.IconSprite;
        hpBar.SetHP(_tazo.HP, _tazo.MaxHp);
        SetExp();
        switch (_tazo.Gender)
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

    public void SetSelected(bool selected)
    {
        if(selected)
        {
            selectedFrame.gameObject.SetActive(true);
        }
        else
        {
            selectedFrame.gameObject.SetActive(false);
        }
    }

    public void SetExp()
    {
        if (expBar == null) return;

        float normalizedExp = GetNormalizedExp();
        expBar.fillAmount = normalizedExp;
    }

    public float GetNormalizedExp()
    {
        int currLevelExp = _tazo.Base.GetExpForLevel(_tazo.Level);
        int nextLevelExp = _tazo.Base.GetExpForLevel(_tazo.Level + 1);

        float normalizedExp = (float)(_tazo.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
}