using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Menu, PartyScreen, Bag, Cutscene, Paused }

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleSystem battleSystem;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private BagUI bagUI;

    [SerializeField] private GameObject cameras;

    private TrainerController trainer;
    private MenuController menuController;

    private GameState state;
    private GameState prevState;

    public SceneDetails CurrentScene { get; private set; }
    public SceneDetails PreviousScene { get; private set; }
    public GameState State => state;


    private void Awake()
    {
        Instance = this;

        menuController = GetComponent<MenuController>();

        TazoDB.Init();
        MoveDB.Init();
        ConditionsDB.Init();
        ItemDB.Init();
        QuestDB.Init();
    }

    // Start is called before the first frame update
    private void Start()
    {
        battleSystem.OnBattleOver += EndBattle;
        partyScreen.Init();

        DialogManager.Instance.OnShowDialog += () =>
        {
            prevState = state;
            state = GameState.Dialog;
        };
        DialogManager.Instance.OnDialogFinished += () =>
        {
            if (state == GameState.Dialog)
                state = prevState;
        };

        menuController.OnBack += () =>
        {
            state = GameState.FreeRoam;
        };

        menuController.OnMenuSelected += OnMenuSelected;
    }

    // Update is called once per frame
    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();

            if(Input.GetKeyDown(KeyCode.Return))
            {
                menuController.Open();
                state = GameState.Menu;
            }
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
        else if(state == GameState.Menu)
        {
            menuController.HandleUpdate();
        }
        else if(state == GameState.PartyScreen)
        {
            System.Action onBack = () =>
            {
                StartCoroutine(OnPartyBack());
            };
            partyScreen.HandleUpdate(OnPartySelected, onBack);
        }
        else if(state == GameState.Bag)
        {
            System.Action onBack = () =>
            {
                StartCoroutine(OnBagBack());
            };
            bagUI.HandleUpdate(onBack);
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
        StartCoroutine(trainer.TriggerTrainerBattle(playerController));
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
        var wildTazo = CurrentScene.GetComponent<MapArea>().GetRandomWildTazo();
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

    private void OnMenuSelected(int selectedItem)
    {
        if(selectedItem == 0)
        {
            //Tazopedia

        }
        else if(selectedItem == 1)
        {
            //Party
            partyScreen.gameObject.SetActive(true);
            partyScreen.Open();
            state = GameState.PartyScreen;
        }
        else if(selectedItem == 2)
        {
            //Bag
            bagUI.gameObject.SetActive(true);
            bagUI.Open();
            state = GameState.Bag;
        }
        else if(selectedItem == 3)
        {
            //Trainer Id
        }
        else if(selectedItem == 4)
        {
            //Map
        }
        else if(selectedItem == 5)
        {
            //TBD
        }
        else if(selectedItem == 6)
        {
            //Save Game
            SavingSystem.i.Save("SaveSlot1");
            state = GameState.FreeRoam;
        }
        else if(selectedItem == 7)
        {
            //Options
        }
        
    }

    private void OnPartySelected()
    {
        //Define
    }
    private IEnumerator OnPartyBack()
    {
        partyScreen.Close();
        yield return new WaitForEndOfFrame();
        state = GameState.FreeRoam;
    }

    private IEnumerator OnBagBack()
    {
        bagUI.Close();
        yield return new WaitForEndOfFrame();
        state = GameState.FreeRoam;
    }
}