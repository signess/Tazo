using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerFOV : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTrigger(PlayerController player)
    {
        GameController.Instance.OnEnterTrainerView(GetComponentInParent<TrainerController>());
    }
}
