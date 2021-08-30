using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName ="Tazo/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private TazoType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int ep;
    [SerializeField] int priority;
    [SerializeField] MoveCategory moveCategory;

    public string Name => name;
    public string Description => description;
    public TazoType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int EP => ep;
    public int Priority { get => priority; }
    public MoveCategory MoveCategory { get => moveCategory; }
}

[System.Serializable]
public enum MoveCategory
{
    Physical, Special, Status
}