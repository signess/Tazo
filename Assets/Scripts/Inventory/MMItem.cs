using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoveMachine", menuName = "Items/Movemachine Item")]
public class MMItem : ItemBase
{
    [SerializeField] private MoveBase move;
    [SerializeField] private bool isSM;

    public override string Name => base.Name + $" - {move.Name}";
    public MoveBase Move => move;
    public bool IsSM => isSM;
    public override bool IsReuseable => isSM;

    public override bool CanUseInsideBattle => false;

    public override bool Use(Tazo tazo)
    {
        //Learning move is handled from ui
        return tazo.HasMove(Move);
    }

    public bool CanBeTaught(Tazo tazo)
    {
        return tazo.Base.LearnableByItems.Contains(Move);
    }
}
