using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{ Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, AboutToUse, MoveToForget, BattleOver }

public enum BattleAction
{ Move, SwitchTazo, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [Header("Player Variables")]
    [SerializeField] private BattleUnit playerUnit;

    [SerializeField] private BattleTrainer playerTrainer;

    [Header("Enemy Variables")]
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleTrainer enemyTrainer;

    [Header("UI Variables")]
    [SerializeField] private BattleDialogBox dialogBox;

    [SerializeField] private BattleSelectorBox selectorBox;
    [SerializeField] private PartyScreen partyScreen;
    [SerializeField] private MoveSelectionUI moveSelectionUI;

    [Header("MISC Variables")]
    [SerializeField] private GameObject tazoCatcher;

    private BattleState state;
    private int currentAction;
    private int currentMove;
    private bool aboutToUseChoice = true;

    public event Action<bool> OnBattleOver;

    private TazoParty playerParty;
    private TazoParty trainerParty;
    private Tazo wildTazo;

    private bool isTrainerBattle;
    private PlayerController player;
    private TrainerController trainer;

    private int escapeAttempts;
    private MoveBase moveToLearn;

    public void StartWildBattle(TazoParty playerParty, Tazo wildTazo)
    {
        this.playerParty = playerParty;
        this.wildTazo = wildTazo;
        isTrainerBattle = false;

        player = playerParty.GetComponent<PlayerController>();

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(TazoParty playerParty, TazoParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        isTrainerBattle = true;

        player = playerParty.GetComponent<PlayerController>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        yield return BattleCameraHandler.Instance.SwitchEnemyCamera();
        playerUnit.Clear();
        enemyUnit.Clear();

        partyScreen.Init();
        escapeAttempts = 0;

        playerUnit.gameObject.SetActive(false);
        playerTrainer.gameObject.SetActive(true);
        playerTrainer.TrainerSprite.sprite = player.Sprite;

        if (!isTrainerBattle)
        {
            //Wild Pokemon Battle

            enemyUnit.Setup(wildTazo);

            yield return Fader.Instance.FadeOut(1f);

            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Tazo.Base.Name} appeared.");
            yield return enemyUnit.PlayWildEnterAnimation();
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
        }
        else
        {
            //Trainer Battle
            enemyUnit.gameObject.SetActive(false);

            enemyTrainer.gameObject.SetActive(true);

            enemyTrainer.TrainerSprite.sprite = trainer.Sprite;

            yield return Fader.Instance.FadeOut(1f);

            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle!");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));

            var enemyTazo = trainerParty.GetHealthyTazo();
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {enemyTazo.Base.Name}.");
            yield return enemyTrainer.PlayExitAnimation();
            enemyTrainer.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            enemyUnit.Setup(enemyTazo);
        }
        yield return BattleCameraHandler.Instance.SwitchPlayerTrainerCamera();

        //setup player pokemon
        playerUnit.gameObject.SetActive(true);
        playerUnit.Setup(playerParty.GetHealthyTazo());
        yield return dialogBox.TypeDialog($"Go {playerUnit.Tazo.Base.Name}.");
        yield return BattleCameraHandler.Instance.SwitchGroupCamera();
        playerTrainer.gameObject.SetActive(false);
        selectorBox.SetMoveNames(playerUnit.Tazo.Moves);

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
        partyScreen.CalledFrom = state;
        state = BattleState.PartyScreen;
        partyScreen.gameObject.SetActive(true);
        partyScreen.Open();
    }

    private void MoveSelection()
    {
        selectorBox.SetMoveNames(playerUnit.Tazo.Moves);

        state = BattleState.MoveSelection;
        StartCoroutine(selectorBox.HideActionSelector());
        StartCoroutine(selectorBox.ShowMovesSelector());
    }

