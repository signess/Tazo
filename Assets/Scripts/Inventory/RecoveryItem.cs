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
        // REVIVE
        if(revive || maxRevive)
        {
            if (tazo.HP > 0)
                return false;
            if (revive)
                tazo.IncreaseHP(tazo.MaxHp / 2);
            else if (maxRevive)
                tazo.IncreaseHP(tazo.MaxHp);
            tazo.CureStatus();

            return true;
        }

        //No other items can be used on a fainted tazo.
        if (tazo.HP <= 0)
            return false;

        // RESTORE HP
        if(restoreMaxHP||hpAmount > 0)
        {
            if (tazo.HP == tazo.MaxHp)
                return false;

            if (restoreMaxHP)
                tazo.IncreaseHP(tazo.MaxHp);
            else
            tazo.IncreaseHP(hpAmount);
        }

        //Status Ailments
        if(recoverAllStatus || status != ConditionID.none)
        {
            if (tazo.Status == null && tazo.VolatileStatus != null)
                return false;

            if(recoverAllStatus)
            {
                tazo.CureStatus();
                tazo.CureVolatileStatus();
            }
            else
            {
                if (tazo.Status.ID == status)
                    tazo.CureStatus();
                else if (tazo.VolatileStatus.ID == status)
                    tazo.CureVolatileStatus();
                else
                    return false;
            }
        }

        //Restore EP
        if(restoreMaxPP)
        {
            tazo.Moves.ForEach(m => m.IncreaseEP(m.Base.EP));

        }
        else if(epAmount > 0)
        {
            tazo.Moves.ForEach(m => m.IncreaseEP(epAmount));
        }

        return true;
    }
}