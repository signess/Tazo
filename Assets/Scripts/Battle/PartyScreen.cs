using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : MonoBehaviour
{
    private PartyMemberUI[] memberSlots;
    private List<Tazo> tazos;
    private TazoParty party;

    [SerializeField] private Image tazoTypeIcon;
    [SerializeField] private List<GameObject> movesBox;
    [SerializeField] private GameObject errorDialog;
    private CanvasGroup canvasGroup;

    public BattleState? CalledFrom { get; set; }
    private int selection = 0;

    public Tazo SelectedMember { get => tazos[selection]; }

    public void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        party = TazoParty.GetPlayerParty();
        SetPartyData();
        party.OnUpdate += SetPartyData;
    }

    public void SetPartyData()
    {
        Debug.LogWarning("Party data set!");
        tazos = party.Tazos;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < tazos.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(tazos[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        UpdateMemberSelection(selection);
    }

    public void HandleUpdate(Action onSelected, Action onBack)
    {
        var prevSelection = selection;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selection;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selection;

        selection = Mathf.Clamp(selection, 0, tazos.Count - 1);

        if(selection != prevSelection)
            UpdateMemberSelection(selection);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            onSelected?.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            onBack?.Invoke();
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < tazos.Count; i++)
        {
            if (i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
                Debug.Log($"Tazo: {tazos[i].Base.Name}");
                SetTazoDetails(tazos[i]);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }
        }
    }

    public IEnumerator ShowErrorDialog(string text)
    {
        if (GameObject.Find("ErrorDialog") == null)
        {
            var newErrorDialog = Instantiate(errorDialog, memberSlots[selection].transform.position + new Vector3(100, 0, 0), Quaternion.identity);
            newErrorDialog.transform.SetParent(transform);
            newErrorDialog.name = "ErrorDialog";
            newErrorDialog.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = text;
            yield return newErrorDialog.transform.DOScaleX(1, .2f).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(2f);
            yield return newErrorDialog.transform.DOScaleX(0, .2f).SetEase(Ease.Linear).WaitForCompletion();
            Destroy(newErrorDialog);
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
                movesBox[i].transform.Find("Icon").GetComponent<Image>().sprite = GlobalSettings.Instance.None;
            }
        }

        //Ability Settings
    }

    public void Open()
    {
        StartCoroutine(AnimatePartyScreen(true));
    }

    public void Close()
    {
        StartCoroutine(AnimatePartyScreen(false));
    }

    private IEnumerator AnimatePartyScreen(bool enabled)
    {
        var sequence = DOTween.Sequence();
        if (enabled)
            yield return sequence.Append(transform.DOLocalMoveX(0, .5f)).SetEase(Ease.InOutCubic).Join(canvasGroup.DOFade(1, .5f)).WaitForCompletion();
        else
        {
            yield return sequence.Append(transform.DOLocalMoveX(1920, .5f)).SetEase(Ease.InOutCubic).Join(canvasGroup.DOFade(0, .5f)).WaitForCompletion();
            gameObject.SetActive(false);
        }
    }
}