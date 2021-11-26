using UnityEngine;

public class ItemBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] string description;
    [SerializeField] Sprite icon;

    public virtual string Name => name;
    public string Description => description;
    public Sprite Icon => icon;

    public virtual bool IsReuseable => false;
    public virtual bool CanUseInsideBattle => true;
    public virtual bool CanUseOutsideBattle => true;

    public virtual bool Use(Tazo tazo)
    {
        return false;
    }
}
