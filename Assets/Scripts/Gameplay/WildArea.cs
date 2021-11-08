using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildArea : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTrigger(PlayerController player)
    {
        Debug.Log("Wild Area");
        if (UnityEngine.Random.Range(1, 101) <= 10)
        {
            GameController.Instance.StartBattle();
        }
    }
}
