using UnityEngine;

[CreateAssetMenu(fileName = "Recovery Item", menuName = "Items/Recovery Item")]
public class RecoveryItem : ItemBase
{
    [Header("HP")]
    [SerializeField] private int hpAmount;
    [SerializeField] private bool restoreMaxHP;

    [Header("EP")]
    [SerializeField] private int epAmount;
    [SerializeField] private bool restoreMaxPP;

    [Header("Status Conditions")]
    [SerializeField] private ConditionID status;
    [SerializeField] private bool recoverAllStatus;

    [Header("Revive")]
    [SerializeField] private bool revive;
    [SerializeField] private bool maxRevive;

    public override bool Use(Tazo tazo)
    {
        if(hpAmount > 0)
        {
            if (tazo.HP == tazo.MaxHp)
                return false;

            tazo.IncreaseHP(hpAmount);
        }

        return true;
    }
}