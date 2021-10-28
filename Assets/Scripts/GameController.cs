using System.Collections;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog }

public class GameController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;

    private GameState state;

    [SerializeField] private GameObject cameras;

    private void Awake()
    {
        ConditionsDB.Init();
    }

    // Start is called before the first frame update
    private void Start()
    {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    private void StartBattle()
    {
        StartCoroutine(WildBattleTransition());
    }

    private void EndBattle(bool won)
    {
        StartCoroutine(EndBattleTransition());
    }

    private IEnumerator WildBattleTransition()
    {
        state = GameState.Battle;

        yield return Fader.Instance.StartFlash(.25f, 5, Color.white);
        yield return Fader.Instance.FadeIn(1f);

        battleSystem.gameObject.SetActive(true);
        cameras.SetActive(false);

        var playerParty = playerController.GetComponent<TazoParty>();
        var wildTazo = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildTazo();

        battleSystem.StartBattle(playerParty, wildTazo);
    }

    private IEnumerator EndBattleTransition()
    {
        yield return Fader.Instance.FadeIn(1f);

        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        cameras.SetActive(true);

        yield return new WaitForSeconds(.2f);

        yield return Fader.Instance.FadeOut(1f);
    }
}