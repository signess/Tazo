using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public bool IsRepeatable => false;

    public void OnPlayerTrigger(PlayerController player)
    {
        player.Character.Animator.IsMoving = false;
        GameController.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}
