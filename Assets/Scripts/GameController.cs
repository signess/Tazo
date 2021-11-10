using System.Collections;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene, Paused }

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;

    [SerializeField] private GameObject cameras;

    private GameState state;
    private GameState prevState;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }

    private TrainerController trainer;

    private void Awake()
    {
        ConditionsDB.Init();
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
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

    public void PauseGame(bool pause)
    {
        if(pause)
        {
            prevState = state;
            state = GameState.Paused;
        }
        else
        {
            state = prevState;
        }
    }

    public void StartBattle()
    {
        StartCoroutine(WildBattleTransition());
    }

    public void StartTrainerBattle(TrainerController trainer)
    {
        this.trainer = trainer;
        StartCoroutine(TrainerBattleTransition(trainer));
    }

    public void OnEnterTrainerView(TrainerController trainer)
    {
        state = GameState.Cutscene;
        trainer.TriggerTrainerBattle(playerController).GetAwaiter();
    }

    private void EndBattle(bool won)
    {
        if(trainer != null && won)
        {
            trainer.BattleLost();
            trainer = null;
        }
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
        var wildTazoCopy = new Tazo(wildTazo.Base, wildTazo.Level);

        battleSystem.StartWildBattle(playerParty, wildTazoCopy);
    }

    private IEnumerator TrainerBattleTransition(TrainerController trainer)
    {
        state = GameState.Battle;

        yield return Fader.Instance.StartFlash(.25f, 5, Color.white);
        yield return Fader.Instance.FadeIn(1f);

        battleSystem.gameObject.SetActive(true);
        cameras.SetActive(false);

        var playerParty = playerController.GetComponent<TazoParty>();
        var trainerParty = trainer.GetComponent<TazoParty>();

        battleSystem.StartTrainerBattle(playerParty, trainerParty);
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

    public void SetCurrentScene(SceneDetails currScene)
    {
        PreviousScene = CurrentScene;
        CurrentScene = currScene;
    }
}