    private IEnumerator AboutToUse(Tazo newTazo)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to use {newTazo.Base.Name}. Do you want to switch Tazo?");

        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }

    private IEnumerator ChooseMoveToForget(Tazo tazo, MoveBase newMove)
    {
        state = BattleState.Busy;
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetTazoData(tazo, newMove);
        yield return moveSelectionUI.EnableMoveSelectionUI(true);
        moveToLearn = newMove;

        state = BattleState.MoveToForget;
    }

    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
            BattleCameraHandler.Instance.CheckForDynamicCamera();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
            BattleCameraHandler.Instance.CheckForDynamicCamera();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
            BattleCameraHandler.Instance.CheckForDynamicCamera();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
        else if (state == BattleState.MoveToForget)
        {
            Action<int> onMoveSelected = (moveIndex) =>
            {
                StartCoroutine(moveSelectionUI.EnableMoveSelectionUI(false));
                if (moveIndex == 4)
                {
                    // dont learn any moves
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} did not learn {moveToLearn.Name}.", true));
                }
                else
                {
                    //forget the selected move
                    var selectedMove = playerUnit.Tazo.Moves[moveIndex].Base;
                    StartCoroutine(dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}.", true));
                    playerUnit.Tazo.Moves[moveIndex] = new Move(moveToLearn);
                }
                moveToLearn = null;
                state = BattleState.RunningTurn;
            };
            moveSelectionUI.HandleMoveSelection(onMoveSelected);
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
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 3)
            {
                //Run
                StartCoroutine(RunTurns(BattleAction.Run));
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
            var move = playerUnit.Tazo.Moves[currentMove];
            if (move.EP == 0) return;
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
        partyScreen.HandleUpdate(PartyOnSelected, PartyOnBack);
    }

    private void PartyOnSelected()
    {
        var selectedMember = partyScreen.SelectedMember;
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
        partyScreen.Close();

        if (partyScreen.CalledFrom == BattleState.ActionSelection)
        {
            StartCoroutine(RunTurns(BattleAction.SwitchTazo));
        }
        else
        {
            state = BattleState.Busy;
            bool isTrainerAboutToUse = partyScreen.CalledFrom == BattleState.AboutToUse;
            StartCoroutine(SwitchTazo(selectedMember, isTrainerAboutToUse));
        }
        partyScreen.CalledFrom = null;
    }

    private void PartyOnBack()
    {
        if (playerUnit.Tazo.HP <= 0)
        {
            StartCoroutine(partyScreen.ShowErrorDialog("You must choose a Tazo to continue battle!"));
            return;
        }

        partyScreen.Close();

        if (partyScreen.CalledFrom == BattleState.AboutToUse)
        {
            StartCoroutine(SendNextTrainerTazo());
        }
        else
            ActionSelection();
        partyScreen.CalledFrom = null;
    }

