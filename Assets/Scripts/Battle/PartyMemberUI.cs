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
    [SerializeField] private Image selectedFrame;

    private Tazo _tazo;

    public void SetData(Tazo tazo)
    {
        _tazo = tazo;

        nameText.text = tazo.Base.Name;
        levelText.text = "Lvl. " + tazo.Level;
        tazoIcon.sprite = tazo.Base.IconSprite;
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
}