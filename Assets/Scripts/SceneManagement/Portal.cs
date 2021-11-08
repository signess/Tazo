using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTrigger(PlayerController player)
    {
        print("Step on portal");
    }

}