private void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
            aboutToUseChoice = !aboutToUseChoice;

        dialogBox.UpdateChoiceBox(aboutToUseChoice);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if (aboutToUseChoice)
            {
                //Yes Choice
                OpenPartyScreen();
            }
            else
            {
                //No Choice
                StartCoroutine(SendNextTrainerTazo());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerTazo());
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

            var secondTazo = secondUnit.Tazo;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Tazo.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondTazo.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Tazo.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            yield return OrganizeBattleFeed();
            if (playerAction == BattleAction.SwitchTazo)
            {
                var selectedTazo = partyScreen.SelectedMember;
                state = BattleState.Busy;
                yield return SwitchTazo(selectedTazo);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                yield return ThrowTazoCatcher();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            //EnemyTurn
            var enemyMove = enemyUnit.Tazo.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    private IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        yield return OrganizeBattleFeed();

        //Check status before run move
        bool canRunMove = sourceUnit.Tazo.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Tazo, sourceUnit);
            StartCoroutine(ShowHUDS(sourceUnit));
            yield return sourceUnit.HUD.UpdateHP();
            yield return new WaitForSeconds(.5f);
            yield return HideHUDS(sourceUnit);
            yield return new WaitForSeconds(.5f);
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Tazo, sourceUnit);

        move.EP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Tazo.Base.Name} used {move.Base.Name}.");

        //Check accuracy
        if (CheckIfMoveHits(move, sourceUnit.Tazo, targetUnit.Tazo))
        {
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
                yield return RunMoveEffects(move.Base.Effects, sourceUnit, targetUnit, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Tazo.TakeDamage(move, sourceUnit.Tazo);
                yield return targetUnit.HUD.UpdateHP();
                yield return new WaitForSeconds(1f);
                yield return HideHUDS(targetUnit);
                yield return ShowDamageDetails(damageDetails);
            }

            if (move.Base.SecondaryEffects != null && move.Base.SecondaryEffects.Count > 0 && targetUnit.Tazo.HP > 0)
            {
                foreach (var secondary in move.Base.SecondaryEffects)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffects(secondary, sourceUnit, targetUnit, secondary.Target);
                }
            }

            if (targetUnit.Tazo.HP <= 0)
            {
                yield return HandleTazoFainted(targetUnit);
            }

            yield return HideHUDS(targetUnit);
            yield return new WaitForSeconds(.5f);
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Tazo.Base.Name}'s move missed!");
        }
    }

    private IEnumerator RunMoveEffects(MoveEffects effects, BattleUnit sourceUnit, BattleUnit targetUnit, MoveTarget moveTarget)
    {
        //Stat Boosting
        if (effects.Boosts != null)
        {
            if (moveTarget == MoveTarget.Self)
                sourceUnit.Tazo.ApplyBoost(effects.Boosts);
            else
                targetUnit.Tazo.ApplyBoost(effects.Boosts);
        }

        //Status Condition
        if (effects.Status != ConditionID.none)
        {
            targetUnit.Tazo.SetStatus(effects.Status);
        }

        //Volatile Status Condition
        if (effects.VolatileStatus != ConditionID.none)
        {
            targetUnit.Tazo.SetVolatileStatus(effects.VolatileStatus);
        }

        yield return ShowStatusChanges(sourceUnit.Tazo, sourceUnit);
        yield return ShowStatusChanges(targetUnit.Tazo, targetUnit);
    }

    private IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);

        //Status like burn or psn will hurt the pokemon after the turn
        sourceUnit.Tazo.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Tazo, sourceUnit);

        if (sourceUnit.Tazo.HP <= 0)
        {
            yield return HandleTazoFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }

        yield return new WaitForSeconds(.5f);
    }

    private bool CheckIfMoveHits(Move move, Tazo source, Tazo target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f / 3f, 5f / 3f, 2f, 7f / 3f, 8f / 3f, 3f };
        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }

    private IEnumerator ShowStatusChanges(Tazo tazo, BattleUnit sourceUnit)
    {
        while (tazo.StatusChanges.Count > 0)
        {
            var message = tazo.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message, true);
            //yield return dialogBox.HideDialogBox();

            if (tazo.Status != null)
            {
                if (tazo.Status.ID == ConditionID.psn || tazo.Status.ID == ConditionID.brn)
                {
                    yield return ShowHUDS(sourceUnit);
                    yield return new WaitForSeconds(.5f);

                    yield return sourceUnit.HUD.UpdateHP();

                    yield return new WaitForSeconds(1f);
                    yield return HideHUDS(sourceUnit);
                }
            }
        }
    }

    private IEnumerator HandleTazoFainted(BattleUnit faintedUnit)
    {
        yield return HideHUDS(faintedUnit);
        yield return faintedUnit.PlayFaintAnimation();
        yield return dialogBox.TypeDialog($"{faintedUnit.Tazo.Base.Name} fainted.", true);

        if (!faintedUnit.IsPlayerUnit)
        {
            //exp gain
            int expYield = faintedUnit.Tazo.Base.ExpYield;
            int enemyLevel = faintedUnit.Tazo.Level;
            float trainerBonus = (isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * trainerBonus) / 7);
            playerUnit.Tazo.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} gained {expGain} experience points.", true);
            yield return ShowHUDS(playerUnit);
            yield return playerUnit.HUD.SetExpAsync();

            //check level up
            while (playerUnit.Tazo.CheckForLevelUp())
            {
                playerUnit.HUD.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} grew to level {playerUnit.Tazo.Level}!", true);

                //Try learn a new move
                var newMove = playerUnit.Tazo.GetLearnableMoveAtCurrentLevel();
                if (newMove != null)
                {
                    if (playerUnit.Tazo.Moves.Count < 4)
                    {
                        playerUnit.Tazo.LearnMove(newMove);
                        yield return dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} learned {newMove.Base.Name}!", true);
                        selectorBox.SetMoveNames(playerUnit.Tazo.Moves);
                    }
                    else
                    {
                        //forget a move
                        yield return dialogBox.TypeDialog($"{playerUnit.Tazo.Base.Name} wants to learn {newMove.Base.Name}!");
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                        yield return dialogBox.TypeDialog($"But it cannot learn more than four moves.");
                        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Z));
                        yield return dialogBox.TypeDialog($"Do you want to forget any move to learn {newMove.Base.Name}?", true);
                        yield return ChooseMoveToForget(playerUnit.Tazo, newMove.Base);
                        yield return new WaitUntil(() => state != BattleState.MoveToForget);
                        yield return new WaitForSeconds(2f);
                    }
                }

                yield return playerUnit.HUD.SetExpAsync(true);
            }
            yield return HideHUDS(playerUnit);
            yield return new WaitForEndOfFrame();
        }
        yield return dialogBox.HideDialogBox();
        CheckForBattleOver(faintedUnit);
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
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextTazo = trainerParty.GetHealthyTazo();
                if (nextTazo != null)
                {
                    StartCoroutine(AboutToUse(nextTazo));
                }
                else
                    BattleOver(true);
            }
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

    private IEnumerator SwitchTazo(Tazo newTazo, bool isTrainerAboutToUse = false)
    {
        yield return OrganizeBattleFeed();

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

        if (isTrainerAboutToUse)
            yield return SendNextTrainerTazo();
        else
            state = BattleState.RunningTurn;
    }

    private IEnumerator SendNextTrainerTazo()
    {
        state = BattleState.Busy;
        //Switch to idle camera
        yield return BattleCameraHandler.Instance.SwitchGroupCamera();

        //Hide Both HUDS
        StartCoroutine(playerUnit.HUD.HideBattleHUD(true));
        StartCoroutine(enemyUnit.HUD.HideBattleHUD(false));

        //Hide Action Selector
        yield return selectorBox.HideActionSelector();

        var nextTazo = trainerParty.GetHealthyTazo();
        enemyUnit.Setup(nextTazo, true);

        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextTazo.Base.Name}.");
        yield return dialogBox.HideDialogBox();
        StartCoroutine(enemyUnit.HUD.ShowBattleHUD(false));
        yield return enemyUnit.PlayEnterAnimation();

        yield return new WaitForSeconds(.5f);

        yield return enemyUnit.HUD.HideBattleHUD(false);

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

    private IEnumerator OrganizeBattleFeed()
    {
        //Switch to idle camera
        var cameraSwitch = BattleCameraHandler.Instance.SwitchGroupCamera().GetAwaiter();
        yield return cameraSwitch;

        //Hide Both HUDS
        StartCoroutine(playerUnit.HUD.HideBattleHUD(true));
        StartCoroutine(enemyUnit.HUD.HideBattleHUD(false));

        //Hide Action Selector
        StartCoroutine(selectorBox.HideActionSelector());
        //Hide Move Selector
        yield return selectorBox.HideMovesSelector();
    }

    private IEnumerator ThrowTazoCatcher()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal another trainer's Tazo!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used TAZOCATCHER!");

        var tazoCatcherObj = Instantiate(tazoCatcher, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var tazoCatcherSprite = tazoCatcherObj.GetComponent<SpriteRenderer>();

        //Animations
        var throwSequence = DOTween.Sequence();
        yield return throwSequence.Append(tazoCatcherSprite.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f))
            .Join(tazoCatcherSprite.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360))
            .Join(tazoCatcherSprite.transform.DOScale(new Vector3(.3f, .3f, .3f), 1f))
            .Append(tazoCatcherSprite.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 1f, 1, .5f)).SetEase(Ease.OutCubic)
            .WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();

        yield return tazoCatcherSprite.transform.DOMoveY(enemyUnit.transform.position.y - .5f, .5f).SetEase(Ease.OutBounce).WaitForCompletion();

        yield return new WaitForSeconds(.5f);

        int shakeCount = TryToCatchTazo(enemyUnit.Tazo);
        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(1f);
            tazoCatcherSprite.transform.DOPunchRotation(new Vector3(0, 0, 10f), .8f).WaitForCompletion();
        }
        if (shakeCount == 4)
        {
            yield return new WaitForSeconds(1f);
            //Tazo caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Tazo.Base.Name} was caught!", true);
            yield return tazoCatcherSprite.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddTazo(enemyUnit.Tazo);

            Destroy(tazoCatcherObj);
            BattleOver(true);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
            //Tazo broke out
            tazoCatcherSprite.DOFade(0, .2f);
            enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Tazo.Base.Name} broke free!");
            else
                yield return dialogBox.TypeDialog("So close! Almost caught it.");
            Destroy(tazoCatcherObj);

            state = BattleState.RunningTurn;
        }
    }

    private int TryToCatchTazo(Tazo tazo)
    {
        float a = (3 * tazo.MaxHp - 2 * tazo.HP) * tazo.Base.CatchRate * ConditionsDB.GetStatusBonus(tazo.Status) / (3 * tazo.MaxHp);
        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;
            shakeCount++;
        }
        return shakeCount;
    }

    private IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog("You can't run from trainer battles!", true);
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Tazo.Speed;
        int enemySpeed = enemyUnit.Tazo.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog("Ran away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog("Ran away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog("Can't escape!", true);
                state = BattleState.RunningTurn;
            }
        }
    }
}