using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    //Player Variables
    [SerializeField] private BattleUnit playerUnit;

    [SerializeField] private BattleHUD playerHUD;

    //Enemy Variables
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox dialogBox;
    [SerializeField] private BattleSelectorBox selectorBox;

    [SerializeField] private PartyScreen partyScreen;

    private BattleState state;
    private int currentAction;
    private int currentMove;

    public event Action<bool> OnBattleOver;

    private TazoParty playerParty;
    private Tazo wildTazo;

    public void StartBattle(TazoParty playerParty, Tazo wildTazo)
    {
        this.playerParty = playerParty;
        this.wildTazo = wildTazo;
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        partyScreen.Init();

        playerUnit.Setup(playerParty.GetHealthyTazo());
        playerHUD.SetData(playerUnit.Tazo);

        enemyUnit.Setup(wildTazo);
        enemyHUD.SetData(enemyUnit.Tazo);

        yield return Fader.Instance.FadeOut(1f);

        selectorBox.SetMoveNames(playerUnit.Tazo.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Tazo.Base.Name} appeared.");
        yield return enemyUnit.PlayWildEnterAnimation();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        yield return dialogBox.TypeDialog($"Go {playerUnit.Tazo.Base.Name}.");
        yield return playerUnit.PlayEnterAnimation();

        yield return dialogBox.HideDialogBox();
        PlayerAction();
    }

    private void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(selectorBox.ShowActionSelector());
        if(!playerHUD.IsOn)
         StartCoroutine(playerHUD.ShowBattleHUD(true));
        if(!enemyHUD.IsOn)
            StartCoroutine(enemyHUD.ShowBattleHUD(false));
    }

    private void OpenPartyScreen()
    {
        partyScreen.SetPartyData(playerParty.Tazos);
        partyScreen.gameObject.SetActive(true);
    }

    private void PlayerMove()
    {
        selectorBox.SetMoveNames(playerUnit.Tazo.Moves);

        state = BattleState.PlayerMove;
        StartCoroutine(selectorBox.HideActionSelector());
        StartCoroutine(selectorBox.ShowMovesSelector());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }

        if (state == BattleState.PlayerAction || state == BattleState.PlayerMove)
        {
            if (BattleCameraHandler.Instance.IdleTime < 5)
                BattleCameraHandler.Instance.IdleTime += Time.deltaTime;
            else if (BattleCameraHandler.Instance.IdleTime >= 5)
            {
                if (!BattleCameraHandler.Instance.dollyCartCamera)
                    StartCoroutine(BattleCameraHandler.Instance.SwitchDollyCartCamera());
            }
        }
    }

    private void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentAction;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        selectorBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //Fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //Party
                OpenPartyScreen();
            }
            else if (currentAction == 2)
            {
                //Bag
            }
            else if (currentAction == 3)
            {
                //Run
            }
        }
    }

    private void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentMove;

        currentAction = Mathf.Clamp(currentAction, 0, playerUnit.Tazo.Moves.Count - 1);

        selectorBox.UpdateMoveSelection(currentMove);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(PerformPlayerMove());
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(selectorBox.HideMovesSelector());
            PlayerAction();
        }
    }

    private IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        //Switch to idle camera
        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        //Hide Both HUDS
        StartCoroutine(playerHUD.HideBattleHUD(true));
        yield return enemyHUD.HideBattleHUD(false);
        //Hide Move Selector
        yield return selectorBox.HideMovesSelector();

        var move = playerUnit.Tazo.Moves[currentMove];
        move.EP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} used {move.Base.Name}.");
        yield return dialogBox.HideDialogBox();

        yield return BattleCameraHandler.Instance.SwitchPlayerCamera();

        yield return playerUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(1f);

        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        yield return enemyUnit.PlayHitAnimation();
        //Use animation move

        yield return enemyHUD.ShowBattleHUD(false);
        yield return new WaitForSeconds(.5f);

        var damageDetails = enemyUnit.Tazo.TakeDamage(move, playerUnit.Tazo);
        yield return enemyHUD.UpdateHP();

        yield return new WaitForSeconds(1f);
        yield return enemyHUD.HideBattleHUD(false);

        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return enemyUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{enemyUnit.Tazo.Base.Name} fainted.");

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
            yield return dialogBox.HideDialogBox();
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    private IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Tazo.GetRandomMove();
        move.EP--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Tazo.Base.Name} used {move.Base.Name}.");
        yield return dialogBox.HideDialogBox();

        yield return BattleCameraHandler.Instance.SwitchEnemyCamera();

        yield return enemyUnit.PlayerAttackAnimation();
        yield return new WaitForSeconds(1f);

        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        yield return playerUnit.PlayHitAnimation();
        //Use animation move

        yield return playerHUD.ShowBattleHUD(true);
        yield return new WaitForSeconds(.5f);

        var damageDetails = playerUnit.Tazo.TakeDamage(move, enemyUnit.Tazo);
        yield return playerHUD.UpdateHP();

        yield return new WaitForSeconds(1f);
        yield return playerHUD.HideBattleHUD(true);

        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return playerUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} fainted.");

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

            var nextTazo = playerParty.GetHealthyTazo();
            if (nextTazo != null)
            {
                playerUnit.Setup(nextTazo);
                playerHUD.SetData(nextTazo);

                selectorBox.SetMoveNames(nextTazo.Moves);

                yield return dialogBox.TypeDialog($"Go {nextTazo.Base.Name}.");
                yield return playerUnit.PlayEnterAnimation();

                yield return dialogBox.HideDialogBox();
                PlayerAction();
            }
            else
            {
                yield return dialogBox.HideDialogBox();
                OnBattleOver(false);
            }
        }
        else
        {
            yield return dialogBox.HideDialogBox();
            PlayerAction();
        }
    }

    private IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");

        yield return dialogBox.HideDialogBox();
    }
}