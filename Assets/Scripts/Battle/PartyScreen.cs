using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    private PartyMemberUI[] memberSlots;

    [SerializeField] private Image tazoTypeIcon;
    [SerializeField] private List<GameObject> movesBox;
    
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>();
    }

    public void SetPartyData(List<Tazo> tazos)
    {
        for(int i = 0; i < memberSlots.Length; i++)
        {
            if (i < tazos.Count)
                memberSlots[i].SetData(tazos[i]);
            else
                memberSlots[i].gameObject.SetActive(false);
        }
    }

    public void UpdateTazoSelection(int selectedMove)
    {
        for (int i = 0; i < movesBox.Count; i++)
        {
            if (i == selectedMove)
            {
                movesBox[i].transform.Find("Frame").gameObject.SetActive(true);
                //SetTazoDetails(_tazo);
            }
            else
            {
                movesBox[i].transform.Find("Frame").gameObject.SetActive(false);
            }
        }
    }

    public void SetTazoDetails(Tazo tazo)
    {
        //Type setting
        switch (tazo.Base.Type1)
        {
            case TazoType.Normal:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Normal;
                break;
            case TazoType.Fire:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Fire;
                break;
            case TazoType.Water:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Water;
                break;
            case TazoType.Grass:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Grass;
                break;
            case TazoType.Flying:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Flying;
                break;
            case TazoType.Fighting:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Fighting;
                break;
            case TazoType.Poison:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Poison;
                break;
            case TazoType.Electric:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Electric;
                break;
            case TazoType.Ground:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Ground;
                break;
            case TazoType.Rock:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Rock;
                break;
            case TazoType.Psychic:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Psychic;
                break;
            case TazoType.Ice:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Ice;
                break;
            case TazoType.Bug:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Bug;
                break;
            case TazoType.Ghost:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Ghost;
                break;
            case TazoType.Steel:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Steel;
                break;
            case TazoType.Dragon:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Dragon;
                break;
            case TazoType.Dark:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Dark;
                break;
            case TazoType.Fairy:
                tazoTypeIcon.sprite = GlobalSettings.Instance.Fairy;
                break;
        }

        //Moves Settings
        for (int i = 0; i < movesBox.Count; i++)
        {
            if (i < tazo.Moves.Count)
            {
                movesBox[i].transform.Find("Name Text").GetComponent<TextMeshProUGUI>().text = tazo.Moves[i].Base.Name;
                movesBox[i].transform.Find("EP Text").GetComponent<TextMeshProUGUI>().text = $"{tazo.Moves[i].EP}/{tazo.Moves[i].Base.EP}";
                if (tazo.Moves[i].EP > 0)
                {
                    movesBox[i].transform.Find("EP Text").GetComponent<TextMeshProUGUI>().color = Color.black;
                }
                else
                {
                    movesBox[i].transform.Find("EP Text").GetComponent<TextMeshProUGUI>().color = Color.red;
                }
                switch (tazo.Moves[i].Base.Type)
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
            }
        }

        //Ability Settings
    }
}
