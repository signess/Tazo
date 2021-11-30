using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerTriggerable
{
    bool IsRepeatable { get; }
    void OnPlayerTrigger(PlayerController player);
}
