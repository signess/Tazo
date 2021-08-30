using System;

public class Condition
{
    public ConditionID ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StartMessage { get; set; }
    public Action<Tazo> OnStart { get; set; }
    public Func<Tazo, bool> OnBeforeMove { get; set; }
    public Action<Tazo> OnAfterTurn { get; set; }
}
