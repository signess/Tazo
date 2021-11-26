using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState
{ Idle, Walking, Dialog }

[RequireComponent(typeof(Character))]
public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private Dialog dialog;
    [SerializeField] private List<Vector2> movementPattern;
    [SerializeField] private float timeBetweenMovement;

    private NPCState state;
    private float idleTimer;
    private int currentPattern = 0;

    private Character character;
    private ItemGiver itemGiver;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
    }

    private void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenMovement)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
            }
        }
        character.HandleUpdate();
    }

    private IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            state = NPCState.Dialog;
            character.LookTowards(initiator.position);
            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else
                yield return DialogManager.Instance.ShowDialog(dialog);

            idleTimer = 0f;
            state = NPCState.Idle;
        }
    }
}