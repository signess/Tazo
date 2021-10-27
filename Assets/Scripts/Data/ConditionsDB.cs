using System;
using System.Collections.Generic;
public class ConditionsDB
{
    public static void Init()
    {
        foreach (var kvp in Conditions)
        {
            var conditionID = kvp.Key;
            var condition = kvp.Value;

            condition.ID = conditionID;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn, new Condition()
            {
                Name = "Poison",
                Icon = GlobalSettings.Instance.PoisonIcon,
                StartMessage = "has been poisoned.",
                OnAfterTurn = (Tazo tazo) =>
                {
                    tazo.DecreaseHP(tazo.MaxHp/8);
                    tazo.StatusChanges.Enqueue($"{tazo.Base.Name} hurt itself due to poison.");
                }
            }
        },
        {
             ConditionID.brn, new Condition()
            {
                Name = "Burn",
                Icon = GlobalSettings.Instance.BurnIcon,
                StartMessage = "has been burned.",
                OnAfterTurn = (Tazo tazo) =>
                {
                    tazo.DecreaseHP(tazo.MaxHp/16);
                    tazo.StatusChanges.Enqueue($"{tazo.Base.Name} hurt itself due to burn.");
                }
            }
        },
        {
             ConditionID.par, new Condition()
            {
                Name = "Paralyzed",
                Icon = GlobalSettings.Instance.ParalyzeIcon,
                StartMessage = "has been paralyzed.",
                OnBeforeMove = (Tazo tazo) =>
                {
                    if(UnityEngine.Random.Range(1,5) == 1)
                    {
                        tazo.StatusChanges.Enqueue($"{tazo.Base.Name}'s paralyzed and can't move.");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
             ConditionID.frz, new Condition()
            {
                Name = "Freeze",
                Icon = GlobalSettings.Instance.FreezeIcon,
                StartMessage = "has been frtazoozen.",
                OnBeforeMove = (Tazo tazo) =>
                {
                    if(UnityEngine.Random.Range(1,5) == 1)
                    {
                        tazo.CureStatus();
                        tazo.StatusChanges.Enqueue($"{tazo.Base.Name} is not frozen anymore.");
                        return true;
                    }
                    return false;
                }
            }
        },
        {
             ConditionID.slp, new Condition()
            {
                Name = "Sleep",
                Icon = GlobalSettings.Instance.SleepIcon,
                StartMessage = "has fallen asleep.",
                OnStart = (Tazo tazo) =>
                {
                    //Sleep for 1-3 turns
                    tazo.StatusTime = UnityEngine.Random.Range(1,4);
                },
                OnBeforeMove = (Tazo tazo) =>
                {
                    if(tazo.StatusTime <= 0)
                    {
                        tazo.CureStatus();
                        tazo.StatusChanges.Enqueue($"{tazo.Base.Name} woke up!");
                        return true;
                    }
                    tazo.StatusTime--;
                    tazo.StatusChanges.Enqueue($"{tazo.Base.Name} is sleeping.");
                    return false;
                }
            }
        },
        //VOLATILE STATUS
        {
             ConditionID.confusion, new Condition()
            {
                Name = "Confusion",
                Icon = null,
                StartMessage = "has been confused.",
                OnStart = (Tazo tazo) =>
                {
                    //Confused for 1-4 turns
                    tazo.VolatileStatusTime = UnityEngine.Random.Range(1,5);
                },
                OnBeforeMove = (Tazo tazo) =>
                {
                    if(tazo.VolatileStatusTime <= 0)
                    {
                        tazo.CureVolatileStatus();
                        tazo.StatusChanges.Enqueue($"{tazo.Base.Name} snapped out of confusion!");
                        return true;
                    }
                    tazo.VolatileStatusTime--;
                    tazo.StatusChanges.Enqueue($"{tazo.Base.Name} is confused.");

                    if(UnityEngine.Random.Range(1,3) == 1)
                        return true;

                    tazo.DecreaseHP(tazo.MaxHp / 8);
                    tazo.StatusChanges.Enqueue("It hurt itself due to confusion.");
                    return false;
                }
            }
        }
    };

    public static float GetStatusBonus(Condition condition)
    {
        if (condition == null)
            return 1f;
        else if (condition.ID == ConditionID.slp || condition.ID == ConditionID.frz)
            return 2f;
        else if (condition.ID == ConditionID.par || condition.ID == ConditionID.psn || condition.ID == ConditionID.brn)
            return 1.5f;
        return 1f;
    }
}

public enum ConditionID
{
    none, psn, brn, slp, par, frz,
    confusion
}
