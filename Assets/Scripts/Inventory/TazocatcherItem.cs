using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tazocatcher", menuName = "Items/Tazocatcher Item")]
public class TazocatcherItem : ItemBase
{
    public override bool Use(Tazo tazo)
    {
        return true;
    }
}
