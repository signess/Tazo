using System;
using System.Collections;
using UnityEngine;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, BattleOver }
public enum BattleAction { Move, SwitchTazo, UseItem, Run }

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
        playerParty.Tazos.ForEach(t => t.OnBattleOver());
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
            StartCoroutine(RunTurns(BattleAction.Move));
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
            StartCoroutine(RunTurns(BattleAction.SwitchTazo));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            partyScreen.gameObject.SetActive(false);
            ActionSelection();
        }
    }

    private IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        if (playerAction == BattleAction.Move)
        {
            playerUnit.Tazo.CurrentMove = playerUnit.Tazo.Moves[currentMove];
            enemyUnit.Tazo.CurrentMove = enemyUnit.Tazo.GetRandomMove();

            int playerMovePriority = playerUnit.Tazo.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Tazo.CurrentMove.Base.Priority;

            //Check who goes first
            bool playerGoesFirst = true;
            if (enemyMovePriority > playerMovePriority)
                playerGoesFirst = false;
            else if (enemyMovePriority == playerMovePriority)
                playerGoesFirst = playerUnit.Tazo.Speed >= enemyUnit.Tazo.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondMonster = secondUnit.Tazo;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Tazo.CurrentMove);
            //yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondMonster.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Tazo.CurrentMove);
                //yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchTazo)
            {
                var selectedTazo = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchTazo(selectedTazo);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                yield return selectorBox.HideActionSelector();
                //yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return selectorBox.HideActionSelector();
                //yield return TryToEscape();
            }

            //EnemyTurn
            var enemyMove = enemyUnit.Tazo.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            //yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;

        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        //Switch to idle camera
        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        //Hide Both HUDS
        StartCoroutine(playerUnit.HUD.HideBattleHUD(true));
        StartCoroutine(enemyUnit.HUD.HideBattleHUD(false));
        //Hide Move Selector
        yield return selectorBox.HideMovesSelector();

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

        StartCoroutine(ShowHUDS(targetUnit));
        yield return targetUnit.PlayHitAnimation();

        //Use animation move

        yield return new WaitForSeconds(.5f);

        if (move.Base.Category == MoveCategory.Status)
        {
            yield return HideHUDS(targetUnit);
            yield return RunMoveEffects(move.Base.Effects, sourceUnit.Tazo, targetUnit.Tazo, move.Base.Target);
        }
        else
        {

            var damageDetails = targetUnit.Tazo.TakeDamage(move, sourceUnit.Tazo);
            yield return targetUnit.HUD.UpdateHP();
            yield return new WaitForSeconds(1f);
            yield return HideHUDS(targetUnit);
            yield return ShowDamageDetails(damageDetails);

        }

        if (targetUnit.Tazo.HP <= 0)
        {
            yield return HideHUDS(targetUnit);
            yield return targetUnit.PlayFaintAnimation();
            yield return dialogBox.TypeDialog($"{targetUnit.Tazo.Base.Name} fainted.", true);
            yield return dialogBox.HideDialogBox();

            CheckForBattleOver(targetUnit);
        }
        yield return HideHUDS(targetUnit);
        yield return new WaitForSeconds(.5f);
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, Tazo sourceUnit, Tazo targetUnit, MoveTarget moveTarget)
    {
        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                sourceUnit.ApplyBoost(effects.Boosts);
            else
                targetUnit.ApplyBoost(effects.Boosts);
        }
        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            targetUnit.SetStatus(effects.Status);
        }

        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            targetUnit.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(sourceUnit);
        yield return ShowStatusChanges(targetUnit);
    }

    private IEnumerator ShowStatusChanges(Tazo tazo)
    {
        while (tazo.StatusChanges.Count > 0)
        {
            var message = tazo.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message, true);
            yield return dialogBox.HideDialogBox();
        }
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
            yield return dialogBox.TypeDialog("A critical hit!", true);

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!", true);
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!", true);

        yield return dialogBox.HideDialogBox();
    }

    private IEnumerator SwitchTazo(Tazo newTazo)
    {
        //Switch to idle camera
        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        //Hide Both HUDS
        StartCoroutine(playerUnit.HUD.HideBattleHUD(true));
        StartCoroutine(enemyUnit.HUD.HideBattleHUD(false));

        //Hide Action Selector
        yield return selectorBox.HideActionSelector();

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

        yield return playerUnit.HUD.HideBattleHUD(true);

        state = BattleState.RunningTurn;
    }

    private IEnumerator HideHUDS(BattleUnit targetUnit)
    {
        if (targetUnit.IsPlayerUnit)
            yield return targetUnit.HUD.HideBattleHUD(true);
        else
            yield return targetUnit.HUD.HideBattleHUD(false);
    }

    private IEnumerator ShowHUDS(BattleUnit targetUnit)
    {
        if (targetUnit.IsPlayerUnit)
            yield return targetUnit.HUD.ShowBattleHUD(true);
        else
            yield return targetUnit.HUD.ShowBattleHUD(false);
    }
}