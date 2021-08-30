using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver }

public class BattleSystem : MonoBehaviour
{
    //Player Variables
    [SerializeField] private BattleUnit playerUnit;

    //Enemy Variables
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox dialogBox;
    [SerializeField] private BattleSelectorBox selectorBox;

    [SerializeField] private PartyScreen partyScreen;

    private BattleState state;
    private int currentAction;
    private int currentMove;
    private int currentMember;

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

        enemyUnit.Setup(wildTazo);

        yield return Fader.Instance.FadeOut(1f);

        selectorBox.SetMoveNames(playerUnit.Tazo.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Tazo.Base.Name} appeared.");
        yield return enemyUnit.PlayWildEnterAnimation();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

        yield return dialogBox.TypeDialog($"Go {playerUnit.Tazo.Base.Name}.");
        yield return playerUnit.PlayEnterAnimation();

        yield return dialogBox.HideDialogBox();
        ActionSelection();
    }

    private void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        StartCoroutine(dialogBox.HideDialogBox());
        OnBattleOver(won);
    }

    private void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(selectorBox.ShowActionSelector());
        StartCoroutine(playerUnit.HUD.ShowBattleHUD(true));
        StartCoroutine(enemyUnit.HUD.ShowBattleHUD(false));
    }

    private void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Tazos);
        partyScreen.gameObject.SetActive(true);
    }

    private void MoveSelection()
    {
        selectorBox.SetMoveNames(playerUnit.Tazo.Moves);

        state = BattleState.MoveSelection;
        StartCoroutine(selectorBox.HideActionSelector());
        StartCoroutine(selectorBox.ShowMovesSelector());
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }

        if (state == BattleState.ActionSelection || state == BattleState.MoveSelection)
        {
            if (BattleCameraHandler.Instance.IdleTime < 5)
                BattleCameraHandler.Instance.IdleTime += Time.deltaTime;
            else if (BattleCameraHandler.Instance.IdleTime >= 5 && !BattleCameraHandler.Instance.dollyCartCamera)
            {
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
                MoveSelection();
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

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Tazo.Moves.Count - 1);

        selectorBox.UpdateMoveSelection(currentMove);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(PlayerMove());
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(selectorBox.HideMovesSelector());
            ActionSelection();
        }
    }

    private void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --currentMember;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Tazos.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = playerParty.Tazos[currentMember];
            if (selectedMember.HP <= 0)
            {
                StartCoroutine(partyScreen.ShowErrorDialog("You can't send out a fainted Tazo!"));
                return;
            }
            if (selectedMember == playerUnit.Tazo)
            {
                StartCoroutine(partyScreen.ShowErrorDialog("You can't switch with the same Tazo!"));
                return;
            }

            //Close party Screen and switch
            partyScreen.gameObject.SetActive(false);

            state = BattleState.Busy;
            StartCoroutine(SwitchTazo(selectedMember));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    private IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        //Switch to idle camera
        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        //Hide Both HUDS
        StartCoroutine(playerUnit.HUD.HideBattleHUD(true));
        yield return enemyUnit.HUD.HideBattleHUD(false);
        //Hide Move Selector
        yield return selectorBox.HideMovesSelector();

        var move = playerUnit.Tazo.Moves[currentMove];
        yield return RunMove(playerUnit, enemyUnit, move);

        if(state == BattleState.PerformMove)
            StartCoroutine(EnemyMove());
    }

    private IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Tazo.GetRandomMove();
        yield return RunMove(enemyUnit, playerUnit, move);

        if(state == BattleState.PerformMove)
        ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.EP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Tazo.Base.Name} used {move.Base.Name}.");
        yield return dialogBox.HideDialogBox();

        if (sourceUnit.IsPlayerUnit)
            yield return BattleCameraHandler.Instance.SwitchPlayerCamera();
        else if (!sourceUnit.IsPlayerUnit)
            yield return BattleCameraHandler.Instance.SwitchEnemyCamera();

        yield return sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        yield return targetUnit.PlayHitAnimation();
        //Use animation move

        if (targetUnit.IsPlayerUnit)
            yield return targetUnit.HUD.ShowBattleHUD(true);
        else
            yield return targetUnit.HUD.ShowBattleHUD(false);
        yield return new WaitForSeconds(.5f);

        var damageDetails = targetUnit.Tazo.TakeDamage(move, sourceUnit.Tazo);
        yield return targetUnit.HUD.UpdateHP();
        yield return new WaitForSeconds(1f);

        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            if (targetUnit.IsPlayerUnit)
                StartCoroutine(targetUnit.HUD.HideBattleHUD(true));
            else
                StartCoroutine(targetUnit.HUD.HideBattleHUD(false));
            yield return targetUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{targetUnit.Tazo.Base.Name} fainted.", true);
            yield return dialogBox.HideDialogBox();

            CheckForBattleOver(targetUnit);
        }
        if (targetUnit.IsPlayerUnit)
            yield return targetUnit.HUD.HideBattleHUD(true);
        else
            yield return targetUnit.HUD.HideBattleHUD(false);
        yield return new WaitForSeconds(.5f);
    }

    private void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextTazo = playerParty.GetHealthyTazo();
            if (nextTazo != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }
        }
        else
        {
            BattleOver(true);
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

    private IEnumerator SwitchTazo(Tazo newTazo)
    {
        //Switch to idle camera
        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        //Hide Action Selector
        yield return selectorBox.HideActionSelector();

        //Hide Both HUDS
        StartCoroutine(playerUnit.HUD.HideBattleHUD(true));
        yield return enemyUnit.HUD.HideBattleHUD(false);

        if (playerUnit.Tazo.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Tazo.Base.Name}.");
            yield return dialogBox.HideDialogBox();
            yield return playerUnit.PlayReturnAnimation();
            yield return new WaitForSeconds(1f);
        }

        playerUnit.Setup(newTazo);

        selectorBox.SetMoveNames(newTazo.Moves);

        yield return dialogBox.TypeDialog($"Go {newTazo.Base.Name}.");
        yield return dialogBox.HideDialogBox();
        StartCoroutine(playerUnit.HUD.ShowBattleHUD(true));
        yield return playerUnit.PlayEnterAnimation();

        yield return dialogBox.HideDialogBox();

        yield return playerUnit.HUD.HideBattleHUD(true);

        StartCoroutine(EnemyMove());
    }
}