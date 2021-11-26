using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Tazo
{
    [SerializeField] private TazoBase _base;
    [SerializeField] private int level;

    public TazoBase Base { get => _base; }
    public int Level { get => level; }
    public Gender Gender { get; set; }
    public int Exp { get; set; }
    public int HP { get; set; }
    public List<Move> Moves { get; set; }
    public Move CurrentMove { get; set; }
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatBoosts { get; private set; }
    public Condition Status { get; private set; }
    public int StatusTime { get; set; }
    public Condition VolatileStatus { get; private set; }
    public int VolatileStatusTime { get; set; }
    public Queue<string> StatusChanges { get; private set; }
    public event System.Action OnStatusChanged;
    public event System.Action OnHPChanged;

    public int Attack
    {
        get => GetStat(Stat.Attack);
    }
    public int Defense
    {
        get => GetStat(Stat.Defense);
    }
    public int SpAttack
    {
        get => GetStat(Stat.SpAttack);
    }
    public int SpDefense
    {
        get => GetStat(Stat.SpDefense);
    }
    public int Speed
    {
        get => GetStat(Stat.Speed);
    }
    public int MaxHp { get; private set; }

    public Tazo(TazoBase tBase, int tLevel)
    {
        _base = tBase;
        level = tLevel;

        Init();
    }

    public Tazo(TazoSaveData saveData)
    {
        _base = TazoDB.GetTazoByName(saveData.Name);
        HP = saveData.HP;
        level = saveData.Level;
        Exp = saveData.Exp;

        if (saveData.StatusID != null)
            Status = ConditionsDB.Conditions[saveData.StatusID.Value];
        else
            Status = null;

        //Restore moves
        Moves = saveData.Moves.Select(s => new Move(s)).ToList();

        CalculateStats();
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        VolatileStatus = null;
    }

    public void Init()
    {
        Gender = (Gender)Random.Range(0, 2);
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));

            if (Moves.Count >= 4)
                break;
        }

        Exp = Base.GetExpForLevel(Level);

        CalculateStats();
        HP = MaxHp;
        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolatileStatus = null;
    }

    public TazoSaveData GetSaveData()
    {
        var saveData = new TazoSaveData()
        {
            Name = Base.Name,
            HP = HP,
            Level = Level,
            Exp = Exp,
            StatusID = Status?.ID,
            Moves = Moves.Select(m => m.GetSaveData()).ToList()
        };
        return saveData;
    }

    public bool CheckForLevelUp()
    {
        if(Exp >= Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false;
    }

    public LearnableMove GetLearnableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(x => x.Level == level).FirstOrDefault();
    }

    public void LearnMove(MoveBase moveToLearn)
    {
        if (Moves.Count > 4)
            return;
        Moves.Add(new Move(moveToLearn));
    }

    public bool HasMove(MoveBase moveToCheck)
    {
        return Moves.Count(m => m.Base == moveToCheck) > 0;
    }

    private void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();

        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5);
        Stats.Add(Stat.Defense, Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack * Level) / 100f) + 5);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt((Base.SpDefense * Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10 + Level;
    }

    private int GetStat(Stat stat)
    {
        int statVal = Stats[stat];

        //Apply stat boost
        int boost = StatBoosts[stat];
        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (boost >= 0)
            statVal = Mathf.FloorToInt(statVal * boostValues[boost]);
        else
            statVal = Mathf.FloorToInt(statVal / boostValues[-boost]);

        return statVal;
    }

    private void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0}, {Stat.Defense,0}, {Stat.SpAttack, 0}, {Stat.SpDefense,0}, {Stat.Speed, 0},

            {Stat.Accuracy, 0}, {Stat.Evasion, 0}
        };
    }

    public void ApplyBoost(List<StatBoosts> statBoosts)
    {
        foreach (var statBoost in statBoosts)
        {
            var stat = statBoost.Stat;
            var boost = statBoost.Boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost, -6, 6);

            if (boost > 0)
                StatusChanges.Enqueue($"{Base.Name}'s {stat} rose!");
            else
                StatusChanges.Enqueue($"{Base.Name}'s {stat} fell!");
        }
    }

    public DamageDetails TakeDamage(Move move, Tazo attacker)
    {
        float critical = 1f;
        if (Random.value * 100f <= 6.25f)
            critical = 2f;

        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type1) * TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type2);

        var damageDetails = new DamageDetails()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false
        };

        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float defense = (move.Base.Category == MoveCategory.Special) ? SpDefense : Defense;

        float modifiers = Random.Range(0.85f, 1f) * type * critical;
        float a = (2 * attacker.Level + 10) / 250f;
        float b = a * move.Base.Power * (attack / defense) + 2;
        int damage = Mathf.FloorToInt(b * modifiers);

        DecreaseHP(damage);

        return damageDetails; ;
    }

    public void IncreaseHP(int amount)
    {
        HP = Mathf.Clamp(HP + amount, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void DecreaseHP(int damage)
    {
        HP = Mathf.Clamp(HP - damage, 0, MaxHp);
        OnHPChanged?.Invoke();
    }

    public void SetStatus(ConditionID conditionID)
    {
        if (Status != null) return;

        Status = ConditionsDB.Conditions[conditionID];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {
        if (VolatileStatus != null) return;

        VolatileStatus = ConditionsDB.Conditions[conditionID];
        VolatileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolatileStatus.StartMessage}");
    }

    public void CureVolatileStatus()
    {
        VolatileStatus = null;
    }

    public bool OnBeforeMove()
    {
        bool canPerformMove = true;
        if (Status?.OnBeforeMove != null)
        {
            if (!Status.OnBeforeMove(this))
                canPerformMove = false;
        }

        if (VolatileStatus?.OnBeforeMove != null)
        {
            if (!VolatileStatus.OnBeforeMove(this))
                canPerformMove = false;
        }
        return canPerformMove;
    }

    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolatileStatus?.OnAfterTurn?.Invoke(this);
    }

    public void OnBattleOver()
    {
        VolatileStatus = null;
        ResetStatBoost();
    }

    public Move GetRandomMove()
    {
        var movesWithEP = Moves.Where(x => x.EP > 0).ToList();

        int r = Random.Range(0, movesWithEP.Count);
        return Moves[r];
    }
}

public enum Gender
{
    Male,
    Female,
    Neuter
}

public class DamageDetails
{
    public bool Fainted { get; set; }
    public float Critical { get; set; }
    public float TypeEffectiveness { get; set; }
}

[System.Serializable]
public class TazoSaveData
{
    public string Name;
    public int HP;
    public int Level;
    public int Exp;
    public ConditionID? StatusID;
    public List<MoveSaveData> Moves;
}