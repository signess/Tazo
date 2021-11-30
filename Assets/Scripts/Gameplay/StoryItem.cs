using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Dialog dialog;

    [Header("Movement")]
    [SerializeField] Vector2 movementVector;

    public bool IsRepeatable => false;

    public void OnPlayerTrigger(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
            player.Character.Move(movementVector);
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

    }
}
