using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Tazo/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private TazoType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private bool alwaysHits;
    [SerializeField] private int ep;
    [SerializeField] private int priority;
    [SerializeField] private MoveCategory category;
    [SerializeField] private MoveEffects effects;
    [SerializeField] private List<SecondaryEffects> secondaryEffects;
    [SerializeField] private MoveTarget target;

    public string Name => name;
    public string Description => description;
    public TazoType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public bool AlwaysHits => alwaysHits;
    public int EP => ep;
    public int Priority { get => priority; }
    public MoveCategory Category { get => category; }
    public MoveEffects Effects { get => effects; }
    public List<SecondaryEffects> SecondaryEffects { get => secondaryEffects; }
    public MoveTarget Target { get => target; }
}

[System.Serializable]
public class MoveEffects
{
    [SerializeField] List<StatBoosts> boosts;
    [SerializeField] ConditionID status;
    [SerializeField] ConditionID volatileStatus;
    public List<StatBoosts> Boosts { get => boosts; }
    public ConditionID Status { get => status; }
    public ConditionID VolatileStatus { get => volatileStatus; }
}

[System.Serializable]
public class SecondaryEffects : MoveEffects
{
    [SerializeField] int chance;
    [SerializeField] MoveTarget target;

    public int Chance { get => chance; }
    public MoveTarget Target { get => target; }
}

[System.Serializable]
public class StatBoosts
{
    public Stat Stat;
    public int Boost;
}

[System.Serializable]
public enum MoveCategory
{
    Physical, Special, Status
}

public enum MoveTarget
{
    Foe, Self
}